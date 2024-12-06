using AiPocWebsiteTemplateWithBackend.API;
using AiPocWebsiteTemplateWithBackend.API.Config;
using AiPocWebsiteTemplateWithBackend.Business;
using Microsoft.AspNetCore.Mvc;

namespace AiPocWebsiteTemplateWithBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApiController : ControllerBase
    {
        private readonly IAuthLogic _authLogic;
        private readonly IPromptFlowLogic _promptFlowLogic;

        public ApiController(IAuthLogic authLogic, IPromptFlowLogic promptLogic)
        {
            _authLogic = authLogic;
            _promptFlowLogic = promptLogic;
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
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong");
            }
        }
    }
}