namespace MediSearch.Core.Domain.Companies.DomainEvents;

public sealed class CompanyLogoChangedDomainEvent(
    Guid companyId,
    string companyName,
    string previousImageKey,
    string newImageKey
) : DomainEvent
{
    public Guid CompanyId { get; } = companyId;
    public string CompanyName { get; } = companyName;
    public string PreviousImageKey { get; } = previousImageKey;
    public string NewImageKey { get; } = newImageKey;
}
