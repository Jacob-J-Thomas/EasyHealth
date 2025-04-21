using PersonalPortfolio.API;
using PersonalPortfolio.Client.IntelligenceHub;

namespace PersonalPortfolio.Business
{
    public class AuthLogic : IAuthLogic
    {
        private readonly IAIAuthClient _authClient;

        public AuthLogic(IAIAuthClient authClient) 
        {
            _authClient = authClient;
        }

        public async Task<AuthTokenResponse?> RetrieveAuthToken()
        {
            var tokeResult = await _authClient.RequestAuthToken();
            return tokeResult;
        }
    }
}
