namespace AiPocWebsiteTemplateWithBackend.API
{
    public class HangmanGameData
    {
        public Guid ConversationId { get; set; }
        public string? Message { get; set; }
        public bool Increment { get; set; }
    }
}
