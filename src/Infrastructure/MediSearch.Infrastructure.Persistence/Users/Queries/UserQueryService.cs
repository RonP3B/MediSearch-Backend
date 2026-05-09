using MediSearch.Core.Application.Shared.DTOs;
using MediSearch.Core.Application.Users.DTOs;
using MediSearch.Core.Application.Users.Ports;
using MediSearch.Core.Domain.Users.AccessControl;

namespace MediSearch.Infrastructure.Persistence.Users.Queries;

internal sealed class CompanyQuerySaervice(IDbConnectionFactory db) : IUserQueryService
{
    private readonly IDbConnectionFactory _db = db;

    public async Task<IReadOnlyList<UserDto>> GetCompanyUsersAsync(
        Guid companyId,
        CancellationToken cancellationToken
    )
    {
        await using var conn = await _db.OpenConnectionAsync();

        var lookup = new Dictionary<Guid, (UserDto User, List<EnumerationDto> Roles)>();

        await conn.QueryAsync<UserDto, EnumerationDto, UserDto>(
            UserSql.GetCompanyUsers,
            (user, role) =>
            {
                if (!lookup.TryGetValue(user.Id, out var entry))
                {
                    entry = (user, []);
                    lookup.Add(user.Id, entry);
                }

                if (role != null && role.Id != default)
                {
                    entry.Roles.Add(role);
                }

                return user;
            },
            new { CompanyId = companyId },
            splitOn: nameof(EnumerationDto.Id)
        );

        return [.. lookup.Values.Select(e => e.User with { Roles = e.Roles })];
    }

    public async Task<IReadOnlyList<UserContactInfoDto>> GetCompanyUsersContactInfoAsync(
        Guid companyId,
        CancellationToken cancellationToken
    )
    {
        await using var conn = await _db.OpenConnectionAsync();

        var contactInfo = await conn.QueryAsync<UserContactInfoDto>(
            UserSql.GetCompanyUsersContactInfo,
            new { CompanyId = companyId }
        );

        return [.. contactInfo];
    }

    public async Task<IReadOnlyList<UserContactInfoDto>> GetCompanyOwnerAndManagersContactInfoAsync(
        Guid companyId,
        CancellationToken cancellationToken
    )
    {
        await using var conn = await _db.OpenConnectionAsync();

        var contactInfo = await conn.QueryAsync<UserContactInfoDto>(
            UserSql.GetCompanyOwnerAndManagersContactInfo,
            new
            {
                CompanyId = companyId,
                CompanyOwnerRoleId = Role.CompanyOwner.Id,
                CompanyManagerRoleId = Role.CompanyManager.Id,
            }
        );

        return [.. contactInfo];
    }
}
