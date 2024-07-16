namespace OpenAICustomFunctionCallingAPI.API.DTOs.Hub
{
    //string? profileName, Guid? conversationId, string? username, string? message)//, int? maxMessageHistory)//, string? database, string? ragTarget, int? maxRagDocs)
    public class ChatRequest
    {
        public Guid? ConversationId { get; set; }
        public string Username { get; set; }
        public string Message { get; set; }
    }
}
