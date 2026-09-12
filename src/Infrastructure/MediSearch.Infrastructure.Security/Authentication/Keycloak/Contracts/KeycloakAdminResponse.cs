using System.Net;

namespace MediSearch.Infrastructure.Security.Authentication.Keycloak.Contracts;

/// <summary>
/// Outcome of a single Admin REST API call, normalized so the account service can
/// translate it without dealing with <see cref="HttpResponseMessage"/> directly.
/// </summary>
internal sealed record KeycloakAdminResponse
{
    public required bool IsSuccess { get; init; }

    public required HttpStatusCode StatusCode { get; init; }

    /// <summary>
    /// Identifier parsed out of the <c>Location</c> header returned by user creation.
    /// </summary>
    public string? CreatedResourceId { get; init; }

    public KeycloakErrorResponse? Error { get; init; }

    public static KeycloakAdminResponse Success(
        HttpStatusCode statusCode,
        string? createdResourceId = null
    )
    {
        return new KeycloakAdminResponse
        {
            IsSuccess = true,
            StatusCode = statusCode,
            CreatedResourceId = createdResourceId,
        };
    }

    public static KeycloakAdminResponse Failure(
        HttpStatusCode statusCode,
        KeycloakErrorResponse? error
    )
    {
        return new KeycloakAdminResponse
        {
            IsSuccess = false,
            StatusCode = statusCode,
            Error = error,
        };
    }
}
