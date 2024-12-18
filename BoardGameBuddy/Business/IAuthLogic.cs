using BoardGameBuddy.API;

namespace BoardGameBuddy.Business
{
    public interface IAuthLogic
    {
        Task<AuthTokenResponse?> RetrieveAuthToken();
    }
}
