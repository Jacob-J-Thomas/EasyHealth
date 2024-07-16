
using ConversationalAIWebsite.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ConversationalAIWebsite.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmptyController : ControllerBase
    {
        private readonly ILogger<EmptyController> _logger;

        public EmptyController(ILogger<EmptyController> logger)
        {
            _logger = logger;
        }

        // add methods if desired...
    }
}