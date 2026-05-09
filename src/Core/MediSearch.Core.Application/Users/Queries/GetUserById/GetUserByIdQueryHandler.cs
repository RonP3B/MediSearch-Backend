using MediSearch.Core.Application.Users.DTOs;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;
using MediSearch.Core.Domain.Users;

namespace MediSearch.Core.Application.Users.Queries.GetUserById;

public sealed class GetUserQueryHandler(IUserRepository userRepository)
    : IQueryHandler<GetUserByIdQuery, UserDto>
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<UserDto> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdOrDefaultAsync(
            EntityId<User>.From(query.UserId),
            cancellationToken
        );

        if (user == null)
        {
            throw NotFoundException.Entity(nameof(User), nameof(User.Id), query.UserId);
        }

        return user.Adapt<UserDto>();
    }
}
