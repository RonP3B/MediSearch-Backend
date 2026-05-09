namespace MediSearch.Core.Application.Shared.Events.Notifications;

public abstract record Notification : INotification
{
    public Guid Id { get; } = Guid.CreateVersion7();
    public DateTime OccurredAtUtc { get; } = DateTime.UtcNow;
    public string IdempotencyKey { get; }

    protected Notification(string? idempotencyKey = null)
    {
        IdempotencyKey = idempotencyKey ?? Id.ToString();
    }
}
