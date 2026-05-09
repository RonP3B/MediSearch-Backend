using MediSearch.Core.Application.Users.Commands.UpdateUserProfile;
using MediSearch.Core.Application.Users.DTOs;

namespace MediSearch.Presentation.WebApi.Users.Endpoints;

internal sealed class UpdateUserProfileEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPut(UpdateUserProfile, "profile").DisableAntiforgery();
    }

    [EndpointSummary("Update User Profile")]
    [EndpointDescription("Updates the authenticated user's profile.")]
    public static async Task<Ok<UserDto>> UpdateUserProfile(
        [FromForm] UpdateUserProfileRequest request,
        ISender sender
    )
    {
        var updatedUser = await sender.Send(request.Adapt<UpdateUserProfileCommand>());
        return TypedResults.Ok(updatedUser);
    }
}

internal sealed record UpdateUserProfileRequest
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Province { get; init; }
    public required string Municipality { get; init; }
    public required string Address { get; init; }
    public required string PhoneNumber { get; init; }
    public IFormFile? ProfileImageFile { get; init; } = null;
}
