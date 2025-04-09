using ConversationalAIWebsite.API;
using ConversationalAIWebsite.DAL.Models;

namespace ConversationalAIWebsite.Business
{
    public interface IPublicApiLogic
    {
        Task<BackendResponse<int>> GetRemainingFreeRequests(string apiKey);
        Task<BackendResponse<bool>> UpsertUserData(PublicApiRequest request, string apiKey);
        Task<BackendResponse<Users?>> GetUserData(string apiKey);
    }
}
