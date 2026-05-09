namespace MediSearch.Core.Domain.Users.DomainEvents;

public sealed class CompanyUserRemovedDomainEvent(
    Guid userId,
    string fullName,
    string username,
    Guid companyId,
    string email,
    string? profileImageKey,
    string externalId
) : DomainEvent
{
    public Guid UserId { get; } = userId;
    public string FullName { get; } = fullName;
    public string Username { get; } = username;
    public Guid CompanyId { get; } = companyId;
    public string Email { get; } = email;
    public string? ProfileImageKey { get; } = profileImageKey;
    public string ExternalId { get; } = externalId;
}
