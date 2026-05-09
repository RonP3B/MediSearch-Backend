using MediSearch.Core.Application.Accounts.Commands.RefreshAccessToken;
using MediSearch.Core.Application.Accounts.DTOs;

namespace MediSearch.Presentation.WebApi.Accounts.Endpoints;

internal sealed class RefreshAccessTokenEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(RefreshAccessToken, "tokens");
    }

    [EndpointSummary("Refresh Access Token")]
    [EndpointDescription("Validates a refresh token and returns a new access token.")]
    public static async Task<Ok<RefreshedAccessTokenDto>> RefreshAccessToken(
        HttpContext httpContext,
        ISender sender
    )
    {
        httpContext.Request.Cookies.TryGetValue("refreshToken", out var refreshToken);
        var result = await sender.Send(new RefreshAccessTokenCommand(refreshToken ?? string.Empty));
        return TypedResults.Ok(result);
    }
}
