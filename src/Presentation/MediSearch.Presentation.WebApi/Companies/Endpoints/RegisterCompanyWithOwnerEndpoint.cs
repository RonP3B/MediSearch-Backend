using MediSearch.Core.Application.Companies.Commands.RegisterCompanyWithOwner;
using MediSearch.Core.Application.Companies.DTOs;

namespace MediSearch.Presentation.WebApi.Companies.Endpoints;

internal sealed class RegisterCompanyWithOwnerEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(RegisterCompanyWithOwner).DisableAntiforgery();
    }

    [EndpointSummary("Register Company With Owner")]
    [EndpointDescription("Registers a company together with its owner account.")]
    public static async Task<Created<CompanyDto>> RegisterCompanyWithOwner(
        [FromForm] RegisterCompanyWithOwnerRequest request,
        ISender sender,
        LinkGenerator linkGenerator,
        HttpContext httpContext
    )
    {
        var createdCompany = await sender.Send(request.Adapt<RegisterCompanyWithOwnerCommand>());

        return TypedResults.Created(
            linkGenerator.GetUriByName(
                httpContext,
                nameof(GetCompanyByIdEndpoint.GetCompanyById),
                new { companyId = createdCompany.Id }
            ),
            createdCompany
        );
    }
}

internal sealed record RegisterCompanyWithOwnerRequest
{
    // Owner details
    public required string OwnerFirstName { get; init; }
    public required string OwnerLastName { get; init; }
    public required string OwnerUsername { get; init; }
    public required string OwnerPassword { get; init; }
    public required string OwnerPhoneNumber { get; init; }
    public required string OwnerEmail { get; init; }
    public required string OwnerProvince { get; init; }
    public required string OwnerMunicipality { get; init; }
    public required string OwnerAddress { get; init; }
    public required IFormFile OwnerProfileImageFile { get; init; }

    // Company details
    public required string CompanyName { get; init; }
    public required string CompanyCeoName { get; init; }
    public required string CompanyProvince { get; init; }
    public required string CompanyMunicipality { get; init; }
    public required string CompanyAddress { get; init; }
    public required IFormFile CompanyImageFile { get; init; }
    public required string CompanyEmail { get; init; }
    public required string CompanyPhoneNumber { get; init; }
    public required int CompanyTypeId { get; init; }
    public string? CompanyWebsite { get; init; } = null;
    public string? CompanyFacebook { get; init; } = null;
    public string? CompanyInstagram { get; init; } = null;
    public string? CompanyTwitter { get; init; } = null;
}
