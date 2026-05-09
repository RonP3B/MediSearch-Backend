using MediSearch.Core.Application.Catalog.Comments.DTOs;

namespace MediSearch.Core.Application.Catalog.Comments.Commands.EditComment;

[Authorize]
public sealed record EditCommentCommand(Guid CommentId, string NewContent) : ICommand<CommentDto>;
