using System.Text;
using MediSearch.Core.Application.Accounts.Ports;
using MediSearch.Infrastructure.Security.Authentication.AccountTokens;
using MediSearch.Infrastructure.Security.Authentication.Jwt;
using MediSearch.Infrastructure.Security.Authentication.Keycloak;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Microsoft.Extensions.DependencyInjection;

public static partial class DependencyInjection
{
    private const string JwtSection = "JWT";
    private const string AccountTokensSection = "AccountTokens";
    private const string KeycloakSection = "Keycloak";

    private static void AddAuthenticationServices(this IHostApplicationBuilder builder)
    {
        JwtOptions jwtOptions = Guard.Against.Null(
            builder.Configuration.GetSection(JwtSection).Get<JwtOptions>(),
            $"Configuration key '{JwtSection}' is missing or empty."
        );

        builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtSection));

        builder.AddAccountActionTokenServices();

        builder.AddKeycloakAccountServices();

        builder.Services.AddScoped<IAuthenticationTokenService, AuthenticationJwtService>();

        builder
            .Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtOptions.AccessTokenSecretKey)
                    ),
                }
            );
    }

    private static void AddAccountActionTokenServices(this IHostApplicationBuilder builder)
    {
        AccountTokenOptions accountTokenOptions = Guard.Against.Null(
            builder.Configuration.GetSection(AccountTokensSection).Get<AccountTokenOptions>(),
            $"Configuration key '{AccountTokensSection}' is missing or empty."
        );

        Guard.Against.NullOrWhiteSpace(
            accountTokenOptions.SecretKey,
            message: $"Configuration key '{AccountTokensSection}:SecretKey' is missing or empty."
        );

        builder.Services.Configure<AccountTokenOptions>(
            builder.Configuration.GetSection(AccountTokensSection)
        );

        builder.Services.AddSingleton<AccountActionTokenService>();
    }

    /// <summary>
    /// Registers everything the Web API needs in order to use Keycloak as its account store.
    /// A single named <see cref="HttpClient"/> points at the Keycloak base URL; the token
    /// client and the Admin API client share it.
    /// </summary>
    private static void AddKeycloakAccountServices(this IHostApplicationBuilder builder)
    {
        KeycloakOptions keycloakOptions = Guard.Against.Null(
            builder.Configuration.GetSection(KeycloakSection).Get<KeycloakOptions>(),
            $"Configuration key '{KeycloakSection}' is missing or empty."
        );

        string keycloakUrl = Guard.Against.NullOrWhiteSpace(
            keycloakOptions.Url,
            message: $"Configuration key '{KeycloakSection}:Url' is missing or empty."
        );

        Guard.Against.NullOrWhiteSpace(
            keycloakOptions.Realm,
            message: $"Configuration key '{KeycloakSection}:Realm' is missing or empty."
        );

        Guard.Against.NullOrWhiteSpace(
            keycloakOptions.ClientId,
            message: $"Configuration key '{KeycloakSection}:ClientId' is missing or empty."
        );

        Guard.Against.NullOrWhiteSpace(
            keycloakOptions.ClientSecret,
            message: $"Configuration key '{KeycloakSection}:ClientSecret' is missing or empty."
        );

        builder.Services.Configure<KeycloakOptions>(
            builder.Configuration.GetSection(KeycloakSection)
        );

        builder.Services.AddHttpClient(
            KeycloakConstants.HttpClientName,
            client => client.BaseAddress = new Uri($"{keycloakUrl.TrimEnd('/')}/")
        );

        builder.Services.AddSingleton<KeycloakTokenClient>();
        builder.Services.AddSingleton<KeycloakAdminTokenProvider>();
        builder.Services.AddSingleton<KeycloakAdminApiClient>();

        builder.Services.AddScoped<KeycloakAccountService>();

        builder.Services.AddScoped<IAccountManager>(sp =>
            sp.GetRequiredService<KeycloakAccountService>()
        );

        builder.Services.AddScoped<IAccountPasswordManager>(sp =>
            sp.GetRequiredService<KeycloakAccountService>()
        );

        builder.Services.AddScoped<ICredentialsValidator>(sp =>
            sp.GetRequiredService<KeycloakAccountService>()
        );
    }
}
