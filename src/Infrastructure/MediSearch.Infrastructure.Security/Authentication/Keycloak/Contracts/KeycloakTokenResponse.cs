using System.Text.Json.Serialization;

namespace MediSearch.Infrastructure.Security.Authentication.Keycloak.Contracts;

/// <summary>
/// Successful response of the OpenID Connect token endpoint. The fields are snake_case
/// in the protocol, so they are mapped explicitly.
/// </summary>
internal sealed record KeycloakTokenResponse
{
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; init; }

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; init; }
}
