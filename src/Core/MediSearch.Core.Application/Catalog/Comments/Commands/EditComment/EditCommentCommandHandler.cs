using MediSearch.Core.Application.Catalog.Comments.DTOs;
using MediSearch.Core.Application.Catalog.Comments.Ports;
using MediSearch.Core.Domain.Catalog.Comments;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Core.Application.Catalog.Comments.Commands.EditComment;

public sealed class EditCommentCommandHandler(
    ICommentRepository commentRepository,
    ICurrentUser currentUser,
    IDateTimeProvider dateTimeProvider,
    ICommentQueryService commentQueryService
) : ICommandHandler<EditCommentCommand, CommentDto>
{
    private readonly ICommentRepository _commentRepository = commentRepository;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly ICommentQueryService _commentQueryService = commentQueryService;

    public async Task<CommentDto> Handle(
        EditCommentCommand cmd,
        CancellationToken cancellationToken
    )
    {
        var comment = await _commentRepository.GetByIdOrDefaultAsync(
            EntityId<Comment>.From(cmd.CommentId),
            cancellationToken
        );

        if (comment == null)
        {
            throw NotFoundException.Entity(nameof(Comment), nameof(Comment.Id), cmd.CommentId);
        }

        Guid authenticatedUserId = _currentUser.GetAuthenticatedUserId();

        if (comment.UserId != authenticatedUserId)
        {
            throw new ForbiddenAccessException();
        }

        comment.EditContent(
            newContent: CleanText.From(cmd.NewContent),
            now: _dateTimeProvider.UtcNow
        );

        return comment.Adapt<CommentDto>() with
        {
            Author = await _commentQueryService.GetCommentAuthorAsync(
                authenticatedUserId,
                cancellationToken
            ),
        };
    }
}
