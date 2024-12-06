using AiPocWebsiteTemplateWithBackend.API;

namespace AiPocWebsiteTemplateWithBackend.Business
{
    public interface IAuthLogic
    {
        Task<AuthTokenResponse?> RetrieveAuthToken();
    }
}
