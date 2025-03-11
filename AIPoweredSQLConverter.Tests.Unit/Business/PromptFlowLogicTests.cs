using System.Collections.Generic;
using System.Threading.Tasks;
using AIPoweredSQLConverter.Business;
using AIPoweredSQLConverter.Client.IntelligenceHub;
using Moq;
using Xunit;

namespace AIPoweredSQLConverter.Tests.Unit.Business
{
    public class PromptFlowLogicTests
    {
        private readonly Mock<IAIClientWrapper> _mockAiClient;
        private readonly PromptFlowLogic _promptFlowLogic;

        public PromptFlowLogicTests()
        {
            _mockAiClient = new Mock<IAIClientWrapper>();
            _promptFlowLogic = new PromptFlowLogic(_mockAiClient.Object);
        }

        [Fact]
        public async Task StartHangmanGame_ReturnsValidGameData_WhenResponseIsValid()
        {
            // Arrange
            var word = "hangman";
            var responseContent = $"Please start a new game of hangman. The word you will use for this game of hangman is '{word}'" +
                                  $"Remember that you should not share this word with the user.";
            var aiResponse = new CompletionResponse
            {
                Messages = new List<Message> 
                { new Message
                    {
                        Content = responseContent,
                        Role = Role.User
                    }
                }
            };

            _mockAiClient.Setup(x => x.ChatAsync(It.IsAny<string>(), It.IsAny<CompletionRequest>()))
                .ReturnsAsync(aiResponse);

            // Act
            var result = await _promptFlowLogic.StartHangmanGame(word);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<Guid>(result.ConversationId); // Generated ID should match
            Assert.Equal(responseContent, result.Message);
        }

        [Fact]
        public async Task StartHangmanGame_ReturnsNullMessage_AfterMaxRetries()
        {
            // Arrange
            var word = "hangman";
            var emptyResponse = new CompletionResponse { Messages = null };

            _mockAiClient.Setup(x => x.ChatAsync(It.IsAny<string>(), It.IsAny<CompletionRequest>()))
                .ReturnsAsync(emptyResponse);

            // Act
            var result = await _promptFlowLogic.StartHangmanGame(word);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.Message);
        }

        [Fact]
        public async Task StartHangmanGame_RetriesUpToMaxAttempts()
        {
            // Arrange
            var word = "hangman";
            var emptyResponse = new CompletionResponse { Messages = null };

            _mockAiClient.Setup(x => x.ChatAsync(It.IsAny<string>(), It.IsAny<CompletionRequest>()))
                .ReturnsAsync(emptyResponse);

            // Act
            await _promptFlowLogic.StartHangmanGame(word);

            // Assert
            _mockAiClient.Verify(x => x.ChatAsync(It.IsAny<string>(), It.IsAny<CompletionRequest>()), Times.Exactly(3));
        }

        [Fact]
        public async Task GenerateHangmanWord_ReturnsValidWord_WhenResponseIsValid()
        {
            // Arrange
            var validWord = "hangman";
            var aiResponse = new CompletionResponse
            {
                Messages = new List<Message>
                {
                    new Message { Content = validWord }
                }
            };

            _mockAiClient.Setup(x => x.ChatAsync(It.IsAny<CompletionRequest>()))
                .ReturnsAsync(aiResponse);

            // Act
            var result = await _promptFlowLogic.GenerateHangmanWord();

            // Assert
            Assert.Equal(validWord, result);
        }

        [Fact]
        public async Task GenerateHangmanWord_RetriesOnInvalidWord()
        {
            // Arrange
            var invalidWord = "invalid word";
            var validWord = "hangman";
            var responses = new Queue<CompletionResponse>(new[]
            {
                new CompletionResponse
                {
                    Messages = new List<Message> { new Message { Content = invalidWord } }
                },
                new CompletionResponse
                {
                    Messages = new List<Message> { new Message { Content = validWord } }
                }
            });

            _mockAiClient.Setup(x => x.ChatAsync(It.IsAny<CompletionRequest>()))
                .ReturnsAsync(() => responses.Dequeue());

            // Act
            var result = await _promptFlowLogic.GenerateHangmanWord();

            // Assert
            Assert.Equal(validWord, result);
            _mockAiClient.Verify(x => x.ChatAsync(It.IsAny<CompletionRequest>()), Times.Exactly(2));
        }

        [Fact]
        public async Task GenerateHangmanWord_ReturnsNull_AfterMaxRetries()
        {
            // Arrange
            var invalidResponse = new CompletionResponse
            {
                Messages = new List<Message>
                {
                    new Message { Content = "invalid word" }
                }
            };

            _mockAiClient.Setup(x => x.ChatAsync(It.IsAny<CompletionRequest>()))
                .ReturnsAsync(invalidResponse);

            // Act
            var result = await _promptFlowLogic.GenerateHangmanWord();

            // Assert
            Assert.Null(result);
        }
    }
}