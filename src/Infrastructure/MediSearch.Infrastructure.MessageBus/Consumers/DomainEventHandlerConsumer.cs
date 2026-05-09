using MediSearch.Core.Application.Shared.Events.DomainEvents;
using MediSearch.Core.Domain.SharedKernel.Interfaces;

namespace MediSearch.Infrastructure.MessageBus.Consumers;

internal sealed class DomainEventHandlerConsumer<THandler, TEvent>(
    THandler handler,
    ILogger<DomainEventHandlerConsumer<THandler, TEvent>> logger
) : IConsumer<TEvent>
    where THandler : class, IDomainEventHandler<TEvent>
    where TEvent : class, IDomainEvent
{
    public async Task Consume(ConsumeContext<TEvent> context)
    {
        logger.LogInformation(
            "Consuming domain event {EventType} with handler {HandlerType}. MessageId: {MessageId}",
            typeof(TEvent).Name,
            typeof(THandler).Name,
            context.MessageId
        );

        await handler.Handle(context.Message, context.CancellationToken);

        logger.LogInformation(
            "Consumed domain event {EventType} with handler {HandlerType}. MessageId: {MessageId}",
            typeof(TEvent).Name,
            typeof(THandler).Name,
            context.MessageId
        );
    }
}
