using System.Text.Json;
using MediSearch.Infrastructure.Security.Authentication.Keycloak.Contracts;

namespace MediSearch.Infrastructure.Security.Authentication.Keycloak;

internal static class KeycloakErrorParser
{
    /// <summary>
    /// Best-effort parse of a Keycloak error body. Keycloak occasionally answers with
    /// plain text or an empty body, so a failure to parse is not an error in itself.
    /// </summary>
    public static KeycloakErrorResponse? Parse(string? body)
    {
        if (string.IsNullOrWhiteSpace(body))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<KeycloakErrorResponse>(body, KeycloakJson.Options);
        }
        catch (JsonException)
        {
            return new KeycloakErrorResponse { ErrorMessage = body };
        }
    }
}
