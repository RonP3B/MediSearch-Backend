using MediSearch.Core.Application.Users.DTOs;

namespace MediSearch.Core.Application.Users.Ports;

public interface IUserQueryService : IQueryService
{
    Task<IReadOnlyList<UserDto>> GetCompanyUsersAsync(
        Guid companyId,
        CancellationToken cancellationToken
    );

    Task<IReadOnlyList<UserContactInfoDto>> GetCompanyUsersContactInfoAsync(
        Guid companyId,
        CancellationToken cancellationToken
    );

    Task<IReadOnlyList<UserContactInfoDto>> GetCompanyOwnerAndManagersContactInfoAsync(
        Guid companyId,
        CancellationToken cancellationToken
    );
}
