using MediSearch.Core.Application.Catalog.Comments.DTOs;

namespace MediSearch.Core.Application.Catalog.Comments.Commands.AddCommentReply;

[Authorize]
public sealed record AddCommentReplyCommand(Guid CommentId, string Content) : ICommand<CommentDto>;
