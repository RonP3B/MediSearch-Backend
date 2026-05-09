using MediSearch.Core.Application.Users.DTOs;

namespace MediSearch.Core.Application.Users.Queries.GetUserById;

[Authorize]
public sealed record GetUserByIdQuery(Guid UserId) : IQuery<UserDto>;
