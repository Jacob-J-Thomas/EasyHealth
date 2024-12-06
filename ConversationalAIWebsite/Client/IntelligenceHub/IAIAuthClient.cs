using AiPocWebsiteTemplateWithBackend.API;

namespace AiPocWebsiteTemplateWithBackend.Client.IntelligenceHub
{
    public interface IAIAuthClient
    {
        Task<AuthTokenResponse?> RequestAuthToken();
        Task<AuthTokenResponse?> RequestElevatedAuthToken();
    }
}
