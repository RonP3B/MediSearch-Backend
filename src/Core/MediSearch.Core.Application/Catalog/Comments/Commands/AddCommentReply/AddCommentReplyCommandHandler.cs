using MediSearch.Core.Application.Catalog.Comments.DTOs;
using MediSearch.Core.Application.Catalog.Comments.Ports;
using MediSearch.Core.Domain.Catalog.Comments;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;
using MediSearch.Core.Domain.Users;

namespace MediSearch.Core.Application.Catalog.Comments.Commands.AddCommentReply;

public sealed class AddCommentReplyCommandHandler(
    ICommentRepository commentRepository,
    ICurrentUser currentUser,
    ICommentQueryService commentQueryService
) : ICommandHandler<AddCommentReplyCommand, CommentDto>
{
    private readonly ICommentRepository _commentRepository = commentRepository;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly ICommentQueryService _commentQueryService = commentQueryService;

    public async Task<CommentDto> Handle(
        AddCommentReplyCommand cmd,
        CancellationToken cancellationToken
    )
    {
        var parentComment = await _commentRepository.GetByIdOrDefaultAsync(
            EntityId<Comment>.From(cmd.CommentId),
            cancellationToken
        );

        if (parentComment is null)
        {
            throw NotFoundException.Entity(nameof(Comment), nameof(Comment.Id), cmd.CommentId);
        }

        Guid authenticatedUserId = _currentUser.GetAuthenticatedUserId();

        var commentReply = Comment.CreateCommentReply(
            content: CleanText.From(cmd.Content),
            userId: EntityId<User>.From(authenticatedUserId),
            parent: parentComment
        );

        _commentRepository.Add(commentReply);

        return commentReply.Adapt<CommentDto>() with
        {
            Author = await _commentQueryService.GetCommentAuthorAsync(
                authenticatedUserId,
                cancellationToken
            ),
        };
    }
}
