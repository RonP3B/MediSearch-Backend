using MediSearch.Core.Application.Shared.Compensations.Events;

namespace MediSearch.Infrastructure.MessageBus.Consumers;

internal sealed class CompensationEventHandlerConsumer<THandler, TCompensationEvent>(
    THandler handler,
    ILogger<CompensationEventHandlerConsumer<THandler, TCompensationEvent>> logger
) : IConsumer<TCompensationEvent>
    where THandler : class, ICompensationEventHandler<TCompensationEvent>
    where TCompensationEvent : class, ICompensationEvent
{
    public async Task Consume(ConsumeContext<TCompensationEvent> context)
    {
        logger.LogInformation(
            "Consuming compensation event {CompensationEventType} with handler {HandlerType}. MessageId: {MessageId}",
            typeof(TCompensationEvent).Name,
            typeof(THandler).Name,
            context.MessageId
        );

        await handler.Handle(context.Message, context.CancellationToken);

        logger.LogInformation(
            "Consumed compensation event {CompensationEventType} with handler {HandlerType}. MessageId: {MessageId}",
            typeof(TCompensationEvent).Name,
            typeof(THandler).Name,
            context.MessageId
        );
    }
}
