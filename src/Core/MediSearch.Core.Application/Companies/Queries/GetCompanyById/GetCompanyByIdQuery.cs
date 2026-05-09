using MediSearch.Core.Application.Companies.DTOs;

namespace MediSearch.Core.Application.Companies.Queries.GetCompanyById;

public sealed record GetCompanyByIdQuery(Guid CompanyId) : IQuery<CompanyDto>;
