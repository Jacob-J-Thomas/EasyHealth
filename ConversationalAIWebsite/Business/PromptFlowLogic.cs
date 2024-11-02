using AiPocWebsiteTemplateWithBackend.API.Config;
using AiPocWebsiteTemplateWithBackend.Client.IntelligenceHub;

namespace AiPocWebsiteTemplateWithBackend.Business
{
    public class PromptFlowLogic
    {
        private readonly AIClientWrapper _aiClient;

        public PromptFlowLogic(IntelligenceHubAuthSettings authSettings, AIHubSettings aiSettings, IHttpClientFactory factory) 
        {
            var authClient = new AIAuthClient(authSettings, factory);
            _aiClient = new AIClientWrapper(aiSettings.Endpoint, authClient);
        }

        // Methods can be added here to set up prompt flows executed by function calls (passed
        // through the FunctionController) or via client requests to the ApiController. Of these
        // two, utilizing the function controller is more secure, athlough currently only basic
        // auth is supported

        public async Task<bool> Test()
        {
            var indexes = await _aiClient.GetAllProfilesAsync();

            if (indexes.Count > 0) return true;
            return false;
        }
    }
}
