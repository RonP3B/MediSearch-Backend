using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using MediSearch.Infrastructure.Security.Authentication.Keycloak.Contracts;
using Microsoft.Extensions.Options;

namespace MediSearch.Infrastructure.Security.Authentication.Keycloak;

/// <summary>
/// Thin wrapper over the Keycloak Admin REST API. It replaces the calls that used to go
/// through <c>UserManager&lt;IdentityUser&gt;</c>: every user lives in Keycloak now, so
/// creating, reading, updating and deleting an account is an HTTP call instead of an
/// EF Core query.
/// </summary>
internal sealed class KeycloakAdminApiClient(
    IHttpClientFactory httpClientFactory,
    KeycloakAdminTokenProvider adminTokenProvider,
    IOptions<KeycloakOptions> options
)
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly KeycloakAdminTokenProvider _adminTokenProvider = adminTokenProvider;
    private readonly KeycloakOptions _options = options.Value;

    public async Task<KeycloakUserRepresentation?> FindUserByIdOrDefaultAsync(
        string userId,
        CancellationToken cancellationToken = default
    )
    {
        using HttpResponseMessage response = await SendAsync(
            () => new HttpRequestMessage(HttpMethod.Get, KeycloakEndpoints.User(Realm, userId)),
            cancellationToken
        );

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        await EnsureSuccessAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<KeycloakUserRepresentation>(
            KeycloakJson.Options,
            cancellationToken
        );
    }

    public async Task<KeycloakUserRepresentation?> FindUserByUsernameOrDefaultAsync(
        string username,
        CancellationToken cancellationToken = default
    )
    {
        using HttpResponseMessage response = await SendAsync(
            () =>
                new HttpRequestMessage(
                    HttpMethod.Get,
                    KeycloakEndpoints.UsersByExactUsername(Realm, username)
                ),
            cancellationToken
        );

        await EnsureSuccessAsync(response, cancellationToken);

        List<KeycloakUserRepresentation>? users = await response.Content.ReadFromJsonAsync<
            List<KeycloakUserRepresentation>
        >(KeycloakJson.Options, cancellationToken);

        return users?.FirstOrDefault(user =>
            string.Equals(user.Username, username, StringComparison.OrdinalIgnoreCase)
        );
    }

    public async Task<KeycloakAdminResponse> CreateUserAsync(
        KeycloakUserUpsertRequest user,
        CancellationToken cancellationToken = default
    )
    {
        using HttpResponseMessage response = await SendAsync(
            () => JsonRequest(HttpMethod.Post, KeycloakEndpoints.Users(Realm), user),
            cancellationToken
        );

        return response.IsSuccessStatusCode
            ? KeycloakAdminResponse.Success(response.StatusCode, ExtractCreatedResourceId(response))
            : KeycloakAdminResponse.Failure(
                response.StatusCode,
                await ReadErrorAsync(response, cancellationToken)
            );
    }

    public async Task<KeycloakAdminResponse> UpdateUserAsync(
        string userId,
        KeycloakUserUpsertRequest user,
        CancellationToken cancellationToken = default
    )
    {
        using HttpResponseMessage response = await SendAsync(
            () => JsonRequest(HttpMethod.Put, KeycloakEndpoints.User(Realm, userId), user),
            cancellationToken
        );

        return await ToAdminResponseAsync(response, cancellationToken);
    }

    public async Task<KeycloakAdminResponse> DeleteUserAsync(
        string userId,
        CancellationToken cancellationToken = default
    )
    {
        using HttpResponseMessage response = await SendAsync(
            () => new HttpRequestMessage(HttpMethod.Delete, KeycloakEndpoints.User(Realm, userId)),
            cancellationToken
        );

        return await ToAdminResponseAsync(response, cancellationToken);
    }

    public async Task<KeycloakAdminResponse> ResetPasswordAsync(
        string userId,
        string newPassword,
        CancellationToken cancellationToken = default
    )
    {
        KeycloakCredentialRepresentation credential = new()
        {
            Type = KeycloakConstants.PasswordCredentialType,
            Value = newPassword,
            Temporary = false,
        };

        using HttpResponseMessage response = await SendAsync(
            () =>
                JsonRequest(
                    HttpMethod.Put,
                    KeycloakEndpoints.UserResetPassword(Realm, userId),
                    credential
                ),
            cancellationToken
        );

        return await ToAdminResponseAsync(response, cancellationToken);
    }

    /// <summary>
    /// Returns the creation timestamp of the account's password credential, which changes
    /// on every password reset. Password reset links embed it so that a link stops working
    /// as soon as the password has actually been changed.
    /// </summary>
    public async Task<long?> GetPasswordCredentialTimestampOrDefaultAsync(
        string userId,
        CancellationToken cancellationToken = default
    )
    {
        using HttpResponseMessage response = await SendAsync(
            () =>
                new HttpRequestMessage(
                    HttpMethod.Get,
                    KeycloakEndpoints.UserCredentials(Realm, userId)
                ),
            cancellationToken
        );

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        await EnsureSuccessAsync(response, cancellationToken);

        List<KeycloakCredentialRepresentation>? credentials =
            await response.Content.ReadFromJsonAsync<List<KeycloakCredentialRepresentation>>(
                KeycloakJson.Options,
                cancellationToken
            );

        return credentials
            ?.FirstOrDefault(credential =>
                string.Equals(
                    credential.Type,
                    KeycloakConstants.PasswordCredentialType,
                    StringComparison.OrdinalIgnoreCase
                )
            )
            ?.CreatedDate;
    }

    private string Realm => _options.Realm;

    private static HttpRequestMessage JsonRequest<TPayload>(
        HttpMethod method,
        string requestUri,
        TPayload payload
    )
    {
        return new HttpRequestMessage(method, requestUri)
        {
            Content = JsonContent.Create(payload, options: KeycloakJson.Options),
        };
    }

    private async Task<HttpResponseMessage> SendAsync(
        Func<HttpRequestMessage> requestFactory,
        CancellationToken cancellationToken
    )
    {
        HttpResponseMessage response = await SendOnceAsync(requestFactory(), cancellationToken);

        if (response.StatusCode != HttpStatusCode.Unauthorized)
        {
            return response;
        }

        // The cached service-account token is no longer accepted; get a new one and retry once.
        response.Dispose();
        _adminTokenProvider.Invalidate();

        return await SendOnceAsync(requestFactory(), cancellationToken);
    }

    private async Task<HttpResponseMessage> SendOnceAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        using (request)
        {
            string accessToken = await _adminTokenProvider.GetAccessTokenAsync(cancellationToken);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            HttpClient httpClient = _httpClientFactory.CreateClient(
                KeycloakConstants.HttpClientName
            );

            return await httpClient.SendAsync(request, cancellationToken);
        }
    }

    private static async Task<KeycloakAdminResponse> ToAdminResponseAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken
    )
    {
        return response.IsSuccessStatusCode
            ? KeycloakAdminResponse.Success(response.StatusCode)
            : KeycloakAdminResponse.Failure(
                response.StatusCode,
                await ReadErrorAsync(response, cancellationToken)
            );
    }

    private static async Task<KeycloakErrorResponse?> ReadErrorAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken
    )
    {
        string body = await response.Content.ReadAsStringAsync(cancellationToken);

        return KeycloakErrorParser.Parse(body);
    }

    private static async Task EnsureSuccessAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken
    )
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        string body = await response.Content.ReadAsStringAsync(cancellationToken);

        throw new InvalidOperationException(
            $"Keycloak Admin API call to '{response.RequestMessage?.RequestUri}' failed "
                + $"({(int)response.StatusCode}): {body}"
        );
    }

    private static string? ExtractCreatedResourceId(HttpResponseMessage response)
    {
        Uri? location = response.Headers.Location;

        if (location is null)
        {
            return null;
        }

        string path = (
            location.IsAbsoluteUri ? location.AbsolutePath : location.OriginalString
        ).TrimEnd('/');

        int lastSeparatorIndex = path.LastIndexOf('/');

        return lastSeparatorIndex >= 0 && lastSeparatorIndex < path.Length - 1
            ? path[(lastSeparatorIndex + 1)..]
            : null;
    }
}
