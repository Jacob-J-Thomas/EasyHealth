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

namespace ConversationalAIWebsite.Controllers
{
    [ApiController]
    [Authorize(Policy = "Auth0Policy")]
    [Route("[controller]")]
    public class PromptFlowController : ControllerBase
    {
        private readonly IAuthLogic _authLogic;
        private readonly IPromptFlowLogic _promptFlowLogic;

        // Default error message for 429 responses
        private const string _requestLimitExceededMessage = "You have reached the maximum number of requests allowed per day.";

        public PromptFlowController(IAuthLogic authLogic, IPromptFlowLogic promptLogic, StripeSettings settings)
        {
            _authLogic = authLogic;
            _promptFlowLogic = promptLogic;
        }

        [HttpGet("get/newAPIKey/{username}")]
        public async Task<IActionResult> GetNewAPIKey([FromRoute] string username)
        {
            try
            {
                if (string.IsNullOrEmpty(username)) return BadRequest("The username is required.");

                var response = await _promptFlowLogic.GenerateAndStoreApiKey(username);

                if (response.Success) return Ok(response.Data);
                else return StatusCode((int)response.StatusCode, response.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong when generating the API key");
            }
        }

        [HttpGet("get/sqlData/{username}")]
        public IActionResult GetSQLData([FromRoute] string username)
        {
            try
            {
                if (string.IsNullOrEmpty(username)) return BadRequest("The username is required.");
                var response = _promptFlowLogic.GetSQLData(username);
                if (response.Success) return Ok(response.Data);
                else return StatusCode((int)response.StatusCode, response.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong");
            }
        }

        [HttpPost("post/saveUser/{username}")]
        public async Task<IActionResult> SaveUserData([FromRoute] string username)
        {
            try
            {
                if (string.IsNullOrEmpty(username)) return BadRequest("The sub ID is required.");

                var response = await _promptFlowLogic.SaveNewUser(username);

                if (response.Success) return Ok(response.Data);
                else return StatusCode((int)response.StatusCode, response.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong when saving user data");
            }
        }

        [HttpPost("post/sqlData")]
        public async Task<IActionResult> SaveSQLData([FromBody] FrontEndRequest requestBody)
        {
            try
            {
                if (requestBody == null) return BadRequest("The request body is malformed.");
                if (string.IsNullOrEmpty(requestBody.Username)) return BadRequest("The username is required.");
                if (string.IsNullOrEmpty(requestBody.SqlData)) return BadRequest("The SQL data is required.");

                var response = await _promptFlowLogic.UpsertUserSQLData(requestBody);

                if (response.Success) return NoContent();
                else return StatusCode((int)response.StatusCode, response.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong when authenticating");
            }
        }

        [HttpPost("post/sqlHelp")]
        public async Task<IActionResult> RequestSQLDataHelp([FromBody] FrontEndRequest requestBody)
        {
            try
            {
                if (requestBody == null) return BadRequest("The request body is malformed.");
                if (string.IsNullOrEmpty(requestBody.Username)) return BadRequest("The username is required.");
                if (string.IsNullOrEmpty(requestBody.SqlData)) return BadRequest("The SQL data is required.");
                if (string.IsNullOrEmpty(requestBody.Query)) return BadRequest("The query is required.");

                var response = await _promptFlowLogic.GetSQLDataHelp(requestBody);

                if (response.Success) return Ok(response.Data);
                else if (response.StatusCode == HttpStatusCode.TooManyRequests) return StatusCode(429, _requestLimitExceededMessage);
                else if (response.Message.Contains("concurrency")) return Conflict("A concurrency error occurred. Please retry the request.");
                else return StatusCode((int)response.StatusCode, response.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong when requesting SQL assistance");
            }
        }

        [HttpPost("post/convertQuery")]
        public async Task<IActionResult> ConvertQueryToSQL([FromBody] FrontEndRequest requestBody)
        {
            try
            {
                if (requestBody == null) return BadRequest("The request body is malformed.");
                if (string.IsNullOrEmpty(requestBody.Username)) return BadRequest("The username is required.");
                if (!requestBody.Messages.Any(x => x.Role == Client.IntelligenceHub.Role.User)) return BadRequest("A user query is required.");

                var response = await _promptFlowLogic.ConvertQueryToSQL(requestBody);

                if (response.Success) return Ok(response.Data);
                else if (response.StatusCode == HttpStatusCode.TooManyRequests) return StatusCode(429, _requestLimitExceededMessage);
                else if (response.Message.Contains("concurrency")) return Conflict("A concurrency error occurred. Please retry the request.");
                else return StatusCode((int)response.StatusCode, response.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong during SQL conversion.");
            }
        }
    }
}
