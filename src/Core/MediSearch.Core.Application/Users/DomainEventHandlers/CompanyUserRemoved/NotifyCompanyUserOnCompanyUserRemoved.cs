using MediSearch.Core.Application.Companies.Ports;
using MediSearch.Core.Application.Users.Notifications.CompanyUserRemoved;
using MediSearch.Core.Domain.Users.DomainEvents;

namespace MediSearch.Core.Application.Users.DomainEventHandlers.CompanyUserRemoved;

public sealed class NotifyCompanyUserOnCompanyUserRemoved(
    ICompanyQueryService companyQueryService,
    IEventBus eventBus
) : DomainEventHandler<CompanyUserRemovedDomainEvent>
{
    private readonly ICompanyQueryService _companyQueryService = companyQueryService;
    private readonly IEventBus _eventBus = eventBus;

    public override async Task Handle(
        CompanyUserRemovedDomainEvent domainEvent,
        CancellationToken cancellationToken
    )
    {
        string companyName = await _companyQueryService.GetCompanyNameAsync(
            domainEvent.CompanyId,
            cancellationToken
        );

        await _eventBus.PublishAsync(
            new CompanyUserRemovedNotification(
                domainEvent.Email,
                domainEvent.FullName,
                domainEvent.Username,
                companyName,
                $"{domainEvent.Id}:company-user-removed"
            ),
            cancellationToken
        );
    }
}
