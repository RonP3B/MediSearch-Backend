using MediSearch.Core.Application.Accounts.DTOs;

namespace MediSearch.Infrastructure.Persistence.Accounts.Queries;

internal static partial class AccountSql
{
    public const string GetUserOnboardingData =
        @$"
        SELECT
            CONCAT(u.first_name, ' ', u.last_name)               AS {nameof(UserOnboardingDto.FullName)},
            u.username                                           AS {nameof(UserOnboardingDto.Username)},
            u.email                                              AS {nameof(UserOnboardingDto.Email)},
            c.name                                               AS {nameof(UserOnboardingDto.CompanyName)},
            array_agg(r.id) FILTER (WHERE r.id IS NOT NULL)      AS {nameof(UserOnboardingDto.RoleIds)}
        FROM users u
        LEFT JOIN companies c   ON c.id  = u.company_id
        LEFT JOIN user_roles ur ON ur.user_id = u.id
        LEFT JOIN roles r       ON r.id  = ur.role_id
        WHERE u.external_id = @ExternalUserId
        GROUP BY 
            u.id,
            u.first_name,
            u.last_name,
            u.username,
            u.email,
            c.name
        ";
}
