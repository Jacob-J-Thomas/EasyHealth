
namespace AIPoweredSQLConverter.Client.IntelligenceHub
{
    public interface IAIClientWrapper
    {
        Task<ICollection<Profile>> GetAllProfilesAsync(int page, int count);
        Task<Profile> UpsertProfileAsync(Profile body);
        Task<CompletionResponse> ChatAsync(string name, CompletionRequest request);
        Task<CompletionResponse> ChatAsync(CompletionRequest request);
    }
}
