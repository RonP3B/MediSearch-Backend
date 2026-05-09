using MediSearch.Core.Application.Accounts.Commands.Login;
using MediSearch.Core.Application.Shared.Ports;
using MediSearch.Infrastructure.Security.Authentication.Jwt;
using Microsoft.Extensions.Options;

namespace MediSearch.Presentation.WebApi.Accounts.Endpoints;

internal sealed class LoginEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(Login, "login");
    }

    [EndpointSummary("Login")]
    [EndpointDescription("Authenticates a user and returns an access token plus a refresh token.")]
    public static async Task<Ok<LoginDto>> Login(
        LoginRequest request,
        HttpContext httpContext,
        IOptions<JwtOptions> jwtOptions,
        IDateTimeProvider dateTimeProvider,
        ISender sender
    )
    {
        var result = await sender.Send(new LoginCommand(request.Username, request.Password));

        httpContext.Response.Cookies.Append(
            "refreshToken",
            result.RefreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = dateTimeProvider.UtcNow.AddDays(
                    jwtOptions.Value.RefreshTokenExpirationDays
                ),
            }
        );

        return TypedResults.Ok(new LoginDto(result.AccessToken));
    }
}

internal sealed record LoginRequest
{
    public required string Username { get; init; }
    public required string Password { get; init; }
}

internal sealed record LoginDto(string AccessToken);
