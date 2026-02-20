using System.Text.Json.Serialization;

namespace AgroSolutionsFunctions.Contracts;

public class TokenResponse
{
    [JsonPropertyName("token")]
    public string Token { get; set; }

    public string Email { get; set; }

    public string Role { get; set; }

    public DateTime ExpiresAt { get; set; }
}
