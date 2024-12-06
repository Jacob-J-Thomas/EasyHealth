using System.Threading.Tasks;
using AiPocWebsiteTemplateWithBackend.API;
using AiPocWebsiteTemplateWithBackend.Business;
using AiPocWebsiteTemplateWithBackend.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace AiPocWebsiteTemplateWithBackend.Tests.Unit.Controllers
{
    public class ApiControllerTests
    {
        private readonly Mock<IAuthLogic> _mockAuthLogic;
        private readonly Mock<IPromptFlowLogic> _mockPromptFlowLogic;
        private readonly ApiController _apiController;

        public ApiControllerTests()
        {
            _mockAuthLogic = new Mock<IAuthLogic>();
            _mockPromptFlowLogic = new Mock<IPromptFlowLogic>();
            _apiController = new ApiController(_mockAuthLogic.Object, _mockPromptFlowLogic.Object);
        }

        [Fact]
        public async Task Authorize_ReturnsOkResult_WithValidToken()
        {
            // Arrange
            var expectedToken = new AuthTokenResponse { AccessToken = "test_token", ExpiresIn = 3600 };
            _mockAuthLogic.Setup(auth => auth.RetrieveAuthToken()).ReturnsAsync(expectedToken);

            // Act
            var result = await _apiController.Authorize();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var clientSafeResult = Assert.IsType<AuthTokenResponse>(okResult.Value);
            Assert.Equal(expectedToken.AccessToken, clientSafeResult.AccessToken);
            Assert.Equal(expectedToken.ExpiresIn, clientSafeResult.ExpiresIn);
        }

        [Fact]
        public async Task Authorize_ReturnsInternalServerError_WhenTokenIsNull()
        {
            // Arrange
            _mockAuthLogic.Setup(auth => auth.RetrieveAuthToken()).ReturnsAsync((AuthTokenResponse)null);

            // Act
            var result = await _apiController.Authorize();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task Authorize_ReturnsInternalServerError_OnException()
        {
            // Arrange
            _mockAuthLogic.Setup(auth => auth.RetrieveAuthToken()).ThrowsAsync(new System.Exception());

            // Act
            var result = await _apiController.Authorize();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task Test_ReturnsOkResult_WithValidResponse()
        {
            // Arrange
            _mockPromptFlowLogic.Setup(prompt => prompt.Test()).ReturnsAsync(true);

            // Act
            var result = await _apiController.Test();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.True((bool)okResult.Value);
        }

        [Fact]
        public async Task Test_ReturnsInternalServerError_OnException()
        {
            // Arrange
            _mockPromptFlowLogic.Setup(prompt => prompt.Test()).ThrowsAsync(new System.Exception());

            // Act
            var result = await _apiController.Test();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
        }
    }
}