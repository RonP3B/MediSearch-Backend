using MediSearch.Core.Application.Users.Commands.RegisterCompanyUser;
using MediSearch.Core.Application.Users.DTOs;

namespace MediSearch.Presentation.WebApi.Users.Endpoints;

internal sealed class RegisterCompanyUserEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(RegisterCompanyUser, "/employees");
    }

    [EndpointSummary("Register Company User")]
    [EndpointDescription("Registers a new user for the authenticated company.")]
    public static async Task<Created<UserDto>> RegisterCompanyUser(
        RegisterCompanyUserRequest request,
        ISender sender,
        LinkGenerator linkGenerator,
        HttpContext httpContext
    )
    {
        var createdUser = await sender.Send(request.Adapt<RegisterCompanyUserCommand>());

        return TypedResults.Created(
            linkGenerator.GetUriByName(
                httpContext,
                nameof(GetCompanyUsersEndpoint.GetUsersByCompanyId),
                new { companyId = createdUser.CompanyId }
            ),
            createdUser
        );
    }
}

internal sealed record RegisterCompanyUserRequest
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string PhoneNumber { get; init; }
    public required string Email { get; init; }
    public required int RoleInCompany { get; init; }
    public required string Province { get; init; }
    public required string Municipality { get; init; }
    public required string Address { get; init; }
}
