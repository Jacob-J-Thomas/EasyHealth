using AIPoweredSQLConverter.API;

namespace AIPoweredSQLConverter.Business
{
    public interface IPublicApiLogic
    {
        Task<BackendResponse<int>> GetRemainingFreeRequests(string apiKey);
        Task<BackendResponse<bool>> UpsertUserSQLData(FrontEndRequest request, string apiKey);
        Task<BackendResponse<string?>> GetSQLData(string apiKey);
        Task<BackendResponse<string?>> ConvertQueryToSQL(FrontEndRequest request, string apiKey);
    }
}
