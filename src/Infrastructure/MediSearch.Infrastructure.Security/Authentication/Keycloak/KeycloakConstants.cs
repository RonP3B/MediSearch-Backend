namespace MediSearch.Infrastructure.Security.Authentication.Keycloak;

internal static class KeycloakConstants
{
    /// <summary>
    /// Name of the named <see cref="HttpClient"/> pointed at the Keycloak base URL.
    /// </summary>
    public const string HttpClientName = "Keycloak";

    /// <summary>
    /// Credential type Keycloak uses for passwords.
    /// </summary>
    public const string PasswordCredentialType = "password";

    /// <summary>
    /// Error returned by the token endpoint when the submitted credentials are rejected.
    /// </summary>
    public const string InvalidGrantError = "invalid_grant";
}
