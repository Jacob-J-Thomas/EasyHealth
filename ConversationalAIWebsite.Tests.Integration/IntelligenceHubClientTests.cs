using Xunit;
using ConversationalAIWebsite.Client.IntelligenceHub;
using ConversationalAIWebsite.Business;
using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using ConversationalAIWebsite.DAL;
using ConversationalAIWebsite.Host.Config;

namespace ConversationalAIWebsite.Tests.Integration
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