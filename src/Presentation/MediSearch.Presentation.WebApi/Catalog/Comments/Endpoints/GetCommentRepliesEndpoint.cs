using MediSearch.Core.Application.Catalog.Comments.DTOs;
using MediSearch.Core.Application.Catalog.Comments.Queries.GetCommentReplies;

namespace MediSearch.Presentation.WebApi.Catalog.Comments.Endpoints;

internal sealed class GetCommentRepliesEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetCommentRepliesByCommentId, "{commentId:guid}/replies");
    }

    [EndpointSummary("Get Comment Replies")]
    [EndpointDescription("Gets all replies for a comment.")]
    public static async Task<Ok<IReadOnlyList<CommentDto>>> GetCommentRepliesByCommentId(
        Guid commentId,
        ISender sender
    )
    {
        var result = await sender.Send(new GetCommentRepliesQuery(commentId));
        return TypedResults.Ok(result);
    }
}
