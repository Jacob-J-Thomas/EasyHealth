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
        public async Task GetHangmanWord_ReturnsOk_WhenWordIsValid()
        {
            // Arrange
            var validWord = "hangman";
            _mockPromptFlowLogic.Setup(x => x.GenerateHangmanWord())
                .ReturnsAsync(validWord);

            // Act
            var result = await _apiController.GetHangmanWord();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(validWord, okResult.Value);
        }

        [Fact]
        public async Task GetHangmanWord_Returns500_WhenWordIsEmpty()
        {
            // Arrange
            _mockPromptFlowLogic.Setup(x => x.GenerateHangmanWord())
                .ReturnsAsync(string.Empty);

            // Act
            var result = await _apiController.GetHangmanWord();

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            Assert.Equal("Something went wrong when generating a word", objectResult.Value);
        }

        [Fact]
        public async Task GetHangmanWord_Returns500_WhenWordContainsSpaces()
        {
            // Arrange
            var invalidWord = "invalid word";
            _mockPromptFlowLogic.Setup(x => x.GenerateHangmanWord())
                .ReturnsAsync(invalidWord);

            // Act
            var result = await _apiController.GetHangmanWord();

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            Assert.Equal("Something went wrong when generating a word", objectResult.Value);
        }

        [Fact]
        public async Task GetHangmanWord_Returns500_OnException()
        {
            // Arrange
            _mockPromptFlowLogic.Setup(x => x.GenerateHangmanWord())
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _apiController.GetHangmanWord();

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            Assert.Equal("Something went wrong", objectResult.Value);
        }

        [Fact]
        public async Task StartHangman_ReturnsOk_WhenResponseIsValid()
        {
            // Arrange
            var word = "hangman";
            var validResponse = new HangmanGameData { Message = "Game started!" };
            _mockPromptFlowLogic.Setup(x => x.StartHangmanGame(word))
                .ReturnsAsync(validResponse);

            // Act
            var result = await _apiController.StartHangman(word);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(validResponse, okResult.Value);
        }

        [Fact]
        public async Task StartHangman_Returns500_WhenResponseMessageIsEmpty()
        {
            // Arrange
            var word = "hangman";
            var invalidResponse = new HangmanGameData { Message = string.Empty };
            _mockPromptFlowLogic.Setup(x => x.StartHangmanGame(word))
                .ReturnsAsync(invalidResponse);

            // Act
            var result = await _apiController.StartHangman(word);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            Assert.Equal("Something went wrong when starting the game", objectResult.Value);
        }

        [Fact]
        public async Task StartHangman_Returns500_OnException()
        {
            // Arrange
            var word = "hangman";
            _mockPromptFlowLogic.Setup(x => x.StartHangmanGame(word))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _apiController.StartHangman(word);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            Assert.Equal("Something went wrong", objectResult.Value);
        }
    }
}