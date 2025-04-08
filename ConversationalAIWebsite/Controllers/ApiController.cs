using ConversationalAIWebsite.API;
using ConversationalAIWebsite.Business;
using ConversationalAIWebsite.Common;
using ConversationalAIWebsite.Host.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Stripe;
using Stripe.Checkout;
using System.Net;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;
using Microsoft.AspNetCore.Authorization;
using ConversationalAIWebsite.ClientApp.src.components;

namespace ConversationalAIWebsite.Controllers
{
    [ApiController]
    [Authorize(Policy = "ApiKeyPolicy")]
    [Route("[controller]")]
    public class ApiController : ControllerBase
    {
        private readonly IAuthLogic _authLogic;
        private readonly IPublicApiLogic _apiLogic;

        // Default error message for 429 responses
        private const string _requestLimitExceededMessage = "You have reached the maximum number of requests allowed per day.";

        public ApiController(IAuthLogic authLogic, IPublicApiLogic publicApiLogic, StripeSettings settings)
        {
            _authLogic = authLogic;
            _apiLogic = publicApiLogic;
        }

        [HttpGet("get/sqlData")]
        public async Task<IActionResult> GetSQLData()
        {
            try
            {
                if (!Request.Headers.TryGetValue("x-api-key", out var apiKey)) return BadRequest(new PublicApiResponse { Success = false, ErrorMesssage = "The API key is required." });
                var response = await _apiLogic.GetSQLData(apiKey);
                if (response.Success) return Ok(new PublicApiResponse { Success = true, Data = response.Data });
                return StatusCode((int)response.StatusCode, new PublicApiResponse { Success = false, ErrorMesssage = response.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new PublicApiResponse { Success = false, ErrorMesssage = "Something went wrong" });
            }
        }

        [HttpPost("post/sqlData")]
        public async Task<IActionResult> SaveSQLData([FromBody] ApiRequest requestBody)
        {
            try
            {
                if (requestBody == null) return BadRequest(new PublicApiResponse { Success = false, ErrorMesssage = "The request body is malformed." });
                if (!Request.Headers.TryGetValue("x-api-key", out var apiKey)) return BadRequest(new PublicApiResponse { Success = false, ErrorMesssage = "The API key is required." });

                var response = await _apiLogic.UpsertUserSQLData(requestBody, apiKey);

                if (response.Success) return NoContent();
                else return StatusCode((int)response.StatusCode, new PublicApiResponse { Success = false, ErrorMesssage = response.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new PublicApiResponse { Success = false, ErrorMesssage = "Something went wrong when authenticating" });
            }
        }

        [HttpPost("post/convertQuery")]
        public async Task<IActionResult> ConvertQueryToSQL([FromBody] ApiRequest requestBody)
        {
            try
            {
                if (requestBody == null) return BadRequest(new PublicApiResponse { Success = false, ErrorMesssage = "The request body is malformed." });
                if (!Request.Headers.TryGetValue("x-api-key", out var apiKey)) return BadRequest(new PublicApiResponse { Success = false, ErrorMesssage = "The API key is required." });

                if (string.IsNullOrEmpty(requestBody.Query)) return BadRequest(new PublicApiResponse { Success = false, ErrorMesssage = "A query is required." });

                var response = await _apiLogic.ConvertQueryToSQL(requestBody, apiKey);

                if (response.Success)
                {
                    var remainingFreeQuota = await _apiLogic.GetRemainingFreeRequests(apiKey);
                    return Ok(new PublicApiResponse { Success = true, Data = response.Data, RemainingFreeRequests = remainingFreeQuota.Data - 1 }); // minus 1 to account for the current request
                }
                else if (response.StatusCode == HttpStatusCode.TooManyRequests) return StatusCode(429, new PublicApiResponse { Success = false, ErrorMesssage = _requestLimitExceededMessage });
                else if (response.Message.Contains("concurrency")) return Conflict(new PublicApiResponse { Success = false, ErrorMesssage = "A concurrency error occurred. Please retry the request." });
                else return StatusCode((int)response.StatusCode, new PublicApiResponse { Success = false, ErrorMesssage = response.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new PublicApiResponse { Success = false, ErrorMesssage = "Something went wrong when authenticating" });
            }
        }
    }
}
