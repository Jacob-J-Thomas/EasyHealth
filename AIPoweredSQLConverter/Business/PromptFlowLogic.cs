using AIPoweredSQLConverter.API;
using AIPoweredSQLConverter.Client.IntelligenceHub;
using AIPoweredSQLConverter.DAL;
using AIPoweredSQLConverter.DAL.Models;

namespace AIPoweredSQLConverter.Business
{
    public class PromptFlowLogic : IPromptFlowLogic
    {
        private readonly IAIClientWrapper _aiClient;
        private readonly AppDbContext _dbContext;

        private const string _sqlDataConstructionProfile = "NLSequelDataConstructionHelper";
        private const string _sqlConversionProfile = "NLSequelConverter";

        public PromptFlowLogic(IAIClientWrapper aiClient, AppDbContext context)
        {
            _aiClient = aiClient;
            _dbContext = context;
        }

        public async Task<bool> UpsertUserData(FrontEndRequest request)
        {
            var existingUser = _dbContext.UserData.FirstOrDefault(user => user.Username == request.Username);

            // Add or update existing
            if (existingUser != null)
            {
                existingUser.UserSQLData = request.SqlData;
                _dbContext.UserData.Update(existingUser);
            }
            else
            {
                var newUser = new UserData { Username = request.Username, UserSQLData = request.SqlData };
                await _dbContext.UserData.AddAsync(newUser);
            }

            // Save changes and return whether at least one record was affected.
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public string? GetSQLData(string username)
        {
            var result = _dbContext.UserData.FirstOrDefault(user => user.Username == username);
            return result?.UserSQLData;
        }

        public async Task<string?> GetSQLDataHelp(FrontEndRequest request)
        {
            // Retrieve the user's data using the primary key (Username).
            var userData = _dbContext.UserData.FirstOrDefault(user => user.Username == request.Username);
            if (userData == null) return null;

            // Build the context string using the user's SQL data.
            var completionContent = $"{request.Query}. For context, I provided my current table definitions below: \n\n{userData.UserSQLData}";

            // Construct the completion request using the specified profile and message.
            var completionRequest = new CompletionRequest
            {
                ProfileOptions = new Profile { Name = _sqlDataConstructionProfile },
                Messages = new List<Message> { new Message { Content = completionContent, Role = Role.User, TimeStamp = DateTime.UtcNow } }
            };

            var completionResponse = await _aiClient.ChatAsync(completionRequest);
            return completionResponse.Messages?.LastOrDefault()?.Content;
        }

        public async Task<string?> ConvertQueryToSQL(FrontEndRequest request)
        {
            // Retrieve the user's data using the primary key (Username).
            //var userData = _dbContext.UserData.FirstOrDefault(user => user.Username == request.Username);
            //if (userData == null || string.IsNullOrWhiteSpace(userData.UserSQLData)) return null;

            // Check if the SqlData is out of aligment and save if new
            //if (request.SqlData != userData.UserSQLData)
            //{
            //    request.SqlData = userData.UserSQLData ?? request.SqlData;

            //    var upsertResult = await UpsertUserData(request);
            //    if (!upsertResult) return null;
            //}

            // Build the context string using the user's SQL data.
            var completionContent = $"\n\nThe current time in UTC is {DateTime.UtcNow}. For context, I provided my current table definitions below:\n\n{request.SqlData}";

            // Append completion content to the last message in Messages.
            if (request.Messages.Any(x => x.Role == Role.User)) request.Messages.Last(x => x.Role == Role.User).Content += $"{completionContent}";
            else return null;

            // Construct the completion request using the conversion profile and provided messages.
            var completionRequest = new CompletionRequest
            {
                ProfileOptions = new Profile { Name = _sqlConversionProfile },
                Messages = request.Messages
            };

            var completionResponse = await _aiClient.ChatAsync(completionRequest);
            return completionResponse.Messages?.LastOrDefault()?.Content;

        }
    }
}
