using ConversationalAIWebsite.Client.IntelligenceHub;

namespace ConversationalAIWebsite.API
{
    public class FrontEndRequest
    {
        public required string Username { get; set; }
        public string Query { get; set; } = string.Empty;
        public string SqlData { get; set; } = string.Empty;
        public List<Message> Messages { get; set; } = new List<Message>();
    }
}
