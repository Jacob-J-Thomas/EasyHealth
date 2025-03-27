using AIPoweredSQLConverter.DAL.Models;

namespace AIPoweredSQLConverter.Host.Auth
{
    public interface IApiKeyValidator
    {
        Task<UserData?> ValidateApiKeyAsync(string encryptedApiKey);
        string HashApiKey(string apiKey);
    }
}
