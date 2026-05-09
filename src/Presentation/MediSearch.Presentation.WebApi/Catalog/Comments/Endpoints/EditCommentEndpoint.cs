using MediSearch.Core.Application.Catalog.Comments.Commands.EditComment;
using MediSearch.Core.Application.Catalog.Comments.DTOs;

namespace MediSearch.Presentation.WebApi.Catalog.Comments.Endpoints;

internal sealed class EditCommentEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPatch(EditComment, "{commentId:guid}");
    }

    [EndpointSummary("Edit Comment")]
    [EndpointDescription("Edits the content of an existing comment.")]
    public static async Task<Ok<CommentDto>> EditComment(
        Guid commentId,
        EditCommentRequest request,
        ISender sender
    )
    {
        var updatedComment = await sender.Send(
            new EditCommentCommand(commentId, request.NewContent)
        );

        return TypedResults.Ok(updatedComment);
    }
}

internal sealed record EditCommentRequest
{
    public required string NewContent { get; init; }
}
