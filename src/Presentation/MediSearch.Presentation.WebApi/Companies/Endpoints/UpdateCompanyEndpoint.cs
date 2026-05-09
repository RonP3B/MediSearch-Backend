using MediSearch.Core.Application.Companies.Commands.UpdateCompany;
using MediSearch.Core.Application.Companies.DTOs;

namespace MediSearch.Presentation.WebApi.Companies.Endpoints;

internal sealed class UpdateCompanyEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPut(UpdateCompany, "{companyId:guid}").DisableAntiforgery();
    }

    [EndpointSummary("Update Company")]
    [EndpointDescription("Updates the authenticated company")]
    public static async Task<Ok<CompanyDto>> UpdateCompany(
        Guid companyId,
        [FromForm] UpdateCompanyRequest request,
        ISender sender
    )
    {
        var updatedCompany = await sender.Send(
            request.Adapt<UpdateCompanyCommand>() with
            {
                CompanyId = companyId,
            }
        );

        return TypedResults.Ok(updatedCompany);
    }
}

internal sealed record UpdateCompanyRequest
{
    public required string Name { get; init; }
    public required string CeoName { get; init; }
    public required string Province { get; init; }
    public required string Municipality { get; init; }
    public required string Address { get; init; }
    public required string Email { get; init; }
    public required string PhoneNumber { get; init; }
    public IFormFile? ImageFile { get; init; } = null;
    public string? Website { get; init; } = null;
    public string? Facebook { get; init; } = null;
    public string? Instagram { get; init; } = null;
    public string? Twitter { get; init; } = null;
}
