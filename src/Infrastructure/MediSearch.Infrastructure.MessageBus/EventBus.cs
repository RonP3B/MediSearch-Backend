using MediSearch.Core.Application.Shared.Events;
using MediSearch.Core.Application.Shared.Ports;

namespace MediSearch.Infrastructure.MessageBus;

internal sealed class EventBus(IBus bus) : IEventBus
{
    private readonly IBus _bus = bus;

    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IEvent => _bus.Publish(@event, cancellationToken);

    public Task PublishBatchAsync<TEvent>(
        IEnumerable<TEvent> events,
        CancellationToken cancellationToken = default
    )
        where TEvent : class, IEvent => _bus.PublishBatch(events, cancellationToken);
}
