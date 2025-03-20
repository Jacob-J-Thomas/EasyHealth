using AIPoweredSQLConverter.API;

namespace AIPoweredSQLConverter.Business
{
    public interface IPromptFlowLogic
    {
        Task<bool> UpsertUserData(FrontEndRequest request);
        Task<string?> GetSQLDataHelp(FrontEndRequest request);
        string? GetSQLData(string username);
        Task<string?> ConvertQueryToSQL(FrontEndRequest request);
    }
}
