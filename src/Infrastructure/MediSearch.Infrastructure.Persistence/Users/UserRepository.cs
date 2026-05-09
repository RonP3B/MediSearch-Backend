using MediSearch.Core.Domain.SharedKernel.ValueObjects;
using MediSearch.Core.Domain.Users;
using MediSearch.Core.Domain.Users.ValueObjects;

namespace MediSearch.Infrastructure.Persistence.Users;

internal sealed class UserRepository(AppDbContext dbContext) : IUserRepository
{
    public async Task<User?> GetByIdOrDefaultAsync(
        EntityId<User> id,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext
            .DomainUsers.Include(u => u.Roles)
            .SingleOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetByUsernameOrDefaultAsync(
        Username username,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext
            .DomainUsers.Include(u => u.Roles)
            .SingleOrDefaultAsync(
                u => u.Username.Normalized == username.Normalized,
                cancellationToken
            );
    }

    public async Task<bool> ExistsAsync(
        EntityId<User> id,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext.DomainUsers.AnyAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<bool> IsEmailTakenAsync(
        Email email,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext.DomainUsers.AnyAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<bool> IsUsernameTakenAsync(
        Username username,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext.DomainUsers.AnyAsync(
            u => u.Username.Normalized == username.Normalized,
            cancellationToken
        );
    }

    public void Add(User user)
    {
        foreach (var role in user.Roles)
        {
            dbContext.Attach(role);
        }

        dbContext.DomainUsers.Add(user);
    }

    public void Remove(User user)
    {
        dbContext.DomainUsers.Remove(user);
    }
}
