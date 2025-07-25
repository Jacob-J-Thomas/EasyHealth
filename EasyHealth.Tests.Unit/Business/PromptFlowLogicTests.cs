using System.Collections.Generic;
using System.Threading.Tasks;
using EasyHealth.Business;
using EasyHealth.Client.IntelligenceHub;
using EasyHealth.DAL;
using EasyHealth.Host.Config;
using Moq;
using Xunit;

namespace EasyHealth.Tests.Unit.Business
{
    public class PromptFlowLogicTests
    {
        private readonly Mock<IAIClientWrapper> _mockAiClient;
        private readonly Mock<IAIAuthClient> _authClient;
        private readonly Mock<AppDbContext> _appDbContext;
        private readonly Mock<StripeSettings> _stripeSettings;
        private readonly PromptFlowLogic _promptFlowLogic;

        public PromptFlowLogicTests()
        {
            _mockAiClient = new Mock<IAIClientWrapper>();
            _promptFlowLogic = new PromptFlowLogic(_mockAiClient.Object, _authClient.Object, _appDbContext.Object, _stripeSettings.Object);
        }
    }
}