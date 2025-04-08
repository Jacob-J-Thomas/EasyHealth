using ConversationalAIWebsite.API.DTOs.Hub;

namespace ConversationalAIWebsite.API
{
    public class ChatRequest
    {
        public Guid ConversationId { get; set; }
        public ChatMessage ChatMessage { get; set; }
        public string User { get; set; }
    }
}
