using MediSearch.Core.Application.Catalog.Products.Notifications.ProductPriceChangedForCompanyOwnerAndManagers;
using MediSearch.Core.Application.Users.DTOs;
using MediSearch.Core.Application.Users.Ports;
using MediSearch.Core.Domain.Catalog.Products.DomainEvents;

namespace MediSearch.Core.Application.Catalog.Products.DomainEventHandlers.ProductPriceChanged;

public sealed class NotifyCompanyOwnerAndManagersOnProductPriceChanged(
    IUserQueryService userQueryService,
    IEmailService emailService,
    IEventBus eventBus
) : DomainEventHandler<ProductPriceChangedDomainEvent>
{
    private readonly IUserQueryService _userQueryService = userQueryService;
    private readonly IEmailService _emailService = emailService;
    private readonly IEventBus _eventBus = eventBus;

    public override async Task Handle(
        ProductPriceChangedDomainEvent domainEvent,
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
                    new ProductPriceChangedForCompanyOwnerAndManagersNotification(
                        domainEvent.Name,
                        domainEvent.PreviousPrice,
                        domainEvent.NewPrice,
                        chunk,
                        $"{domainEvent.Id}:product-price-changed-company:{index}"
                    )
            )
            .ToArray();

        await _eventBus.PublishBatchAsync(notifications, cancellationToken);
    }
}
