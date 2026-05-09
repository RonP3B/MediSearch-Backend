using MediSearch.Core.Application.Companies.DTOs;

namespace MediSearch.Core.Application.Companies.Commands.RegisterCompanyWithOwner;

public sealed record RegisterCompanyWithOwnerCommand(
    RegisterCompanyOwnerDto Owner,
    RegisterCompanyDto Company
) : ICommand<CompanyDto>;
