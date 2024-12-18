

using static BoardGameBuddy.API.GeneratedDTOs;

namespace BoardGameBuddy.Client.IntelligenceHub
{
    public interface IAIClientWrapper
    {
        Task<ICollection<Profile>> GetAllProfilesAsync();
        Task<CompletionResponse> ChatAsync(string name, CompletionRequest request);
    }
}
