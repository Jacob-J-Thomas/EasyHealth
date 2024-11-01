using OpenAICustomFunctionCallingAPI.API.DTOs.Hub;

namespace AiPocWebsiteTemplateWithBackend.API
{
    public class ChatRequest
    {
        public Guid ConversationId { get; set; }
        public ChatMessage ChatMessage { get; set; }
        public string User { get; set; }
    }
}
