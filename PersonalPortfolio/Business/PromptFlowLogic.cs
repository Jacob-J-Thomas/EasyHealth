using PersonalPortfolio.API;
using PersonalPortfolio.Client.IntelligenceHub;
using PersonalPortfolio.DAL;
using PersonalPortfolio.DAL.Models;
using PersonalPortfolio.Host.Config;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Stripe;
using System.Security.Cryptography;
using System.Text;

namespace PersonalPortfolio.Business
{
    public class PromptFlowLogic : IPromptFlowLogic
    {
        private readonly IAIAuthClient _authClient;
        private readonly IAIClientWrapper _aiClient;
        private readonly AppDbContext _dbContext;

        private readonly string _meterName;
        private readonly string _stripeKey;

        public PromptFlowLogic(IAIClientWrapper aiClient, IAIAuthClient authClient, AppDbContext context, StripeSettings settings)
        {
            _authClient = authClient;
            _aiClient = aiClient;
            _dbContext = context;

            _meterName = settings.MeterName;
            _stripeKey = settings.SecretKey;
        }

        public async Task<BackendResponse<Profile?>> GetProfile(string profile)
        {
            try
            {
                var profileData = await _aiClient.GetProfileAsync(profile);
                if (profileData != null) return BackendResponse<Profile?>.CreateSuccessResponse(profileData);
                return BackendResponse<Profile?>.CreateFailureResponse("Failed to retrieve profile", System.Net.HttpStatusCode.NotFound);
            }
            catch (Exception)
            {
                return BackendResponse<Profile?>.CreateFailureResponse($"Error retrieving the profile.");
            }
        }

        public async Task<BackendResponse<Profile?>> UpsertProfile(Profile profile)
        {
            try
            {
                var profileData = await _aiClient.UpsertProfileAsync(profile);
                if (profileData != null) return BackendResponse<Profile?>.CreateSuccessResponse(profileData);
                return BackendResponse<Profile?>.CreateFailureResponse("Error updating the profile.");
            }
            catch (Exception)
            {
                return BackendResponse<Profile?>.CreateFailureResponse($"Error updating the profile.");
            }
        }

        public async Task<BackendResponse<Users?>> GetUserData(string username)
        {
            try
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
                if (user == null)
                {
                    return BackendResponse<Users?>.CreateFailureResponse("User not found.", System.Net.HttpStatusCode.NotFound);
                }
                return BackendResponse<Users?>.CreateSuccessResponse(user, "User profile retrieved successfully.");
            }
            catch (Exception)
            {
                return BackendResponse<Users?>.CreateFailureResponse($"Error retrieving user.");
            }
        }

        public async Task<BackendResponse<bool>> MarkUserAsPaying(string username, string customerId)
        {
            try
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
                if (user == null) return BackendResponse<bool>.CreateFailureResponse("User not found.");
                user.IsPayingCustomer = true;
                user.StripeCustomerId = customerId;
                _dbContext.Users.Update(user);
                await _dbContext.SaveChangesAsync();
                return BackendResponse<bool>.CreateSuccessResponse(true, "User updated to paying status.");
            }
            catch (Exception)
            {
                return BackendResponse<bool>.CreateFailureResponse($"Error updating user.");
            }
        }

        public async Task<BackendResponse<bool>> MarkUserAsNonPaying(string customerId)
        {
            try
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.StripeCustomerId == customerId);
                if (user == null) return BackendResponse<bool>.CreateFailureResponse("User not found.");
                user.IsPayingCustomer = false;
                _dbContext.Users.Update(user);
                await _dbContext.SaveChangesAsync();
                return BackendResponse<bool>.CreateSuccessResponse(true, "User updated to non-paying status.");
            }
            catch (Exception)
            {
                return BackendResponse<bool>.CreateFailureResponse($"Error updating user.");
            }
        }

        public async Task<BackendResponse<bool>> SaveNewUser(string sub)
        {
            try
            {
                var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == sub);
                if (existingUser != null)
                {
                    return BackendResponse<bool>.CreateSuccessResponse(true);
                }

                var newUser = new Users
                {
                    Username = sub,
                    IsPayingCustomer = false,
                    RequestCount = 0,
                    LastRequestDate = null,
                    RowVersion = new byte[0]
                };

                await _dbContext.Users.AddAsync(newUser);
                await _dbContext.SaveChangesAsync();

                return BackendResponse<bool>.CreateSuccessResponse(true, "User saved successfully.");
            }
            catch (Exception)
            {
                return BackendResponse<bool>.CreateFailureResponse($"Error saving user.");
            }
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

        public async Task<BackendResponse<string>> GenerateAndStoreApiKey(string username)
        {
            try
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
                if (user == null)
                {
                    return BackendResponse<string>.CreateFailureResponse("User not found.", System.Net.HttpStatusCode.NotFound);
                }

                // Generate a new API key
                var apiKey = GenerateApiKey();

                // Encrypt the API key
                var encryptedApiKey = HashApiKey(apiKey);

                // Update user profile
                user.EncryptedApiKey = encryptedApiKey;
                user.ApiKeyGeneratedDate = DateTime.UtcNow;

                _dbContext.Users.Update(user);
                await _dbContext.SaveChangesAsync();

                return BackendResponse<string>.CreateSuccessResponse(apiKey, "API key generated and stored successfully.");
            }
            catch (Exception)
            {
                return BackendResponse<string>.CreateFailureResponse($"Error generating API key.");
            }
        }

        private string GenerateApiKey()
        {
            var apiKeyBytes = RandomNumberGenerator.GetBytes(32);
            return Convert.ToBase64String(apiKeyBytes);
        }

        private string HashApiKey(string apiKey)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(apiKey));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        public async Task<BackendResponse<string>> GetIntelligenceHubBearerKey(string username)
        {
            try
            {
                var response = await _authClient.RequestAuthToken();
                if (!string.IsNullOrEmpty(response.AccessToken)) return BackendResponse<string>.CreateSuccessResponse(response.AccessToken);
                return BackendResponse<string>.CreateFailureResponse("Failed to retrieve auth token");
            }
            catch (Exception)
            {
                return BackendResponse<string>.CreateFailureResponse("Something went wrong when getting the key.", statusCode: System.Net.HttpStatusCode.InternalServerError);
            }
        }
    }
}
