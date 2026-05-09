using Microsoft.Extensions.Options;

namespace MediSearch.Infrastructure.Templating.Shared.UrlBuilder;

public sealed class AppUrlBuilder(IOptionsMonitor<AppUrlOptions> options)
{
    private readonly IOptionsMonitor<AppUrlOptions> _options = options;

    public string ClientLoginUrl =>
        Combine(_options.CurrentValue.ClientBaseUrl, _options.CurrentValue.ClientLoginPath);

    public string BuildAssetUrl(string assetKey) =>
        Combine(_options.CurrentValue.AssetBaseUrl, Escape(assetKey));

    public string BuildEmailConfirmationUrl(string externalUserId, string token) =>
        Combine(
            _options.CurrentValue.ApiBaseUrl,
            string.Format(
                _options.CurrentValue.ApiEmailConfirmationPath,
                externalUserId,
                Escape(token)
            )
        );

    public string BuildResetPasswordUrl(string externalUserId, string token) =>
        Combine(
            _options.CurrentValue.ClientBaseUrl,
            string.Format(
                _options.CurrentValue.ClientResetPasswordPath,
                externalUserId,
                Escape(token)
            )
        );

    public string BuildEmailConfirmationResultUrl(string status) =>
        Combine(
            _options.CurrentValue.ClientBaseUrl,
            string.Format(_options.CurrentValue.ClientEmailConfirmationResultPath, Escape(status))
        );

    private static string Escape(string value) => Uri.EscapeDataString(value);

    private static string Combine(string baseUrl, string path) =>
        $"{baseUrl.TrimEnd('/')}/{path.TrimStart('/')}";
}
