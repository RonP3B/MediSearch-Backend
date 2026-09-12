using System.Text.Json;
using System.Text.Json.Serialization;

namespace MediSearch.Infrastructure.Security.Authentication.Keycloak;

internal static class KeycloakJson
{
    /// <summary>
    /// Keycloak representations are camelCase. Nulls are never written so that a partial
    /// user update only changes the fields it actually carries.
    /// </summary>
    public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };
}
