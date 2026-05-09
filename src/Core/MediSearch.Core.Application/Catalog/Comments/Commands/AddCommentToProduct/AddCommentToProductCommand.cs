using MediSearch.Core.Application.Catalog.Comments.DTOs;

namespace MediSearch.Core.Application.Catalog.Comments.Commands.AddCommentToProduct;

[Authorize]
public sealed record AddCommentToProductCommand(Guid ProductId, string Content)
    : ICommand<CommentDto>;
