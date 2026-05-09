using MediSearch.Core.Application.Shared.Compensations.Events.AssetDeletion;
using MediSearch.Core.Application.Shared.Compensations.Events.ExternalUserDeletion;
using MediSearch.Core.Domain.Users.DomainEvents;

namespace MediSearch.Core.Application.Users.DomainEventHandlers.CompanyUserRemoved;

public sealed class CompensationOnCompanyUserRemoved(IEventBus eventBus)
    : DomainEventHandler<CompanyUserRemovedDomainEvent>
{
    private readonly IEventBus _eventBus = eventBus;

    public override async Task Handle(
        CompanyUserRemovedDomainEvent domainEvent,
        CancellationToken cancellationToken
    )
    {
        await _eventBus.PublishAsync(
            new ExternalUserDeletionCompensationEvent(domainEvent.ExternalId),
            cancellationToken
        );

        if (domainEvent.ProfileImageKey is not null)
        {
            await _eventBus.PublishAsync(
                new AssetDeletionCompensationEvent(domainEvent.ProfileImageKey),
                cancellationToken
            );
        }
    }
}
