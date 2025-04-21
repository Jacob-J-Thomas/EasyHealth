
using PersonalPortfolio.API.Config;
using PersonalPortfolio.Common;

namespace PersonalPortfolio.Client.IntelligenceHub
{
    public class AIClientWrapper : AIClient, IAIClientWrapper
    {
        // This class passes the authHandler to an httpClient, allowing all requests to be wrapped in the associated
        // authentication handler. There is an HttpClient in there as well to support simple rerequests

        // Audeince is the same as the API endpoint
        public AIClientWrapper(IntelligenceHubAuthSettings hubSettings, AIAuthClient authClient) : base(hubSettings.Audience, new HttpClient(new AIAuthHandler(authClient, new HttpClientHandler())))
        {
        }
    }
}
