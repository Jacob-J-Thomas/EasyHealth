using System.Text.Json.Serialization;

namespace AiPocWebsiteTemplateWithBackend.API
{
    public class AuthTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
        [JsonPropertyName("tokens_type")]
        public string TokenType { get; set; }
    }

}
