using AIPoweredSQLConverter.API;

namespace AIPoweredSQLConverter.Client.IntelligenceHub
{
    public interface IAIAuthClient
    {
        Task<AuthTokenResponse?> RequestAuthToken();
        Task<AuthTokenResponse?> RequestElevatedAuthToken();
    }
}
