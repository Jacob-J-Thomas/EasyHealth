using Xunit;
using AIPoweredSQLConverter.Client.IntelligenceHub;
using AIPoweredSQLConverter.Business;
using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using AIPoweredSQLConverter.DAL;
using AIPoweredSQLConverter.Host.Config;

namespace AIPoweredSQLConverter.Tests.Integration
{
    public class PromptFlowLogicTests
    {
        private readonly Mock<IAIClientWrapper> _mockAIClientWrapper;
        private readonly Mock<AppDbContext> _appDbContext;
        private readonly Mock<StripeSettings> _stripeSettings;
        private readonly PromptFlowLogic _promptFlowLogic;

        public PromptFlowLogicTests()
        {
            _mockAIClientWrapper = new Mock<IAIClientWrapper>();
            _promptFlowLogic = new PromptFlowLogic(_mockAIClientWrapper.Object, _appDbContext.Object, _stripeSettings.Object);
        }
    }
}