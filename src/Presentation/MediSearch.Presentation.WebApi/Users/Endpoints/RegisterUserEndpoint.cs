using MediSearch.Core.Application.Users.Commands.RegisterUser;
using MediSearch.Core.Application.Users.DTOs;

namespace MediSearch.Presentation.WebApi.Users.Endpoints;

internal sealed class RegisterUserEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(RegisterUser).DisableAntiforgery();
    }

    [EndpointSummary("Register User")]
    [EndpointDescription("Registers a new user client account.")]
    public static async Task<Created<UserDto>> RegisterUser(
        [FromForm] RegisterUserRequest request,
        ISender sender,
        LinkGenerator linkGenerator,
        HttpContext httpContext
    )
    {
        var createdUser = await sender.Send(request.Adapt<RegisterUserCommand>());

        return TypedResults.Created(
            linkGenerator.GetUriByName(
                httpContext,
                nameof(GetUserByIdEndpoint.GetUserById),
                new { userId = createdUser.Id }
            ),
            createdUser
        );
    }
}

internal sealed record RegisterUserRequest
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Username { get; init; }
    public required string Password { get; init; }
    public required string PhoneNumber { get; init; }
    public required string Email { get; init; }
    public required string Province { get; init; }
    public required string Municipality { get; init; }
    public required string Address { get; init; }
    public required IFormFile ProfileImageFile { get; init; }
}
