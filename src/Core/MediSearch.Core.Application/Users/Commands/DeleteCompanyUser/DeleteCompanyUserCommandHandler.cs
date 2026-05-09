using MediSearch.Core.Domain.SharedKernel.ValueObjects;
using MediSearch.Core.Domain.Users;

namespace MediSearch.Core.Application.Users.Commands.DeleteCompanyUser;

public sealed class DeleteCompanyUserCommandHandler(
    IUserRepository userRepository,
    ICurrentUser currentUser
) : ICommandHandler<DeleteCompanyUserCommand>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task Handle(DeleteCompanyUserCommand cmd, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdOrDefaultAsync(
            EntityId<User>.From(cmd.UserId),
            cancellationToken
        );

        if (user == null)
        {
            throw NotFoundException.Entity(nameof(User), nameof(User.Id), cmd.UserId);
        }

        if (_currentUser.GetAuthenticatedUserCompanyId() != user.CompanyId?.Value)
        {
            throw new ForbiddenAccessException();
        }

        user.RemoveCompanyUser();

        _userRepository.Remove(user);
    }
}
