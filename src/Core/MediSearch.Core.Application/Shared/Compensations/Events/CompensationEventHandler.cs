namespace MediSearch.Core.Application.Shared.Compensations.Events;

public abstract class CompensationEventHandler<TCompensationEvent>
    : ICompensationEventHandler<TCompensationEvent>
    where TCompensationEvent : ICompensationEvent
{
    Task ICompensationEventHandler.Handle(
        ICompensationEvent compensationEvent,
        CancellationToken cancellationToken
    ) => Handle((TCompensationEvent)compensationEvent, cancellationToken);

    public abstract Task Handle(
        TCompensationEvent compensationEvent,
        CancellationToken cancellationToken
    );
}
