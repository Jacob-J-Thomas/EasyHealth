using AIPoweredSQLConverter.API;

namespace AIPoweredSQLConverter.Business
{
    public interface IAuthLogic
    {
        Task<AuthTokenResponse?> RetrieveAuthToken();
    }
}
