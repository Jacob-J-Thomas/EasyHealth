using PersonalPortfolio.API;
using PersonalPortfolio.Client.IntelligenceHub;
using PersonalPortfolio.DAL;
using PersonalPortfolio.DAL.Models;
using PersonalPortfolio.Host.Auth;
using PersonalPortfolio.Host.Config;
using Microsoft.EntityFrameworkCore;
using Stripe;
using System.Security.Cryptography;
using System.Text;

namespace PersonalPortfolio.Business
{
    public class PublicApiLogic : IPublicApiLogic
    {
        private readonly IAIClientWrapper _aiClient;
        private readonly AppDbContext _dbContext;

        private const int _maxRequestsPerDay = 20;

        private readonly string _meterName;
        private readonly string _stripeKey;

        public PublicApiLogic(
            IAIClientWrapper aiClient,
            AppDbContext context,
            StripeSettings settings)
        {
            _aiClient = aiClient;
            _dbContext = context;
            _meterName = settings.MeterName;
            _stripeKey = settings.SecretKey;
        }

        public async Task<BackendResponse<int>> GetRemainingFreeRequests(string apiKey)
        {
            var encryptedApiKey = HashApiKey(apiKey);
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.EncryptedApiKey == encryptedApiKey);
            if (user == null)
            {
                return BackendResponse<int>.CreateFailureResponse("Invalid API key.", System.Net.HttpStatusCode.Unauthorized);
            }
            return BackendResponse<int>.CreateSuccessResponse(_maxRequestsPerDay - user.RequestCount);
        }

        public async Task<BackendResponse<bool>> UpsertUserData(PublicApiRequest request, string apiKey)
        {
            try
            {
                var encryptedApiKey = HashApiKey(apiKey);
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.EncryptedApiKey == encryptedApiKey);
                if (user == null || user.ApiKeyGeneratedDate < DateTime.UtcNow.AddMonths(-6))
                {
                    return BackendResponse<bool>.CreateFailureResponse("Invalid or expired API key.");
                }

                var lastMessage = request.Messages.LastOrDefault();
                user.ExampleDataField = $"Latest message = {lastMessage?.User}: {lastMessage?.Content}";

                _dbContext.Users.Update(user);
                var changes = await _dbContext.SaveChangesAsync();
                return changes > 0
                    ? BackendResponse<bool>.CreateSuccessResponse(true, "User data updated successfully.")
                    : BackendResponse<bool>.CreateFailureResponse("No changes were made to the database.");
            }
            catch (Exception)
            {
                return BackendResponse<bool>.CreateFailureResponse("An error occurred when updating user data.");
            }
        }

        public async Task<BackendResponse<Users?>> GetUserData(string apiKey)
        {
            try
            {
                var encryptedApiKey = HashApiKey(apiKey);
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.EncryptedApiKey == encryptedApiKey);
                if (user == null || user.ApiKeyGeneratedDate < DateTime.UtcNow.AddMonths(-6))
                {
                    return BackendResponse<Users?>.CreateFailureResponse("Invalid API key.", System.Net.HttpStatusCode.Unauthorized);
                }

                return user != null
                    ? BackendResponse<Users?>.CreateSuccessResponse(user)
                    : BackendResponse<Users?>.CreateFailureResponse("No SQL data found for the given user.", System.Net.HttpStatusCode.NotFound);
            }
            catch (Exception)
            {
                return BackendResponse<Users?>.CreateFailureResponse("An error occurred when retrieving SQL data.");
            }
        }

        private string HashApiKey(string apiKey)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(apiKey));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private void RecordUsageWithStripe(string customerId, long quantity)
        {
            StripeConfiguration.ApiKey = _stripeKey;
            var options = new Stripe.Billing.MeterEventCreateOptions
            {
                EventName = _meterName,
                Payload = new Dictionary<string, string>
                {
                    { "value", $"{quantity}" },
                    { "stripe_customer_id", customerId },
                },
            };
            var service = new Stripe.Billing.MeterEventService();
            service.Create(options);
        }
    }
}
