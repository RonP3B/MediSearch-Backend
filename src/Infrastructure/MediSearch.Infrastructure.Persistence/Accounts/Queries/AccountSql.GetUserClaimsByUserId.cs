using MediSearch.Core.Application.Accounts.DTOs;

namespace MediSearch.Infrastructure.Persistence.Accounts.Queries;

internal static partial class AccountSql
{
    public const string GetUserClaimsByUserId =
        @$"
        SELECT
            u.id                    AS {nameof(UserClaimsDto.Id)},
            u.external_id           AS {nameof(UserClaimsDto.ExternalId)},
            u.company_id            AS {nameof(UserClaimsDto.CompanyId)},
            array_agg(r.name)       AS {nameof(UserClaimsDto.Roles)}
        FROM users u
        LEFT JOIN companies c ON c.id = u.company_id
        INNER JOIN user_roles ur ON ur.user_id = u.id
        INNER JOIN roles r ON r.id = ur.role_id
        WHERE u.id = @UserId
        GROUP BY u.id, u.company_id;
        ";
}
