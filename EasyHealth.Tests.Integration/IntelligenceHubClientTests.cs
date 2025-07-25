using Xunit;
using EasyHealth.Client.IntelligenceHub;
using EasyHealth.Business;
using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using EasyHealth.DAL;
using EasyHealth.Host.Config;

namespace EasyHealth.Tests.Integration
{
    public class PromptFlowLogicTests
    {
        private readonly Mock<IAIClientWrapper> _mockAIClientWrapper;
        private readonly Mock<IAIAuthClient> _authClient;
        private readonly Mock<AppDbContext> _appDbContext;
        private readonly Mock<StripeSettings> _stripeSettings;
        private readonly PromptFlowLogic _promptFlowLogic;

        public PromptFlowLogicTests()
        {
            _mockAIClientWrapper = new Mock<IAIClientWrapper>();
            _promptFlowLogic = new PromptFlowLogic(_mockAIClientWrapper.Object, _authClient.Object, _appDbContext.Object, _stripeSettings.Object);
        }
    }
}