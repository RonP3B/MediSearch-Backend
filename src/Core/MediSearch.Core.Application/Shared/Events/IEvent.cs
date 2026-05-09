namespace MediSearch.Core.Application.Shared.Events;

public interface IEvent
{
    Guid Id { get; }
    DateTime OccurredAtUtc { get; }
}
