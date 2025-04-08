using ConversationalAIWebsite.DAL.Models;

namespace ConversationalAIWebsite.Host.Auth
{
    public interface IApiKeyValidator
    {
        Task<UserData?> ValidateApiKeyAsync(string encryptedApiKey);
        string HashApiKey(string apiKey);
    }
}
