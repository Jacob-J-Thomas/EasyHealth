using System.Text.Json.Serialization;

namespace AIPoweredSQLConverter.API.Config
{
    // Currently also using this as the Auth DTO since this whole App needs a serious rework anyway
    public class IntelligenceHubAuthSettings
    {
        [JsonIgnore]
        public string Endpoint { get; set; } = string.Empty;
        public string AdminClientId { get; set; } = string.Empty;
        public string AdminClientSecret { get; set; } = string.Empty;
        public string DefaultClientId { get; set; } = string.Empty;
        public string DefaultClientSecret { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
    }
}
