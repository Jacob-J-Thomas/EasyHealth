using AiPocWebsiteTemplateWithBackend.API.Config;
using AiPocWebsiteTemplateWithBackend.Client.IntelligenceHub;

namespace AiPocWebsiteTemplateWithBackend.Business
{
    public class PromptFlowLogic : IPromptFlowLogic
    {
        private readonly IAIClientWrapper _aiClient;

        public PromptFlowLogic(IAIClientWrapper aiClient) 
        {
            _aiClient = aiClient;
        }

        // Methods can be added here to set up prompt flows executed by function calls (passed
        // through the FunctionController) or via client requests to the ApiController. Of these
        // two, utilizing the function controller is more secure, athlough currently only basic
        // auth is supported

        public async Task<bool> Test()
        {
            var indexes = await _aiClient.GetAllProfilesAsync();
            if (indexes == null || indexes.Count < 1) return false;
            return true;
        }
    }
}
