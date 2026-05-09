using System.Text;
using MediSearch.Core.Application.Accounts.Ports;
using MediSearch.Infrastructure.Security.Authentication.AspNetCoreIdentity;
using MediSearch.Infrastructure.Security.Authentication.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Microsoft.Extensions.DependencyInjection;

public static partial class DependencyInjection
{
    private const string JwtSection = "JWT";

    private static void AddAuthenticationServices(this IHostApplicationBuilder builder)
    {
        JwtOptions jwtOptions = Guard.Against.Null(
            builder.Configuration.GetSection(JwtSection).Get<JwtOptions>(),
            $"Configuration key '{JwtSection}' is missing or empty."
        );

        builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtSection));

        builder.Services.AddScoped<IAuthenticationTokenService, AuthenticationJwtService>();

        builder.Services.AddScoped<AspNetCoreIdentityAuthenticationService>();

        builder.Services.AddScoped<IAccountManager>(sp =>
            sp.GetRequiredService<AspNetCoreIdentityAuthenticationService>()
        );

        builder.Services.AddScoped<IAccountPasswordManager>(sp =>
            sp.GetRequiredService<AspNetCoreIdentityAuthenticationService>()
        );

        builder.Services.AddScoped<ICredentialsValidator>(sp =>
            sp.GetRequiredService<AspNetCoreIdentityAuthenticationService>()
        );

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
}
