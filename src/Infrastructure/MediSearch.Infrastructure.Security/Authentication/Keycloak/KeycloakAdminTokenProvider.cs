using System.Diagnostics.CodeAnalysis;
using MediSearch.Core.Application.Shared.Ports;
using MediSearch.Infrastructure.Security.Authentication.Keycloak.Contracts;

namespace MediSearch.Infrastructure.Security.Authentication.Keycloak;

/// <summary>
/// Caches the service-account access token the Web API uses against the Admin REST API.
/// Keycloak access tokens are short lived (five minutes in the shipped realm), so the
/// token is refreshed on demand rather than stored anywhere.
/// </summary>
internal sealed class KeycloakAdminTokenProvider(
    KeycloakTokenClient tokenClient,
    IDateTimeProvider dateTimeProvider
)
{
    private static readonly TimeSpan ExpirationSkew = TimeSpan.FromSeconds(30);

    private readonly KeycloakTokenClient _tokenClient = tokenClient;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly SemaphoreSlim _refreshGate = new(1, 1);

    private string? _accessToken;
    private DateTime _expiresAtUtc = DateTime.MinValue;

    public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        if (TryGetCachedToken(out string? cachedToken))
        {
            return cachedToken;
        }

        await _refreshGate.WaitAsync(cancellationToken);

        try
        {
            if (TryGetCachedToken(out cachedToken))
            {
                return cachedToken;
            }

            KeycloakTokenResponse token = await _tokenClient.RequestClientCredentialsTokenAsync(
                cancellationToken
            );

            string accessToken = Guard.Against.NullOrWhiteSpace(token.AccessToken);

            _accessToken = accessToken;
            _expiresAtUtc = _dateTimeProvider.UtcNow.AddSeconds(token.ExpiresIn);

            return accessToken;
        }
        finally
        {
            _refreshGate.Release();
        }
    }

    /// <summary>
    /// Drops the cached token so the next call fetches a fresh one. Used when Keycloak
    /// answers an administrative call with 401, which usually means the token was revoked
    /// or the realm was restarted.
    /// </summary>
    public void Invalidate()
    {
        _accessToken = null;
        _expiresAtUtc = DateTime.MinValue;
    }

    private bool TryGetCachedToken([NotNullWhen(true)] out string? token)
    {
        token = _accessToken;

        return !string.IsNullOrWhiteSpace(token)
            && _dateTimeProvider.UtcNow.Add(ExpirationSkew) < _expiresAtUtc;
    }
}
