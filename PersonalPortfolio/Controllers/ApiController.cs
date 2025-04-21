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

        [HttpGet("get/Users")]
        public async Task<IActionResult> GetUserData()
        {
            try
            {
                if (!Request.Headers.TryGetValue("x-api-key", out var apiKey)) return BadRequest(new PublicApiResponse { Success = false, ErrorMesssage = "The API key is required." });
                var response = await _apiLogic.GetUserData(apiKey);
                if (response.Success) return Ok(new PublicApiResponse { Success = true, Data = response.Data?.ExampleDataField });
                return StatusCode((int)response.StatusCode, new PublicApiResponse { Success = false, ErrorMesssage = response.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new PublicApiResponse { Success = false, ErrorMesssage = "Something went wrong" });
            }
        }

        [HttpPost("post/sqlData")]
        public async Task<IActionResult> SaveUserData([FromBody] PublicApiRequest requestBody)
        {
            try
            {
                if (requestBody == null) return BadRequest(new PublicApiResponse { Success = false, ErrorMesssage = "The request body is malformed." });
                if (!Request.Headers.TryGetValue("x-api-key", out var apiKey)) return BadRequest(new PublicApiResponse { Success = false, ErrorMesssage = "The API key is required." });

                var response = await _apiLogic.UpsertUserData(requestBody, apiKey);

                if (response.Success) return NoContent();
                else return StatusCode((int)response.StatusCode, new PublicApiResponse { Success = false, ErrorMesssage = response.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new PublicApiResponse { Success = false, ErrorMesssage = "Something went wrong when authenticating" });
            }
        }
    }
}
