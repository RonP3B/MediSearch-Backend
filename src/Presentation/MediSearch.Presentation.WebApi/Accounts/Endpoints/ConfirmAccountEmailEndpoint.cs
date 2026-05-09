using MediSearch.Core.Application.Accounts.Commands.ConfirmAccountEmail;
using MediSearch.Core.Application.Accounts.Enums;
using MediSearch.Infrastructure.Templating.Shared.UrlBuilder;

namespace MediSearch.Presentation.WebApi.Accounts.Endpoints;

internal sealed class ConfirmAccountEmailEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(ConfirmAccountEmail, "email-confirmation");
    }

    [EndpointSummary("Confirm Account Email")]
    [EndpointDescription(
        "Confirms an account email using the external user id and confirmation token."
    )]
    public static async Task<RedirectHttpResult> ConfirmAccountEmail(
        [AsParameters] ConfirmAccountEmailRequest request,
        ISender sender,
        AppUrlBuilder appUrlBuilder
    )
    {
        var result = await sender.Send(
            new ConfirmAccountEmailCommand(request.ExternalUserId, request.ConfirmationToken)
        );

        var status = result.ActivationStatus switch
        {
            AccountEmailConfirmationStatus.Success => "success",
            AccountEmailConfirmationStatus.AlreadyConfirmed => "already-confirmed",
            AccountEmailConfirmationStatus.InvalidToken => "invalid-token",
            _ => "invalid-token",
        };

        return TypedResults.Redirect(appUrlBuilder.BuildEmailConfirmationResultUrl(status));
    }
}

internal sealed record ConfirmAccountEmailRequest
{
    public required string ExternalUserId { get; init; }
    public required string ConfirmationToken { get; init; }
}
