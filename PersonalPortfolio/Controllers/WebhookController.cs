using PersonalPortfolio.API;
using PersonalPortfolio.Business;
using PersonalPortfolio.Common;
using PersonalPortfolio.Host.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Stripe;
using Stripe.Checkout;
using System.Net;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace PersonalPortfolio.Controllers
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
    }
}
