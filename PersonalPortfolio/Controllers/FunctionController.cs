
using PersonalPortfolio.API.Config;
using PersonalPortfolio.Business;
using PersonalPortfolio.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PersonalPortfolio.Controllers
{

    // PLEASE NOTE: This API is intended to be used in a somewhat unorthodox manner, in that, not only is it capable
    // of receiving requests from the client, but it is also capable of being called from the IntelligenceHub via
    // tool calls returned from the AI

    [ApiController]
    [Route("[controller]")]
    [Authorize(Policy = Policies.BasicAuthPolicy)]
    public class FunctionController : ControllerBase
    {
        private readonly IAuthLogic _authLogic;
        private readonly IPromptFlowLogic _promptFlowLogic;

        public FunctionController(IAuthLogic authLogic, IPromptFlowLogic promptLogic)
        {
            _authLogic = authLogic;
            _promptFlowLogic = promptLogic;
        }

        // Add methods here and create a corresponding tool to execute code with the AI models.

        // Alternatively, you can define tools to target other APIs and return the HttpResponse to the calling client.
        // See the read me file's section on tool calling for more details.
    }
}