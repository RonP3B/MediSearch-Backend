namespace MediSearch.Infrastructure.Security.Authentication.Keycloak.Contracts;

/// <summary>
/// The subset of Keycloak's <c>UserRepresentation</c> that MediSearch writes.
/// Every property is nullable on purpose: Keycloak only applies the fields that are
/// present, so a partial update such as <c>{ "emailVerified": true }</c> leaves the
/// rest of the account untouched.
/// </summary>
internal sealed record KeycloakUserUpsertRequest
{
    public string? Username { get; init; }

    public string? Email { get; init; }

    public bool? Enabled { get; init; }

    public bool? EmailVerified { get; init; }

    public IReadOnlyList<KeycloakCredentialRepresentation>? Credentials { get; init; }
}
