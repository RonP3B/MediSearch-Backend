using MediSearch.Core.Application.Users.DTOs;
using MediSearch.Core.Application.Users.Queries.GetCompanyUsers;

namespace MediSearch.Presentation.WebApi.Users.Endpoints;

internal sealed class GetCompanyUsersEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetUsersByCompanyId, "employees/{companyId:guid}");
    }

    [EndpointSummary("Get Company Users")]
    [EndpointDescription("Gets all the employees that belong to a company.")]
    public static async Task<Ok<IReadOnlyList<UserDto>>> GetUsersByCompanyId(
        Guid companyId,
        ISender sender
    )
    {
        var result = await sender.Send(new GetCompanyUsersQuery(companyId));
        return TypedResults.Ok(result);
    }
}
