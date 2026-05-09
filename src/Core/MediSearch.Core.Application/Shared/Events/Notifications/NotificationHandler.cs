namespace MediSearch.Core.Application.Shared.Events.Notifications;

public abstract class NotificationHandler<TNotification> : INotificationHandler<TNotification>
    where TNotification : INotification
{
    Task INotificationHandler.Handle(
        INotification notification,
        CancellationToken cancellationToken
    ) => Handle((TNotification)notification, cancellationToken);

    public abstract Task Handle(TNotification notification, CancellationToken cancellationToken);
}
