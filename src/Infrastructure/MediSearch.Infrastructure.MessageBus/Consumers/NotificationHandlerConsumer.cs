using MediSearch.Core.Application.Shared.Events.Notifications;

namespace MediSearch.Infrastructure.MessageBus.Consumers;

internal sealed class NotificationHandlerConsumer<THandler, TNotification>(
    THandler handler,
    ILogger<NotificationHandlerConsumer<THandler, TNotification>> logger
) : IConsumer<TNotification>
    where THandler : class, INotificationHandler<TNotification>
    where TNotification : class, INotification
{
    public async Task Consume(ConsumeContext<TNotification> context)
    {
        logger.LogInformation(
            "Consuming notification {NotificationType} with handler {HandlerType}. MessageId: {MessageId}",
            typeof(TNotification).Name,
            typeof(THandler).Name,
            context.MessageId
        );

        await handler.Handle(context.Message, context.CancellationToken);

        logger.LogInformation(
            "Consumed notification {NotificationType} with handler {HandlerType}. MessageId: {MessageId}",
            typeof(TNotification).Name,
            typeof(THandler).Name,
            context.MessageId
        );
    }
}
