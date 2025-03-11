using AIPoweredSQLConverter.API;
using AIPoweredSQLConverter.Client.IntelligenceHub;

namespace AIPoweredSQLConverter.Business
{
    public class PromptFlowLogic : IPromptFlowLogic
    {
        private readonly IAIClientWrapper _aiClient;

        private const string _hangmanPlayerProfile = "HangmanPlayer";
        private const string _wordGenProfile = "HangmanWordGenerator";

        public PromptFlowLogic(IAIClientWrapper aiClient) 
        {
            _aiClient = aiClient;
        }

        public async Task<HangmanGameData> StartHangmanGame(string word)
        {
            var messages = new List<Message>()
            {
                new Message()
                {
                    Content = $"Please start a new game of hangman. The word you will use for this game of hangman is '{word}'" +
                              $"Remember that you should not share this word with the user.",
                    Role = Role.User
                }
            };

            var conversationId = Guid.NewGuid();
            var request = new CompletionRequest()
            {
                ConversationId = conversationId,
                Messages = messages
            };

            var attempts = 0;
            var response = new CompletionResponse();
            while ((response.Messages == null || !response.Messages.Any()) && attempts < 3)
            {
                response = await _aiClient.ChatAsync(_hangmanPlayerProfile, request);
                attempts++;
            }
            return new HangmanGameData()
            {
                ConversationId = conversationId,
                Message = response.Messages?.LastOrDefault()?.Content ?? null
            };
        }

        public async Task<string?> GenerateHangmanWord()
        {
            var messages = new List<Message>()
            {
                new Message()
                {
                    // Configure additional instructions to set difficulty, word length, etc.
                    Content = "Generate a new word.",
                    Role = Role.User
                }
            };
            var request = new CompletionRequest()
            {
                ProfileOptions = new Profile() { Name = _wordGenProfile }, // pass in 
                Messages = messages
            };

            var attempts = 0;
            var word = string.Empty;
            while ((string.IsNullOrEmpty(word) || word.Contains(" ")) && attempts < 3)
            {
                var response = await _aiClient.ChatAsync(request);
                word = response.Messages.LastOrDefault()?.Content;
                attempts++;
            }
            if (!string.IsNullOrEmpty(word) && !word.Contains(" ")) return word;
            return null;
        }
    }
}
