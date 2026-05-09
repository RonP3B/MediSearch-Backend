using MediSearch.Core.Application.Companies.Commands.RegisterCompanyWithOwner;
using MediSearch.Core.Application.Companies.Commands.UpdateCompany;
using MediSearch.Core.Application.Companies.DTOs;
using MediSearch.Core.Application.Shared.DTOs;
using MediSearch.Presentation.WebApi.Companies.Endpoints;

namespace MediSearch.Presentation.WebApi.Companies;

internal sealed class Mappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config
            .NewConfig<UpdateCompanyRequest, UpdateCompanyCommand>()
            .Ignore(dest => dest.CompanyId);

        config
            .NewConfig<RegisterCompanyWithOwnerRequest, RegisterCompanyWithOwnerCommand>()
            .Map(
                dest => dest.Owner,
                src => new RegisterCompanyOwnerDto
                {
                    FirstName = src.OwnerFirstName,
                    LastName = src.OwnerLastName,
                    Username = src.OwnerUsername,
                    Password = src.OwnerPassword,
                    PhoneNumber = src.OwnerPhoneNumber,
                    Email = src.OwnerEmail,
                    Province = src.OwnerProvince,
                    Municipality = src.OwnerMunicipality,
                    Address = src.OwnerAddress,
                    ProfileImageFile = src.OwnerProfileImageFile.Adapt<FileDto>(),
                }
            )
            .Map(
                dest => dest.Company,
                src => new RegisterCompanyDto
                {
                    Name = src.CompanyName,
                    CeoName = src.CompanyCeoName,
                    Province = src.CompanyProvince,
                    Municipality = src.CompanyMunicipality,
                    Address = src.CompanyAddress,
                    ImageFile = src.CompanyImageFile.Adapt<FileDto>(),
                    Email = src.CompanyEmail,
                    PhoneNumber = src.CompanyPhoneNumber,
                    TypeId = src.CompanyTypeId,
                    Website = src.CompanyWebsite,
                    Facebook = src.CompanyFacebook,
                    Instagram = src.CompanyInstagram,
                    Twitter = src.CompanyTwitter,
                }
            );
    }
}
