using MediSearch.Core.Application.Accounts.Commands.RequestAccountEmailConfirmation;

namespace MediSearch.Presentation.WebApi.Accounts.Endpoints;

internal sealed class RequestAccountEmailConfirmationEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(RequestAccountEmailConfirmation, "email-confirmation/{username}");
    }

    [EndpointSummary("Request Account Email Confirmation")]
    [EndpointDescription(
        "Generates a confirmation link and triggers the email confirmation notification flow."
    )]
    public static async Task<NoContent> RequestAccountEmailConfirmation(
        string username,
        ISender sender
    )
    {
        await sender.Send(new RequestAccountEmailConfirmationCommand(username));
        return TypedResults.NoContent();
    }
}
