using ConversationalAIWebsite.Client.IntelligenceHub;

namespace ConversationalAIWebsite.API
{
    public class PublicApiRequest
    {
        public required string Username { get; set; }
        public List<Message> Messages { get; set; } = new List<Message>();
    }
}
