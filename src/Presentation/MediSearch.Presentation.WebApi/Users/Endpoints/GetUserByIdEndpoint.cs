using MediSearch.Core.Application.Users.DTOs;
using MediSearch.Core.Application.Users.Queries.GetUserById;

namespace MediSearch.Presentation.WebApi.Users.Endpoints;

internal sealed class GetUserByIdEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetUserById, "{userId:guid}");
    }

    [EndpointSummary("Get User By Id")]
    [EndpointDescription("Gets a user by id.")]
    public static async Task<Ok<UserDto>> GetUserById(Guid userId, ISender sender)
    {
        var result = await sender.Send(new GetUserByIdQuery(userId));
        return TypedResults.Ok(result);
    }
}
