
using AiPocTemplateWithBackend.Client.IntelligenceHub;

namespace AiPocWebsiteTemplateWithBackend.Client.IntelligenceHub
{
    public class AIClientWrapper : AIClient
    {
        // This class passes the authHandler to an httpClient, allowing all requests to be wrapped in the associated
        // authentication handler. There is an HttpClient in there as well to support simple rerequests

        public AIClientWrapper(string baseUrl, AIAuthClient authClient) : base(baseUrl, new HttpClient(new AIAuthHandler(authClient, new HttpClientHandler())))
        {
        }
    }
}
