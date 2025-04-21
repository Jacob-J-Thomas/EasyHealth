using PersonalPortfolio.API;

namespace PersonalPortfolio.Client.IntelligenceHub
{
    public interface IAIAuthClient
    {
        Task<AuthTokenResponse?> RequestAuthToken();
        Task<AuthTokenResponse?> RequestElevatedAuthToken();
    }
}
