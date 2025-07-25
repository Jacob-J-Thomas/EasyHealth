using EasyHealth.API;
using EasyHealth.Client.IntelligenceHub;
using EasyHealth.DAL.Models;

namespace EasyHealth.Business
{
    public interface IPromptFlowLogic
    {
        Task<BackendResponse<Profile?>> GetProfile(string test);
        Task<BackendResponse<Users?>> GetUserData(string username);
        Task<BackendResponse<bool>> MarkUserAsPaying(string username, string customerId);
        Task<BackendResponse<bool>> MarkUserAsNonPaying(string username);
        Task<BackendResponse<bool>> SaveNewUser(string username);
        Task<BackendResponse<bool>> UpsertUserData(PublicApiRequest request, string apiKey);
        Task<BackendResponse<string>> GenerateAndStoreApiKey(string username);
        Task <BackendResponse<Profile?>> UpsertProfile(Profile profile);
        Task <BackendResponse<string>> GetIntelligenceHubBearerKey();
    }
}

