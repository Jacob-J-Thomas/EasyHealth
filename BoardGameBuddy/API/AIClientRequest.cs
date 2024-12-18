using BoardGameBuddy.API;

namespace BoardGameBuddy.API.DTOs.Hub
{
    //string? profileName, Guid? conversationId, string? username, string? message)//, int? maxMessageHistory)//, string? database, string? ragTarget, int? maxRagDocs)
    public class AIClientRequest
    {
        public Guid? ConversationId { get; set; }
        public ChatProfile ProfileOptions { get; set; } = new ChatProfile();
        public List<ChatMessage> Messages { get; set; }
    }
}
