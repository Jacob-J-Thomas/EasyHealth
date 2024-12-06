

using static AiPocWebsiteTemplateWithBackend.API.GeneratedDTOs;

namespace AiPocWebsiteTemplateWithBackend.Client.IntelligenceHub
{
    public interface IAIClientWrapper
    {
        Task<ICollection<Profile>> GetAllProfilesAsync();
    }
}
