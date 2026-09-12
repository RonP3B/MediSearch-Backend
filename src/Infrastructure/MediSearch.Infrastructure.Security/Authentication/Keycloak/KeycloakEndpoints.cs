namespace MediSearch.Infrastructure.Security.Authentication.Keycloak;

/// <summary>
/// Relative Keycloak URLs. They are combined with the base address of the named
/// <c>Keycloak</c> <see cref="HttpClient"/>, so none of them starts with a slash.
/// </summary>
internal static class KeycloakEndpoints
{
    public static string Token(string realm) =>
        $"realms/{Uri.EscapeDataString(realm)}/protocol/openid-connect/token";

    public static string Users(string realm) => $"admin/realms/{Uri.EscapeDataString(realm)}/users";

    public static string User(string realm, string userId) =>
        $"{Users(realm)}/{Uri.EscapeDataString(userId)}";

    public static string UserCredentials(string realm, string userId) =>
        $"{User(realm, userId)}/credentials";

    public static string UserResetPassword(string realm, string userId) =>
        $"{User(realm, userId)}/reset-password";

    public static string UsersByExactUsername(string realm, string username) =>
        $"{Users(realm)}?username={Uri.EscapeDataString(username)}&exact=true";
}
