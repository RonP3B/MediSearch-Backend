using MediSearch.Core.Application.Catalog.Products.Notifications.ProductCreated;
using MediSearch.Core.Application.Users.DTOs;
using MediSearch.Core.Application.Users.Ports;
using MediSearch.Core.Domain.Catalog.Products.DomainEvents;

namespace MediSearch.Core.Application.Catalog.Products.DomainEventHandlers.ProductCreated;

public sealed class NotifyCompanyOwnerAndManagersProductCreated(
    IUserQueryService userQueryService,
    IEmailService emailService,
    IEventBus eventBus
) : DomainEventHandler<ProductCreatedDomainEvent>
{
    private readonly IUserQueryService _userQueryService = userQueryService;
    private readonly IEmailService _emailService = emailService;
    private readonly IEventBus _eventBus = eventBus;

    public override async Task Handle(
        ProductCreatedDomainEvent domainEvent,
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
                    new ProductCreatedNotification(
                        domainEvent.Name,
                        domainEvent.Price,
                        chunk,
                        $"{domainEvent.Id}:product-created:{index}"
                    )
            )
            .ToArray();

        await _eventBus.PublishBatchAsync(notifications, cancellationToken);
    }
}
