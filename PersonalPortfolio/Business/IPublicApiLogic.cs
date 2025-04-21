using PersonalPortfolio.API;
using PersonalPortfolio.DAL.Models;

namespace PersonalPortfolio.Business
{
    public interface IPublicApiLogic
    {
        Task<BackendResponse<int>> GetRemainingFreeRequests(string apiKey);
        Task<BackendResponse<bool>> UpsertUserData(PublicApiRequest request, string apiKey);
        Task<BackendResponse<Users?>> GetUserData(string apiKey);
    }
}
