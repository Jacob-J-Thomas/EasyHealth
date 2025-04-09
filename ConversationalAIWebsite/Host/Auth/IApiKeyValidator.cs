using ConversationalAIWebsite.DAL.Models;

namespace ConversationalAIWebsite.Host.Auth
{
    public interface IApiKeyValidator
    {
        Task<Users?> ValidateApiKeyAsync(string encryptedApiKey);
        string HashApiKey(string apiKey);
    }
}
