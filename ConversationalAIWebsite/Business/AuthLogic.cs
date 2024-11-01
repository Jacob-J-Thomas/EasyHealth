using AiPocWebsiteTemplateWithBackend.API;
using AiPocWebsiteTemplateWithBackend.API.Config;
using AiPocWebsiteTemplateWithBackend.Client.IntelligenceHub;

namespace AiPocWebsiteTemplateWithBackend.Business
{
    public class AuthLogic
    {
        private readonly AIAuthClient _authClient;

        public AuthLogic(AuthSettings settings, IHttpClientFactory factory) 
        {
            _authClient = new AIAuthClient(settings, factory);
        }

        public async Task<AuthTokenResponse?> RetrieveAuthToken()
        {
            var tokeResult = await _authClient.RequestAuthToken();
            return tokeResult;
        }
    }
}
