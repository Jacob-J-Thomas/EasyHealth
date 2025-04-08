using System.Data;

namespace ConversationalAIWebsite.API
{
    public class ChatResponseChunk
    {
        public int Id { get; set; }
        public string CompletionUpdate { get; set; } = string.Empty;
        public string? Base64Image { get; set; }
        public string? Role { get; set; }
        public string? FinishReason { get; set; }
        public Dictionary<string, string> ToolCalls { get; set; } = new Dictionary<string, string>();
        public List<HttpResponseMessage> ToolExecutionResponses { get; set; } = new List<HttpResponseMessage>();
    }
}
