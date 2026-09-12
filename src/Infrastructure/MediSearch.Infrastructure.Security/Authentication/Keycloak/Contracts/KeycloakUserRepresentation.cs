namespace MediSearch.Infrastructure.Security.Authentication.Keycloak.Contracts;

/// <summary>
/// The subset of Keycloak's <c>UserRepresentation</c> that MediSearch reads.
/// </summary>
internal sealed record KeycloakUserRepresentation
{
    public string? Id { get; init; }

    public string? Username { get; init; }

    public string? Email { get; init; }

    public bool Enabled { get; init; }

    public bool EmailVerified { get; init; }
}
