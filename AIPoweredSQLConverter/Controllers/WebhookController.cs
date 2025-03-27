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

namespace AIPoweredSQLConverter.Controllers
{
    [ApiController]
    [Authorize]
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

        [HttpPost("create-checkout-session/{username}")]
        public IActionResult CreateCheckoutSession([FromRoute] string username)
        {
            try
            {
                string sub = username;
                if (string.IsNullOrEmpty(sub)) return BadRequest("The sub ID is required.");

                StripeConfiguration.ApiKey = _stripeKey;

                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = new List<SessionLineItemOptions> { new SessionLineItemOptions { Price = _meterPriceKey }, },
                    Mode = "subscription",
                    SuccessUrl = _domain + "/home",
                    CancelUrl = _domain + "/home",
                    // Pass the user's identifier so that you can update your DB later.
                    ClientReferenceId = sub // Replace with the actual username
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
                    ReturnUrl = _domain + "/home"
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
            // Read the raw JSON from the request body.
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                // Construct the event using Stripe's utility
                var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], _signupWebhookKey);

                // Check for the subscription creation event (indicating a new paying user)
                if (stripeEvent.Type == "customer.subscription.created")
                {
                    var subscription = stripeEvent.Data.Object as Stripe.Subscription;
                    // Retrieve the username from metadata
                    var username = subscription?.Metadata["username"];
                    // Retrieve the customer ID from the subscription
                    var customerId = subscription?.Customer.Id;

                    if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(customerId))
                    {
                        // Update the user's record to mark them as paying, including the customerID.
                        var result = await _promptFlowLogic.MarkUserAsPaying(username, customerId);
                        if (!result.Success)
                        {
                            // Optionally log the error and return a non-200 response if needed.
                            return StatusCode(StatusCodes.Status500InternalServerError, "Error updating paying status in the database.");
                        }
                    }
                }

                // Acknowledge receipt of the event.
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong when authenticating");
            }
        }



        [HttpPost("stripe-update-isnonpaying")]
        public async Task<IActionResult> StripeWebhookNonPaying()
        {
            // Read the raw JSON from the request body.
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], _cancelationWebhookKey);

                if (stripeEvent.Type == "customer.subscription.deleted")
                {
                    var subscription = stripeEvent.Data.Object as Stripe.Subscription;
                    // Use the ClientReferenceId (or metadata) to identify the user.
                    var username = subscription?.Metadata["username"];
                    if (!string.IsNullOrEmpty(username))
                    {
                        // Update the user's record to mark them as non-paying.
                        var result = await _promptFlowLogic.MarkUserAsNonPaying(username);
                        if (!result.Success)
                        {
                            // Optionally log the error and return a non-200 response if needed.
                            return StatusCode(StatusCodes.Status500InternalServerError, "Error updating non-paying status in the database.");
                        }
                    }
                }

                // Acknowledge receipt of the event.
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong when authenticating");
            }
        }
    }
}
