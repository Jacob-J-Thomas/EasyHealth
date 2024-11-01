
using AiPocWebsiteTemplateWithBackend.API;
using AiPocWebsiteTemplateWithBackend.API.Config;
using AiPocWebsiteTemplateWithBackend.Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace AiPocWebsiteTemplateWithBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApiController : ControllerBase
    {
        private readonly AuthLogic _authLogic;
        private readonly PromptFlowLogic _promptFlowLogic;
        private readonly ILogger<ApiController> _logger;

        public ApiController(AuthSettings authSettings, AIHubSettings hubSettings, IHttpClientFactory factory, ILogger<ApiController> logger)
        {
            _logger = logger;
            _authLogic = new AuthLogic(authSettings, factory);
            _promptFlowLogic = new PromptFlowLogic(authSettings, hubSettings, factory);
        }

        [HttpGet("Auth")]
        public async Task<IActionResult> Authorize()
        {
            try
            {
                var result = await _authLogic.RetrieveAuthToken();
                if (result?.AccessToken == null) return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong when authenticating");

                // scrub unnescessary data
                var clientSafeResult = new AuthTokenResponse()
                {
                    AccessToken = result.AccessToken,
                    ExpiresIn = result.ExpiresIn,
                };

                return Ok(clientSafeResult);
                
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong when authenticating");
            }
        }

        [HttpGet("Test")]
        public async Task<IActionResult> Test()
        {
            try
            {
                var result = await _promptFlowLogic.Test();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong when authenticating");
            }
        }
    }
}