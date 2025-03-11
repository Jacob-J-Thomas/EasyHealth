using Xunit;
using AIPoweredSQLConverter.Client.IntelligenceHub;
using AIPoweredSQLConverter.Business;
using System.Net.Http;
using System.Threading.Tasks;
using Moq;

namespace AIPoweredSQLConverter.Tests.Integration
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