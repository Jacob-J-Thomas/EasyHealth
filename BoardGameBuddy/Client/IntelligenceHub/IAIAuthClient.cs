using BoardGameBuddy.API;

namespace BoardGameBuddy.Client.IntelligenceHub
{
    public interface IAIAuthClient
    {
        Task<AuthTokenResponse?> RequestAuthToken();
        Task<AuthTokenResponse?> RequestElevatedAuthToken();
    }
}
