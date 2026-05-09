using MediSearch.Core.Application.Companies.DTOs;

namespace MediSearch.Core.Application.Companies.Commands.UpdateCompany;

[Authorize(Permission = PermissionCodes.ModifyCompany)]
public sealed record UpdateCompanyCommand(
    Guid CompanyId,
    string Name,
    string CeoName,
    string Province,
    string Municipality,
    string Address,
    string Email,
    string PhoneNumber,
    FileDto? ImageFile = null,
    string? Website = null,
    string? Facebook = null,
    string? Instagram = null,
    string? Twitter = null
) : ICommand<CompanyDto>;
