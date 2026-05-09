namespace MediSearch.Core.Application.Shared.Compensations.Events;

public abstract record CompensationEvent : ICompensationEvent
{
    public Guid Id { get; } = Guid.CreateVersion7();
    public DateTime OccurredAtUtc { get; } = DateTime.UtcNow;
}
