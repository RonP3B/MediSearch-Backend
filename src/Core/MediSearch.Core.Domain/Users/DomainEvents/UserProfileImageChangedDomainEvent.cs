namespace MediSearch.Core.Domain.Users.DomainEvents;

public sealed class UserProfileImageChangedDomainEvent(Guid userId, string previousImageKey)
    : DomainEvent
{
    public Guid UserId { get; } = userId;
    public string PreviousImageKey { get; } = previousImageKey;
}
