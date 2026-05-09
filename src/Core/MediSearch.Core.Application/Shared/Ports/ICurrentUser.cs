namespace MediSearch.Core.Application.Shared.Ports;

public interface ICurrentUser
{
    Guid? Id { get; }
    string? ExternalId { get; }
    Guid? CompanyId { get; }
    IReadOnlyList<string> Roles { get; }
}
