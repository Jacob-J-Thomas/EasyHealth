using System.Threading.Tasks;
using EasyHealth.API;
using EasyHealth.Business;
using EasyHealth.Client.IntelligenceHub;
using Moq;
using Xunit;

namespace EasyHealth.Tests.Unit.Business
{
    public class AuthLogicTests
    {
        private readonly Mock<IAIAuthClient> _mockAuthClient;
        private readonly AuthLogic _authLogic;

        public AuthLogicTests()
        {
            _mockAuthClient = new Mock<IAIAuthClient>();
            _authLogic = new AuthLogic(_mockAuthClient.Object);
        }

        [Fact]
        public async Task RetrieveAuthToken_ReturnsToken_WhenRequestIsSuccessful()
        {
            // Arrange
            var expectedToken = new AuthTokenResponse { AccessToken = "test_token" };
            _mockAuthClient.Setup(client => client.RequestAuthToken()).ReturnsAsync(expectedToken);

            // Act
            var result = await _authLogic.RetrieveAuthToken();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedToken.AccessToken, result.AccessToken);
        }

        [Fact]
        public async Task RetrieveAuthToken_ReturnsNull_WhenRequestFails()
        {
            // Arrange
            _mockAuthClient.Setup(client => client.RequestAuthToken()).ReturnsAsync((AuthTokenResponse)null);

            // Act
            var result = await _authLogic.RetrieveAuthToken();

            // Assert
            Assert.Null(result);
        }
    }
}
