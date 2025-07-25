using EasyHealth.API;
using EasyHealth.Business;
using EasyHealth.Common;
using EasyHealth.Host.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EasyHealth.Client.IntelligenceHub;

namespace EasyHealth.Controllers
{
    [ApiController]
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

        [HttpGet("get/newBearerKey")]
        public async Task<IActionResult> GetStreamKey()
        {
            try
            {
                var response = await _promptFlowLogic.GetIntelligenceHubBearerKey();

                if (response.Success) return Ok(response.Data);
                else return StatusCode((int)response.StatusCode, response.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong when generating the API key");
            }
        }
    }
}
