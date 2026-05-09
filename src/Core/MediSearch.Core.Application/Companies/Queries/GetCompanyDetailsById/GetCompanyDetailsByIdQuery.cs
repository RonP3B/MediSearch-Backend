using MediSearch.Core.Application.Companies.DTOs;

namespace MediSearch.Core.Application.Companies.Queries.GetCompanyDetailsById;

public sealed record GetCompanyDetailsByIdQuery(Guid CompanyId) : IQuery<CompanyDetailsDto>;
