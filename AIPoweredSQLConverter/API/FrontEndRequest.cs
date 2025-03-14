namespace AIPoweredSQLConverter.API
{
    public class FrontEndRequest
    {
        public required string Username { get; set; }
        public Guid? ConversationId { get; set; } = null;
        public string Query { get; set; } = string.Empty;
        public string SqlData { get; set; } = string.Empty;
    }
}
