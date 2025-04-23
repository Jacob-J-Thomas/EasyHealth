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

namespace PersonalPortfolio.Controllers
{
    [ApiController]
    [Authorize(Policy = Policies.PublicApiPolicy)]
    [Route("[controller]")]
    public class ApiController : ControllerBase
    {
        private readonly IAuthLogic _authLogic;
        private readonly IPublicApiLogic _apiLogic;

        public ApiController(IAuthLogic authLogic, IPublicApiLogic publicApiLogic, StripeSettings settings)
        {
            _authLogic = authLogic;
            _apiLogic = publicApiLogic;
        }
    }
}
