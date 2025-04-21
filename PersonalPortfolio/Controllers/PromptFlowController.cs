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
using PersonalPortfolio.Client.IntelligenceHub;

namespace PersonalPortfolio.Controllers
{
    [ApiController]
    [Authorize(Policy = Policies.Auth0Policy)]
    [Route("[controller]")]
    public class PromptFlowController : ControllerBase
    {
        private readonly IAuthLogic _authLogic;
        private readonly IPromptFlowLogic _promptFlowLogic;

        public PromptFlowController(IAuthLogic authLogic, IPromptFlowLogic promptLogic, StripeSettings settings)
        {
            _authLogic = authLogic;
            _promptFlowLogic = promptLogic;
        }

        [HttpGet("get/newBearerKey/{username}")]
        public async Task<IActionResult> GetStreamKey([FromRoute] string username)
        {
            try
            {
                if (string.IsNullOrEmpty(username)) return BadRequest("The username is required.");

                var response = await _promptFlowLogic.GetIntelligenceHubBearerKey(username);

                if (response.Success) return Ok(response.Data);
                else return StatusCode((int)response.StatusCode, response.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong when generating the API key");
            }
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

        [HttpGet("get/profile/{profile}")]
        public async Task<IActionResult> GetProfile([FromRoute] string profile)
        {
            try
            {
                if (string.IsNullOrEmpty(profile)) return BadRequest("The profile name is required.");

                var response = await _promptFlowLogic.GetProfile(profile);

                if (response.Success) return Ok(response.Data);
                else return StatusCode((int)response.StatusCode, response.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong when getting the profile.");
            }
        }

        [HttpPost("post/profile")]
        public async Task<IActionResult> UpsertProfile([FromBody] Profile profile)
        {
            try
            {
                if (string.IsNullOrEmpty(profile.Name)) return BadRequest("The profile name is required.");

                var response = await _promptFlowLogic.UpsertProfile(profile);

                if (response.Success) return Ok(response.Data);
                else return StatusCode((int)response.StatusCode, response.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong when generating the API key");
            }
        }

        [HttpGet("get/Users/{username}")]
        public async Task<IActionResult> GetUserData([FromRoute] string username)
        {
            try
            {
                if (string.IsNullOrEmpty(username)) return BadRequest("The username is required.");
                var response = await _promptFlowLogic.GetUserData(username);
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
    }
}
