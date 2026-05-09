using MediSearch.Core.Application.Catalog.Comments.DTOs;
using MediSearch.Core.Application.Catalog.Comments.Ports;
using MediSearch.Core.Domain.Catalog.Comments;
using MediSearch.Core.Domain.Catalog.Products;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;
using MediSearch.Core.Domain.Users;

namespace MediSearch.Core.Application.Catalog.Comments.Commands.AddCommentToProduct;

public sealed class AddCommentToProductCommandHandler(
    ICommentRepository commentRepository,
    ICurrentUser currentUser,
    ICommentQueryService commentQueryService
) : ICommandHandler<AddCommentToProductCommand, CommentDto>
{
    private readonly ICommentRepository _commentRepository = commentRepository;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly ICommentQueryService _commentQueryService = commentQueryService;

    public async Task<CommentDto> Handle(
        AddCommentToProductCommand cmd,
        CancellationToken cancellationToken
    )
    {
        Guid authenticatedUserId = _currentUser.GetAuthenticatedUserId();

        var comment = Comment.Create(
            productId: EntityId<Product>.From(cmd.ProductId),
            userId: EntityId<User>.From(authenticatedUserId),
            content: CleanText.From(cmd.Content)
        );

        _commentRepository.Add(comment);

        return comment.Adapt<CommentDto>() with
        {
            Author = await _commentQueryService.GetCommentAuthorAsync(
                authenticatedUserId,
                cancellationToken
            ),
        };
    }
}
