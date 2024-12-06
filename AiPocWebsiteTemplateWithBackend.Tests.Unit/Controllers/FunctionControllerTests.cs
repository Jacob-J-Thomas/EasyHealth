using System.Threading.Tasks;
using AiPocWebsiteTemplateWithBackend.Business;
using AiPocWebsiteTemplateWithBackend.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace AiPocWebsiteTemplateWithBackend.Tests.Unit.Controllers
{
    public class FunctionControllerTests
    {
        private readonly Mock<IAuthLogic> _mockAuthLogic;
        private readonly Mock<IPromptFlowLogic> _mockPromptFlowLogic;
        private readonly FunctionController _functionController;

        public FunctionControllerTests()
        {
            _mockAuthLogic = new Mock<IAuthLogic>();
            _mockPromptFlowLogic = new Mock<IPromptFlowLogic>();
            _functionController = new FunctionController(_mockAuthLogic.Object, _mockPromptFlowLogic.Object);
        }

        [Fact]
        public async Task Test_ReturnsOkResult_WithValidResponse()
        {
            // Arrange
            _mockPromptFlowLogic.Setup(prompt => prompt.Test()).ReturnsAsync(true);

            // Act
            var result = await _functionController.Test();

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
            var result = await _functionController.Test();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
        }
    }
}