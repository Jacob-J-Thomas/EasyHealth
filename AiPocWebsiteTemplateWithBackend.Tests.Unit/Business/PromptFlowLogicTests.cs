using System.Collections.Generic;
using System.Threading.Tasks;
using AiPocWebsiteTemplateWithBackend.Business;
using AiPocWebsiteTemplateWithBackend.Client.IntelligenceHub;
using Moq;
using Xunit;
using static AiPocWebsiteTemplateWithBackend.API.GeneratedDTOs;

namespace AiPocWebsiteTemplateWithBackend.Tests.Unit.Business
{
    public class PromptFlowLogicTests
    {
        [Fact]
        public async Task Test_ReturnsTrue_WhenProfilesExist()
        {
            // Arrange
            var mockAIClient = new Mock<IAIClientWrapper>();
            mockAIClient.Setup(client => client.GetAllProfilesAsync()).ReturnsAsync(new List<Profile> { new Profile() });
            var promptFlowLogic = new PromptFlowLogic(mockAIClient.Object);

            // Act
            var result = await promptFlowLogic.Test();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Test_ReturnsFalse_WhenNoProfilesExist()
        {
            // Arrange
            var mockAIClient = new Mock<IAIClientWrapper>();
            mockAIClient.Setup(client => client.GetAllProfilesAsync()).ReturnsAsync(new List<Profile>());
            var promptFlowLogic = new PromptFlowLogic(mockAIClient.Object);

            // Act
            var result = await promptFlowLogic.Test();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task Test_ReturnsFalse_WhenProfilesIsNull()
        {
            // Arrange
            var mockAIClient = new Mock<IAIClientWrapper>();
            mockAIClient.Setup(client => client.GetAllProfilesAsync()).ReturnsAsync((ICollection<Profile>)null);
            var promptFlowLogic = new PromptFlowLogic(mockAIClient.Object);

            // Act
            var result = await promptFlowLogic.Test();

            // Assert
            Assert.False(result);
        }
    }
}