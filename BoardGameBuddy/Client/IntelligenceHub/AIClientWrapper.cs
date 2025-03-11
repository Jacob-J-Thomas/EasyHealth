
using BoardGameBuddy.API.Config;

namespace BoardGameBuddy.Client.IntelligenceHub
{
    public class AIClientWrapper : AIClient, IAIClientWrapper
    {
        // This class passes the authHandler to an httpClient, allowing all requests to be wrapped in the associated
        // authentication handler. There is an HttpClient in there as well to support simple rerequests

        public AIClientWrapper(AIHubSettings hubSettings, AIAuthClient authClient) : base(hubSettings.Endpoint, new HttpClient(new AIAuthHandler(authClient, new HttpClientHandler())))
        {
        }
    }
}
