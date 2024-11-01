using System.Text.Json.Serialization;

namespace AiPocWebsiteTemplateWithBackend.API.Config
{
    // Currently also using this as the Auth DTO since this whole App needs a serious rework anyway
    public class AuthRequest
    {
        [JsonPropertyName("grant_type")]
        public string GrantType { get; set; } = string.Empty;
        [JsonPropertyName("client_id")]
        public string ClientId { get; set; } = string.Empty;
        [JsonPropertyName("client_secret")]
        public string ClientSecret { get; set; } = string.Empty;
        [JsonPropertyName("audience")]
        public string Audience { get; set; } = string.Empty;
    }
}
