using AIPoweredSQLConverter.API;
using AIPoweredSQLConverter.Client.IntelligenceHub;

namespace AIPoweredSQLConverter.Business
{
    public class PromptFlowLogic : IPromptFlowLogic
    {
        private readonly IAIClientWrapper _aiClient;

        private const string _sqlDataConstructionProfile = "NLSequelDataConstructionHelper";
        private const string _sqlConversionProfile = "NLSequelConversionProfile-";
        private const string _conversionProfileSystemMessage = "Your job is to convert any natural language you receive into an SQL " +
            "command or query. If the user fails to provide a database name, you should either ask for one, or provide a placeholder. " +
            "If the request is unrelated to constructing an SQL command or query, respond with nothing but the word 'NULL'. Below is " +
            "a database construction string delimited with tripple backticks. Please use it when converting the natural language " +
            "strings to SQL.\n\n";

        public PromptFlowLogic(IAIClientWrapper aiClient) 
        {
            _aiClient = aiClient;
        }

        public async Task<bool> UpsertUserSQLProfile(FrontEndRequest request)
        {
            var profile = new Profile()
            {
                Name = _sqlConversionProfile + request.Username,
                SystemMessage = _conversionProfileSystemMessage + $"```\n{request.SqlData}\n```",
                MaxTokens = 1200,
                MaxMessageHistory = 20,
                Host = AGIServiceHosts.Azure,
                Model = "gpt-4o",
            };

            var response = await _aiClient.UpsertProfileAsync(profile);

            return true;
        }

        public async Task<string?> GetSQLDataHelp(FrontEndRequest request)
        {
            var completionContent = $"{request.Query}\n\n" +
                $"SQL Data:\n\n{request.SqlData}";

            var completion = new CompletionRequest()
            {
                ProfileOptions = new Profile() { Name = _sqlDataConstructionProfile },
                Messages = new List<Message>()
                {
                    new Message() { Content = completionContent, Role = Role.User, TimeStamp = DateTime.UtcNow }
                },
            };
            var completionResponse = await _aiClient.ChatAsync(completion);
            return completionResponse.Messages?.LastOrDefault()?.Content ?? null;
        }

        public async Task<string?> ConvertQueryToSQL(FrontEndRequest request)
        {
            var completion = new CompletionRequest()
            {
                ConversationId = request.ConversationId,
                ProfileOptions = new Profile() { Name = _sqlConversionProfile + request.Username },
                Messages = new List<Message>()
                {
                    new Message() { Content = request.Query, Role = Role.User, TimeStamp = DateTime.UtcNow }
                }
            };
            var completionResponse = await _aiClient.ChatAsync(completion);
            return completionResponse.Messages?.LastOrDefault()?.Content ?? null;
        }
    }
}
