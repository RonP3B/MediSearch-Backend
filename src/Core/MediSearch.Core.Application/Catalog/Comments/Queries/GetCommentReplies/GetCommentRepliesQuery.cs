using MediSearch.Core.Application.Catalog.Comments.DTOs;

namespace MediSearch.Core.Application.Catalog.Comments.Queries.GetCommentReplies;

public sealed record GetCommentRepliesQuery(Guid CommentId) : IQuery<IReadOnlyList<CommentDto>>;
