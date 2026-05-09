using MediSearch.Core.Application.Catalog.Comments.Notifications.CommentPosted;
using MediSearch.Core.Application.Catalog.Comments.Ports;
using MediSearch.Core.Application.Users.DTOs;
using MediSearch.Core.Application.Users.Ports;
using MediSearch.Core.Domain.Catalog.Comments;

namespace MediSearch.Core.Application.Catalog.Comments.DomainEventHandlers.CommentPosted;

public sealed class NotifyCompanyUsersOnCommentPosted(
    ICommentQueryService commentQueryService,
    IUserQueryService userQueryService,
    IEmailService emailService,
    IEventBus eventBus
) : DomainEventHandler<CommentPostedDomainEvent>
{
    private readonly ICommentQueryService _commentQueryService = commentQueryService;
    private readonly IUserQueryService _userQueryService = userQueryService;
    private readonly IEmailService _emailService = emailService;
    private readonly IEventBus _eventBus = eventBus;

    public override async Task Handle(
        CommentPostedDomainEvent domainEvent,
        CancellationToken cancellationToken
    )
    {
        var commentPostedForCompany = await _commentQueryService.GetCommentPostedForCompanyAsync(
            domainEvent.ProductId,
            domainEvent.UserId,
            cancellationToken
        );

        if (commentPostedForCompany.SellerCompanyId == commentPostedForCompany.AuthorCompanyId)
        {
            return; // No need to notify if the comment author belongs to the same company as the seller
        }

        UserContactInfoDto[] companyUsersContactInfo =
        [
            .. await _userQueryService.GetCompanyUsersContactInfoAsync(
                commentPostedForCompany.SellerCompanyId,
                cancellationToken
            ),
        ];

        var notifications = companyUsersContactInfo
            .Chunk(_emailService.MaxBatchSize)
            .Select(
                (chunk, index) =>
                    new CommentPostedNotification(
                        commentPostedForCompany.SellerCompanyName,
                        commentPostedForCompany.ProductName,
                        commentPostedForCompany.CommentAuthorName,
                        domainEvent.Content,
                        chunk,
                        $"{domainEvent.Id}:comment-posted:{index}"
                    )
            )
            .ToArray();

        await _eventBus.PublishBatchAsync(notifications, cancellationToken);
    }
}
