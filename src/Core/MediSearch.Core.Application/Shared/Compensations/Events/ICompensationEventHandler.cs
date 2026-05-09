namespace MediSearch.Core.Application.Shared.Compensations.Events;

public interface ICompensationEventHandler
{
    Task Handle(ICompensationEvent compensationEvent, CancellationToken cancellationToken);
}

public interface ICompensationEventHandler<in TCompensationEvent> : ICompensationEventHandler
    where TCompensationEvent : ICompensationEvent
{
    Task Handle(TCompensationEvent compensationEvent, CancellationToken cancellationToken);
}
