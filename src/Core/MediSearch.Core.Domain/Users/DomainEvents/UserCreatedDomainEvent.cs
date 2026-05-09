namespace MediSearch.Core.Domain.Users.DomainEvents;

public sealed class UserCreatedDomainEvent(Guid userId, string username) : DomainEvent
{
    public Guid UserId { get; } = userId;
    public string Username { get; } = username;
}
