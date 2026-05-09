namespace MediSearch.Core.Domain.Companies.DomainEvents;

public sealed class CompanyCreatedDomainEvent(Guid companyId) : DomainEvent
{
    public Guid CompanyId { get; } = companyId;
}
