using MediSearch.Core.Application.Catalog.Products.Notifications.ProductDeleted;
using MediSearch.Core.Application.Users.DTOs;
using MediSearch.Core.Application.Users.Ports;
using MediSearch.Core.Domain.Catalog.Products.DomainEvents;

namespace MediSearch.Core.Application.Catalog.Products.DomainEventHandlers.ProductDeleted;

public sealed class NotifyCompanyOwnerAndManagersOnProductDeleted(
    IUserQueryService userQueryService,
    IEmailService emailService,
    IEventBus eventBus
) : DomainEventHandler<ProductDeletedDomainEvent>
{
    private readonly IUserQueryService _userQueryService = userQueryService;
    private readonly IEmailService _emailService = emailService;
    private readonly IEventBus _eventBus = eventBus;

    public override async Task Handle(
        ProductDeletedDomainEvent domainEvent,
        CancellationToken cancellationToken
    )
    {
        UserContactInfoDto[] companyOwnerAndManagersContactInfo =
        [
            .. await _userQueryService.GetCompanyOwnerAndManagersContactInfoAsync(
                domainEvent.CompanyId,
                cancellationToken
            ),
        ];

        if (companyOwnerAndManagersContactInfo.Length == 0)
        {
            throw CorruptedInvariantException.MissingCompanyOwnerContact(domainEvent.CompanyId);
        }

        var notifications = companyOwnerAndManagersContactInfo
            .Chunk(_emailService.MaxBatchSize)
            .Select(
                (chunk, index) =>
                    new ProductDeletedNotification(
                        domainEvent.Name,
                        chunk,
                        $"{domainEvent.Id}:product-deleted:{index}"
                    )
            )
            .ToArray();

        await _eventBus.PublishBatchAsync(notifications, cancellationToken);
    }
}
