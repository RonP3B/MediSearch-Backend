namespace MediSearch.Core.Domain.Companies.DomainEvents;

public sealed class CompanyRenamedDomainEvent(
    Guid companyId,
    string previousCompanyName,
    string newCompanyName
) : DomainEvent
{
    public Guid CompanyId { get; } = companyId;
    public string PreviousCompanyName { get; } = previousCompanyName;
    public string NewCompanyName { get; } = newCompanyName;
}
