using EasyHealth.Client.IntelligenceHub;

namespace EasyHealth.API
{
    public class FrontEndRequest
    {
        public required string Username { get; set; }
        public List<Message> Messages { get; set; } = new List<Message>();
    }
}
