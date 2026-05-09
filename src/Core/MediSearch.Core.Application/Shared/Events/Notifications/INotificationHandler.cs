namespace MediSearch.Core.Application.Shared.Events.Notifications;

public interface INotificationHandler
{
    Task Handle(INotification notification, CancellationToken cancellationToken);
}

public interface INotificationHandler<in TNotification> : INotificationHandler
    where TNotification : INotification
{
    Task Handle(TNotification notification, CancellationToken cancellationToken);
}
