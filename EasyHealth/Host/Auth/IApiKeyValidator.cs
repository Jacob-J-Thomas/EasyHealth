using EasyHealth.DAL.Models;

namespace EasyHealth.Host.Auth
{
    public interface IApiKeyValidator
    {
        Task<Users?> ValidateApiKeyAsync(string encryptedApiKey);
        string HashApiKey(string apiKey);
    }
}
