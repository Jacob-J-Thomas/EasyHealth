using ConversationalAIWebsite.API;

namespace ConversationalAIWebsite.Client.IntelligenceHub
{
    public interface IAIAuthClient
    {
        Task<AuthTokenResponse?> RequestAuthToken();
        Task<AuthTokenResponse?> RequestElevatedAuthToken();
    }
}
