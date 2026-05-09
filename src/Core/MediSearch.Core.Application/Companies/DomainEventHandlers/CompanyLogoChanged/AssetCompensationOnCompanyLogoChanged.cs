using MediSearch.Core.Application.Shared.Compensations.Events.AssetDeletion;
using MediSearch.Core.Domain.Companies.DomainEvents;

namespace MediSearch.Core.Application.Companies.DomainEventHandlers.CompanyLogoChanged;

public sealed class AssetCompensationOnCompanyLogoChanged(IEventBus eventBus)
    : DomainEventHandler<CompanyLogoChangedDomainEvent>
{
    private readonly IEventBus _eventBus = eventBus;

    public override async Task Handle(
        CompanyLogoChangedDomainEvent domainEvent,
        CancellationToken cancellationToken
    )
    {
        await _eventBus.PublishAsync(
            new AssetDeletionCompensationEvent(domainEvent.PreviousImageKey),
            cancellationToken
        );
    }
}
