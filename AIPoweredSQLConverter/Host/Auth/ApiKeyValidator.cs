using AIPoweredSQLConverter.DAL.Models;
using AIPoweredSQLConverter.DAL;
using AIPoweredSQLConverter.Host.Config;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace AIPoweredSQLConverter.Host.Auth
{
    public class ApiKeyValidator : IApiKeyValidator
    {
        private readonly AppDbContext _dbContext;
        private readonly string _encryptionKey;

        public ApiKeyValidator(AppDbContext dbContext, StripeSettings stripeSettings)
        {
            _dbContext = dbContext;
            _encryptionKey = stripeSettings.ApiEncryptionKey;
        }

        public async Task<UserData?> ValidateApiKeyAsync(string encryptedApiKey)
        {
            var user = await _dbContext.UserData.FirstOrDefaultAsync(u => u.EncryptedApiKey == encryptedApiKey);
            if (user == null)
                return null;

            if (user.ApiKeyGeneratedDate.HasValue &&
                (DateTime.UtcNow - user.ApiKeyGeneratedDate.Value).TotalDays > 180)
            {
                return null;
            }
            return user;
        }

        public string HashApiKey(string apiKey)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(apiKey));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
