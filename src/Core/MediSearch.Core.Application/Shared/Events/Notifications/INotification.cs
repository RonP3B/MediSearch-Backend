namespace MediSearch.Core.Application.Shared.Events.Notifications;

public interface INotification : IEvent
{
    public string IdempotencyKey { get; }
}
