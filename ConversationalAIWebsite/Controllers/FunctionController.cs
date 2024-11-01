
using AiPocWebsiteTemplateWithBackend.API.Config;
using AiPocWebsiteTemplateWithBackend.Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace AiPocWebsiteTemplateWithBackend.Controllers
{

    // PLEASE NOTE: This API is intended to be used in a somewhat unorthodox manner, in that, not only is it capable
    // of receiving requests from the client, but it is also capable of being called from the IntelligenceHub via
    // tool calls returned from the AI

    [ApiController]
    [Route("[controller]")]
    //[Authorize] // uncomment to enforce basic auth
    public class FunctionController : ControllerBase
    {
        private readonly ILogger<ApiController> _logger;

        public FunctionController(ILogger<ApiController> logger)
        {
            _logger = logger;
        }

        // Add methods here and create a corresponding tool to execute code with the AI models.

        // Alternatively, you can define tools to target other APIs and return the HttpResponse to the calling client.
        // See the read me file's section on tool calling for more details.
    }
}