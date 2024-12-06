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

        [Fact]
        public async Task Test_With_Invalid_Credentials_Should_Fail_Authentication()
        {
            // Arrange
            _mockAIClientWrapper.Setup(client => client.GetAllProfilesAsync())
                .ThrowsAsync(new InvalidOperationException("Failed to retrieve access token."));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await _promptFlowLogic.Test();
            });

            Assert.Equal("Failed to retrieve access token.", exception.Message);
        }

        [Fact]
        public async Task Test_With_Valid_Credentials_But_Insufficient_Permissions_Should_Fail_Authorization()
        {
            // Arrange
            _mockAIClientWrapper.Setup(client => client.GetAllProfilesAsync())
                .ReturnsAsync(new List<Profile>());

            // Act
            var result = await _promptFlowLogic.Test();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task Test_With_Valid_Credentials_And_Sufficient_Permissions_Should_Succeed()
        {
            // Arrange
            _mockAIClientWrapper.Setup(client => client.GetAllProfilesAsync())
                .ReturnsAsync(new List<Profile> { new Profile() });

            // Act
            var result = await _promptFlowLogic.Test();

            // Assert
            Assert.True(result);
        }
    }
}