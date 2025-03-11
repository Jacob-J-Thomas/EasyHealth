namespace AIPoweredSQLConverter.API.DTOs.Hub
{
    //string? profileName, Guid? conversationId, string? username, string? message)//, int? maxMessageHistory)//, string? database, string? ragTarget, int? maxRagDocs)
    public class ChatMessage
    {
        public int Role { get; set; }
        public string Content { get; set; }
        public string? Base64Image { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
    }
}
