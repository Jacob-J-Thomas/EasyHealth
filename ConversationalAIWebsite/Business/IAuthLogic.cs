using ConversationalAIWebsite.API;

namespace ConversationalAIWebsite.Business
{
    public interface IAuthLogic
    {
        Task<AuthTokenResponse?> RetrieveAuthToken();
    }
}
