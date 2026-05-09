using MediSearch.Core.Application.Catalog.Comments.DTOs;

namespace MediSearch.Core.Application.Catalog.Comments.Ports;

public interface ICommentQueryService : IQueryService
{
    Task<CommentPostedForCompanyDto> GetCommentPostedForCompanyAsync(
        Guid productId,
        Guid authorUserId,
        CancellationToken cancellationToken = default
    );

    Task<CommentReplyForAuthorDto> GetCommentReplyForAuthorAsync(
        Guid parentCommentId,
        Guid replyAuthorUserId,
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyList<CommentDto>> GetCommentRepliesAsync(
        Guid commentId,
        CancellationToken cancellationToken = default
    );

    Task<AuthorDto> GetCommentAuthorAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    );
}
