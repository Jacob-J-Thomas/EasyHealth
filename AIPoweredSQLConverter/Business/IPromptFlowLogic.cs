using AIPoweredSQLConverter.API;

namespace AIPoweredSQLConverter.Business
{
    public interface IPromptFlowLogic
    {
        Task<bool> UpsertUserSQLProfile(FrontEndRequest request);
        Task<string?> GetSQLDataHelp(FrontEndRequest request);
        Task<string?> ConvertQueryToSQL(FrontEndRequest request);
    }
}
