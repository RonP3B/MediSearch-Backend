using MediSearch.Core.Application.Accounts.Commands.ChangePassword;

namespace MediSearch.Presentation.WebApi.Accounts.Endpoints;

internal sealed class ChangePasswordEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPut(ChangePassword, "password");
    }

    [EndpointSummary("Change Password")]
    [EndpointDescription("Changes the password of the currently authenticated account.")]
    public static async Task<NoContent> ChangePassword(
        ChangePasswordRequest request,
        ISender sender
    )
    {
        await sender.Send(new ChangePasswordCommand(request.CurrentPassword, request.NewPassword));
        return TypedResults.NoContent();
    }
}

internal sealed record ChangePasswordRequest
{
    public required string CurrentPassword { get; init; }
    public required string NewPassword { get; init; }
}
