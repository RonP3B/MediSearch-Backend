using MediSearch.Core.Application.Users.Commands.DeleteCompanyUser;

namespace MediSearch.Presentation.WebApi.Users.Endpoints;

internal sealed class DeleteCompanyUserEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapDelete(DeleteCompanyUser, "/employees/{userId:guid}");
    }

    [EndpointSummary("Delete Company User")]
    [EndpointDescription("Deletes an employee from the authenticated company.")]
    public static async Task<NoContent> DeleteCompanyUser(Guid userId, ISender sender)
    {
        await sender.Send(new DeleteCompanyUserCommand(userId));
        return TypedResults.NoContent();
    }
}
