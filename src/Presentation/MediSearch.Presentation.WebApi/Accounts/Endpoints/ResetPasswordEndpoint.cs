using MediSearch.Core.Application.Accounts.Commands.ResetPassword;

namespace MediSearch.Presentation.WebApi.Accounts.Endpoints;

internal sealed class ResetPasswordEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPut(ResetPassword, "password-reset");
    }

    [EndpointSummary("Reset Password")]
    [EndpointDescription(
        "Resets an account password using an external user id, reset token, and the new password."
    )]
    public static async Task<NoContent> ResetPassword(ResetPasswordRequest request, ISender sender)
    {
        await sender.Send(
            new ResetPasswordCommand(
                request.ExternalUserId,
                request.ResetToken,
                request.NewPassword
            )
        );

        return TypedResults.NoContent();
    }
}

internal sealed record ResetPasswordRequest
{
    public required string ExternalUserId { get; init; }
    public required string ResetToken { get; init; }
    public required string NewPassword { get; init; }
}
