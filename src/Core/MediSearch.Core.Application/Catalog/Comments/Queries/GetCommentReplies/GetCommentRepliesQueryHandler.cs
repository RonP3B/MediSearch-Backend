using MediSearch.Core.Application.Catalog.Comments.DTOs;
using MediSearch.Core.Application.Catalog.Comments.Ports;

namespace MediSearch.Core.Application.Catalog.Comments.Queries.GetCommentReplies;

public sealed class GetCommentRepliesQueryHandler(ICommentQueryService commentQueryService)
    : IQueryHandler<GetCommentRepliesQuery, IReadOnlyList<CommentDto>>
{
    private readonly ICommentQueryService _commentQueryService = commentQueryService;

    public async Task<IReadOnlyList<CommentDto>> Handle(
        GetCommentRepliesQuery query,
        CancellationToken cancellationToken
    )
    {
        return await _commentQueryService.GetCommentRepliesAsync(
            query.CommentId,
            cancellationToken
        );
    }
}
