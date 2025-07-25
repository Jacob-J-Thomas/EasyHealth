using EasyHealth.API;

namespace EasyHealth.Business
{
    public interface IAuthLogic
    {
        Task<AuthTokenResponse?> RetrieveAuthToken();
    }
}
