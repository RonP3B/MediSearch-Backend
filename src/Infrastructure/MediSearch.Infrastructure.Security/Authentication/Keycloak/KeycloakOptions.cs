namespace MediSearch.Infrastructure.Security.Authentication.Keycloak;

/// <summary>
/// Settings the Web API needs in order to talk to the Keycloak realm that stores
/// MediSearch accounts.
/// </summary>
public sealed record KeycloakOptions
{
    /// <summary>
    /// Base URL of the Keycloak server, for example <c>http://localhost:8080</c>.
    /// When the solution runs through the Aspire AppHost this value is injected as
    /// the <c>Keycloak__Url</c> environment variable.
    /// </summary>
    public required string Url { get; init; }

    /// <summary>
    /// Realm that owns the MediSearch accounts.
    /// </summary>
    public required string Realm { get; init; }

    /// <summary>
    /// Client id of the confidential client the Web API authenticates as.
    /// </summary>
    public required string ClientId { get; init; }

    /// <summary>
    /// Secret of the confidential client. It is used both for the client-credentials
    /// grant (Admin REST API access) and for the password grant (credential checks).
    /// </summary>
    public required string ClientSecret { get; init; }
}
