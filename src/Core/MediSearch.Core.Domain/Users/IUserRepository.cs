using MediSearch.Core.Domain.Users.ValueObjects;

namespace MediSearch.Core.Domain.Users;

public interface IUserRepository : IRepository
{
    Task<User?> GetByIdOrDefaultAsync(
        EntityId<User> id,
        CancellationToken cancellationToken = default
    );
    Task<User?> GetByUsernameOrDefaultAsync(
        Username username,
        CancellationToken cancellationToken = default
    );
    Task<bool> ExistsAsync(EntityId<User> id, CancellationToken cancellationToken = default);
    Task<bool> IsEmailTakenAsync(Email email, CancellationToken cancellationToken = default);
    Task<bool> IsUsernameTakenAsync(
        Username username,
        CancellationToken cancellationToken = default
    );
    void Add(User user);
    void Remove(User user);
}
