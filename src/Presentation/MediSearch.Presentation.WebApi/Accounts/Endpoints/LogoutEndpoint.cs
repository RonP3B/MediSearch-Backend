namespace MediSearch.Presentation.WebApi.Accounts.Endpoints;

internal sealed class LogoutEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(Logout, "logout");
    }

    [EndpointSummary("Logout")]
    [EndpointDescription(
        "Logs out the current authenticated account by removing the refresh token cookie."
    )]
    public static NoContent Logout(HttpContext httpContext)
    {
        httpContext.Response.Cookies.Delete(
            "refreshToken",
            new CookieOptions { HttpOnly = true, Secure = true }
        );

        return TypedResults.NoContent();
    }
}
