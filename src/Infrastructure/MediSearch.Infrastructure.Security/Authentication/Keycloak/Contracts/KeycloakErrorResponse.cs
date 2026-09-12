using System.Text.Json.Serialization;

namespace MediSearch.Infrastructure.Security.Authentication.Keycloak.Contracts;

/// <summary>
/// Keycloak reports failures in a few different shapes depending on the endpoint:
/// the Admin REST API mostly uses <c>errorMessage</c>, the token endpoint uses
/// <c>error</c> plus <c>error_description</c>, and user-profile/password-policy
/// violations use a message key in <c>error</c>. All of them are captured here.
/// </summary>
internal sealed record KeycloakErrorResponse
{
    [JsonPropertyName("error")]
    public string? Error { get; init; }

    [JsonPropertyName("error_description")]
    public string? ErrorDescription { get; init; }

    [JsonPropertyName("errorMessage")]
    public string? ErrorMessage { get; init; }

    [JsonPropertyName("field")]
    public string? Field { get; init; }

    /// <summary>
    /// The first non-empty human readable message carried by the response.
    /// </summary>
    public string? Message =>
        FirstNonEmpty(ErrorMessage) ?? FirstNonEmpty(ErrorDescription) ?? FirstNonEmpty(Error);

    /// <summary>
    /// The message key Keycloak used, when it sent one.
    /// </summary>
    public string? Key => FirstNonEmpty(Error) ?? FirstNonEmpty(ErrorMessage);

    private static string? FirstNonEmpty(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value;
}
