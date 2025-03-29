using AIPoweredSQLConverter.API;
using AIPoweredSQLConverter.Business;
using AIPoweredSQLConverter.Common;
using AIPoweredSQLConverter.Host.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Stripe;
using Stripe.Checkout;
using System.Net;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace AIPoweredSQLConverter.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebhookController : ControllerBase
    {
        private readonly IAuthLogic _authLogic;
        private readonly IPromptFlowLogic _promptFlowLogic;

        private readonly string _stripeKey = string.Empty;
        private readonly string _signupWebhookKey = string.Empty;
        private readonly string _cancelationWebhookKey = string.Empty;
        private readonly string _meterPriceKey = string.Empty;
        private readonly string _domain = string.Empty;

        public WebhookController(IAuthLogic authLogic, IPromptFlowLogic promptLogic, StripeSettings settings)
        {
            _authLogic = authLogic;
            _promptFlowLogic = promptLogic;

            _stripeKey = settings.SecretKey;
            _signupWebhookKey = settings.SignupWebhookKey;
            _cancelationWebhookKey = settings.CancelationWebhookKey;
            _meterPriceKey = settings.MeterPriceKey;
            _domain = settings.Domain;
        }

        [Authorize(Policy = "Auth0Policy")]
        [HttpPost("create-checkout-session/{username}")]
        public IActionResult CreateCheckoutSession([FromRoute] string username)
        {
            try
            {
                if (string.IsNullOrEmpty(username)) return BadRequest("The sub ID is required.");

                StripeConfiguration.ApiKey = _stripeKey;

                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = new List<SessionLineItemOptions> { new SessionLineItemOptions { Price = _meterPriceKey }, },
                    Mode = "subscription",
                    SuccessUrl = _domain + "/home",
                    CancelUrl = _domain + "/home",
                    // Pass the user's identifier so that you can update your DB later.
                    ClientReferenceId = username // Replace with the actual username
                };

                var service = new SessionService();
                Stripe.Checkout.Session session = service.Create(options);

                return Ok(new { sessionId = session.Id });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong when authenticating");
            }
        }

        [Authorize(Policy = "Auth0Policy")]
        [HttpPost("create-portal-session/{username}")]
        public async Task<IActionResult> CreatePortalSession([FromRoute] string username)
        {
            try
            {
                // In this example we expect the customer identifier to be sent as "sub".
                // Adjust accordingly if you store your Stripe customer id differently.
                if (string.IsNullOrEmpty(username)) return BadRequest("The customer ID is required.");

                // if user has no subscription data, redirect to checkout
                var customer = await _promptFlowLogic.GetUser(username);
                if (customer.Data == null || !customer.Data.IsPayingCustomer) return CreateCheckoutSession(username); // fix this implementation

                StripeConfiguration.ApiKey = _stripeKey;

                var options = new Stripe.BillingPortal.SessionCreateOptions
                {
                    Customer = customer.Data.StripeCustomerId,
                    ReturnUrl = _domain + "/home",
                };

                var service = new Stripe.BillingPortal.SessionService();
                var session = service.Create(options);

                return Ok(new { url = session.Url });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong while creating the portal session");
            }
        }

        [HttpPost("stripe-update-ispaying")]
        public async Task<IActionResult> StripeWebhookPaying()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            try
            {
                // Verify and construct the Stripe event using your webhook secret
                var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], _signupWebhookKey);

                // Check if the event is "checkout.session.completed"
                if (stripeEvent.Type == "checkout.session.completed")
                {
                    // Cast the event data to a Stripe Checkout Session object
                    var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
                    if (session != null)
                    {
                        // Extract client_reference_id and customer ID from the session
                        var clientReferenceId = session.ClientReferenceId;
                        var customerId = session.CustomerId;

                        // Ensure both required fields are present
                        if (!string.IsNullOrEmpty(clientReferenceId) && !string.IsNullOrEmpty(customerId))
                        {
                            // Call MarkUserAsPaying with client_reference_id instead of username
                            var result = await _promptFlowLogic.MarkUserAsPaying(clientReferenceId, customerId);
                            if (result.Success) return Ok();
                            return StatusCode(StatusCodes.Status500InternalServerError, result.Message);
                        }
                        return StatusCode(StatusCodes.Status500InternalServerError, $"session id error. client reference id or customer id error. reference id: {clientReferenceId}. customer id: {customerId}");
                    }
                    return StatusCode(StatusCodes.Status500InternalServerError, $"session id error. session: {session}");
                }
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error updating paying status in the database. stripe event: {stripeEvent.Type}");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }



        [HttpPost("stripe-update-isnonpaying")]
        public async Task<IActionResult> StripeWebhookNonPaying()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], _cancelationWebhookKey);
                if (stripeEvent.Type == "customer.subscription.deleted")
                {
                    var subscription = stripeEvent.Data.Object as Stripe.Subscription;
                    var customerId = subscription?.Customer?.Id ?? subscription?.CustomerId;
                    if (!string.IsNullOrEmpty(customerId))
                    {
                        var result = await _promptFlowLogic.MarkUserAsNonPaying(customerId);
                        if (!result.Success) return StatusCode(StatusCodes.Status500InternalServerError, "Error updating non-paying status in the database.");
                    }
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating non-paying status in the database.");
            }
        }
    }
}
