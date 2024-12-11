using Xunit;
using AiPocWebsiteTemplateWithBackend.Client.IntelligenceHub;
using AiPocWebsiteTemplateWithBackend.Business;
using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using static AiPocWebsiteTemplateWithBackend.API.GeneratedDTOs;

namespace AiPocWebsiteTemplateWithBackend.Tests.Integration
{
    public class PromptFlowLogicTests
    {
        private readonly Mock<IAIClientWrapper> _mockAIClientWrapper;
        private readonly PromptFlowLogic _promptFlowLogic;

        public PromptFlowLogicTests()
        {
            _mockAIClientWrapper = new Mock<IAIClientWrapper>();
            _promptFlowLogic = new PromptFlowLogic(_mockAIClientWrapper.Object);
        }
    }
}