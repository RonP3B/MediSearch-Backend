using MediSearch.Core.Application.Shared.Compensations.Events.AssetDeletion;
using MediSearch.Core.Domain.Users.DomainEvents;

namespace MediSearch.Core.Application.Users.DomainEventHandlers.UserProfileImageChanged;

public sealed class CompensateAssetOnUserProfileImageChanged(IEventBus eventBus)
    : DomainEventHandler<UserProfileImageChangedDomainEvent>
{
    private readonly IEventBus _eventBus = eventBus;

    public override async Task Handle(
        UserProfileImageChangedDomainEvent domainEvent,
        CancellationToken cancellationToken
    )
    {
        await _eventBus.PublishAsync(
            new AssetDeletionCompensationEvent(domainEvent.PreviousImageKey),
            cancellationToken
        );
    }
}
