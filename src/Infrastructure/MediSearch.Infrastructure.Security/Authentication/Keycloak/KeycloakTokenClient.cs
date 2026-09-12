using System.Net;
using System.Net.Http.Json;
using MediSearch.Infrastructure.Security.Authentication.Keycloak.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MediSearch.Infrastructure.Security.Authentication.Keycloak;

/// <summary>
/// Talks to the realm's OpenID Connect token endpoint.
/// </summary>
/// <remarks>
/// Two grants are used, and neither of them issues tokens that MediSearch hands to
/// callers. The API keeps signing its own access and refresh tokens.
/// <list type="bullet">
/// <item>
/// <c>client_credentials</c> authenticates the Web API itself so it can call the
/// Admin REST API.
/// </item>
/// <item>
/// <c>password</c> (direct access grant) is the only supported way to ask Keycloak
/// "is this the right password for this user?", because password hashes never leave
/// Keycloak. The tokens that come back are discarded.
/// </item>
/// </list>
/// </remarks>
internal sealed class KeycloakTokenClient(
    IHttpClientFactory httpClientFactory,
    IOptions<KeycloakOptions> options,
    ILogger<KeycloakTokenClient> logger
)
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly KeycloakOptions _options = options.Value;
    private readonly ILogger<KeycloakTokenClient> _logger = logger;

    public async Task<KeycloakTokenResponse> RequestClientCredentialsTokenAsync(
        CancellationToken cancellationToken = default
    )
    {
        using HttpResponseMessage response = await PostFormAsync(
            new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = _options.ClientId,
                ["client_secret"] = _options.ClientSecret,
            },
            cancellationToken
        );

        if (!response.IsSuccessStatusCode)
        {
            string body = await response.Content.ReadAsStringAsync(cancellationToken);

            throw new InvalidOperationException(
                $"Keycloak rejected the credentials of client '{_options.ClientId}' in realm "
                    + $"'{_options.Realm}' ({(int)response.StatusCode}): {body}"
            );
        }

        KeycloakTokenResponse? token =
            await response.Content.ReadFromJsonAsync<KeycloakTokenResponse>(
                KeycloakJson.Options,
                cancellationToken
            );

        if (token is null || string.IsNullOrWhiteSpace(token.AccessToken))
        {
            throw new InvalidOperationException(
                "Keycloak returned an empty access token for the client credentials grant."
            );
        }

        return token;
    }

    /// <summary>
    /// Returns <see langword="true"/> when Keycloak accepts the username/password pair.
    /// Anything that is not a credential rejection is surfaced as an exception so that an
    /// unreachable or misconfigured Keycloak is never reported to the user as a wrong password.
    /// </summary>
    public async Task<bool> IsPasswordValidAsync(
        string username,
        string password,
        CancellationToken cancellationToken = default
    )
    {
        using HttpResponseMessage response = await PostFormAsync(
            new Dictionary<string, string>
            {
                ["grant_type"] = "password",
                ["client_id"] = _options.ClientId,
                ["client_secret"] = _options.ClientSecret,
                ["username"] = username,
                ["password"] = password,
            },
            cancellationToken
        );

        if (response.IsSuccessStatusCode)
        {
            return true;
        }

        string body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (IsCredentialRejection(response.StatusCode, body))
        {
            // Keycloak rejects a password and a not-fully-set-up account with the same
            // error, so the description is the only way to tell them apart. It is logged
            // rather than returned, because the caller deliberately reports one generic
            // "invalid credentials" result to the user.
            _logger.LogWarning(
                "Keycloak rejected the credentials of '{Username}': {Reason}",
                username,
                KeycloakErrorParser.Parse(body)?.Message ?? "no reason given"
            );

            return false;
        }

        throw new InvalidOperationException(
            $"Keycloak could not verify the password of '{username}' in realm "
                + $"'{_options.Realm}' ({(int)response.StatusCode}): {body}"
        );
    }

    private static bool IsCredentialRejection(HttpStatusCode statusCode, string body)
    {
        if (statusCode is not HttpStatusCode.Unauthorized and not HttpStatusCode.BadRequest)
        {
            return false;
        }

        KeycloakErrorResponse? error = KeycloakErrorParser.Parse(body);

        return statusCode == HttpStatusCode.Unauthorized
            || string.Equals(
                error?.Error,
                KeycloakConstants.InvalidGrantError,
                StringComparison.OrdinalIgnoreCase
            );
    }

    private async Task<HttpResponseMessage> PostFormAsync(
        Dictionary<string, string> form,
        CancellationToken cancellationToken
    )
    {
        HttpClient httpClient = _httpClientFactory.CreateClient(KeycloakConstants.HttpClientName);

        using FormUrlEncodedContent content = new(form);

        return await httpClient.PostAsync(
            KeycloakEndpoints.Token(_options.Realm),
            content,
            cancellationToken
        );
    }
}
