using MediSearch.Core.Application.Catalog.Comments.Commands.AddCommentReply;
using MediSearch.Core.Application.Catalog.Comments.DTOs;

namespace MediSearch.Presentation.WebApi.Catalog.Comments.Endpoints;

internal sealed class AddCommentReplyEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(AddCommentReply, "{commentId:guid}/replies");
    }

    [EndpointSummary("Add Comment Reply")]
    [EndpointDescription("Adds a reply to an existing comment.")]
    public static async Task<Created<CommentDto>> AddCommentReply(
        Guid commentId,
        AddCommentReplyRequest request,
        ISender sender,
        LinkGenerator linkGenerator,
        HttpContext httpContext
    )
    {
        var createdComment = await sender.Send(
            new AddCommentReplyCommand(commentId, request.Content)
        );

        return TypedResults.Created(
            linkGenerator.GetUriByName(
                httpContext,
                nameof(GetCommentRepliesEndpoint.GetCommentRepliesByCommentId),
                new { commentId }
            ),
            createdComment
        );
    }
}

internal sealed record AddCommentReplyRequest
{
    public required string Content { get; init; }
}
