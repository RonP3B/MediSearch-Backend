using Ardalis.GuardClauses;

namespace Microsoft.Extensions.DependencyInjection;

internal static partial class DependencyInjection
{
    private const string CorsPoliciesSectionName = "Cors";

    public static void AddCorsServices(this IHostApplicationBuilder builder)
    {
        CorsSettings settings = Guard.Against.Null(
            builder.Configuration.GetSection(CorsPoliciesSectionName).Get<CorsSettings>(),
            $"Configuration key '{CorsPoliciesSectionName}' is missing or empty."
        );

        Guard.Against.NullOrEmpty(
            settings.AllowedOrigins,
            $"Configuration key '{CorsPoliciesSectionName}.{nameof(CorsSettings.AllowedOrigins)}' is missing or empty."
        );

        builder.Services.AddCors(options =>
            options.AddPolicy(
                CorsPolicies.DynamicCors,
                corsPolicyBuilder =>
                    corsPolicyBuilder
                        .WithOrigins(settings.AllowedOrigins)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
            )
        );
    }
}
