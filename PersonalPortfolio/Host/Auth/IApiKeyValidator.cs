using PersonalPortfolio.DAL.Models;

namespace PersonalPortfolio.Host.Auth
{
    public interface IApiKeyValidator
    {
        Task<Users?> ValidateApiKeyAsync(string encryptedApiKey);
        string HashApiKey(string apiKey);
    }
}
