using MediSearch.Core.Application.Accounts.Commands.RequestPasswordReset;

namespace MediSearch.Presentation.WebApi.Accounts.Endpoints;

internal sealed class RequestPasswordResetEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(RequestPasswordReset, "password-reset/{username}");
    }

    [EndpointSummary("Request Password Reset")]
    [EndpointDescription(
        "Generates a password reset token and triggers the password reset notification flow."
    )]
    public static async Task<NoContent> RequestPasswordReset(string username, ISender sender)
    {
        await sender.Send(new RequestPasswordResetCommand(username));
        return TypedResults.NoContent();
    }
}
