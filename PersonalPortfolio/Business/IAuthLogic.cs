using PersonalPortfolio.API;

namespace PersonalPortfolio.Business
{
    public interface IAuthLogic
    {
        Task<AuthTokenResponse?> RetrieveAuthToken();
    }
}
