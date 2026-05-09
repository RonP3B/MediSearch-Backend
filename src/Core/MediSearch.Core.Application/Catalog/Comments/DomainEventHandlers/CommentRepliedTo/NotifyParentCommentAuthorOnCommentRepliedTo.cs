using MediSearch.Core.Application.Catalog.Comments.Notifications.CommentRepliedTo;
using MediSearch.Core.Application.Catalog.Comments.Ports;
using MediSearch.Core.Domain.Catalog.Comments;

namespace MediSearch.Core.Application.Catalog.Comments.DomainEventHandlers.CommentRepliedTo;

public sealed class NotifyParentCommentAuthorOnCommentRepliedTo(
    ICommentQueryService commentQueryService,
    IEventBus eventBus
) : DomainEventHandler<CommentRepliedToDomainEvent>
{
    private readonly ICommentQueryService _commentQueryService = commentQueryService;
    private readonly IEventBus _eventBus = eventBus;

    public override async Task Handle(
        CommentRepliedToDomainEvent domainEvent,
        CancellationToken cancellationToken
    )
    {
        var commentReplyForAuthor = await _commentQueryService.GetCommentReplyForAuthorAsync(
            domainEvent.ParentCommentId,
            domainEvent.UserId,
            cancellationToken
        );

        if (commentReplyForAuthor.ParentCommentAuthorUserId == domainEvent.UserId)
        {
            return; // No need to notify if the author of the parent comment is the same as the replier
        }

        await _eventBus.PublishAsync(
            new CommentRepliedToNotification(
                commentReplyForAuthor.ParentCommentAuthorEmail,
                commentReplyForAuthor.ParentCommentAuthorDisplayName,
                commentReplyForAuthor.ReplyAuthorName,
                domainEvent.Content,
                $"{domainEvent.Id}:comment-replied-to"
            ),
            cancellationToken
        );
    }
}
