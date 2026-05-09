namespace MediSearch.Infrastructure.Templating.Shared.UrlBuilder;

public sealed record AppUrlOptions
{
    public required string ApiBaseUrl { get; init; }
    public required string ApiEmailConfirmationPath { get; init; }
    public required string AssetBaseUrl { get; init; }
    public required string ClientBaseUrl { get; init; }
    public required string ClientResetPasswordPath { get; init; }
    public required string ClientLoginPath { get; init; }
    public required string ClientEmailConfirmationResultPath { get; init; }
}
