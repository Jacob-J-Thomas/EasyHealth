
namespace ConversationalAIWebsite.Client.IntelligenceHub
{
    public interface IAIClientWrapper
    {
        Task<Profile> GetProfileAsync(string profile);
        Task<ICollection<Profile>> GetAllProfilesAsync(int page, int count);
        Task<Profile> UpsertProfileAsync(Profile body);
        Task<CompletionResponse> ChatAsync(string name, CompletionRequest request);
        Task<CompletionResponse> ChatAsync(CompletionRequest request);
    }
}
