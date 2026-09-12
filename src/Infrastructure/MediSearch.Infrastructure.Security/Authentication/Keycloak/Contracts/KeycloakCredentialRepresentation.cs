namespace MediSearch.Infrastructure.Security.Authentication.Keycloak.Contracts;

/// <summary>
/// Keycloak <c>CredentialRepresentation</c>. Used to send a password when creating or
/// resetting an account, and to read the password credential metadata back.
/// </summary>
internal sealed record KeycloakCredentialRepresentation
{
    public string? Type { get; init; }

    public string? Value { get; init; }

    public bool? Temporary { get; init; }

    /// <summary>
    /// Epoch milliseconds the credential was created at. Keycloak never returns the
    /// password itself, but it does return this timestamp, and it changes on every
    /// password reset. That makes it a natural replacement for the ASP.NET Core Identity
    /// security stamp: password reset links carry it and stop working once it moves.
    /// </summary>
    public long? CreatedDate { get; init; }
}
