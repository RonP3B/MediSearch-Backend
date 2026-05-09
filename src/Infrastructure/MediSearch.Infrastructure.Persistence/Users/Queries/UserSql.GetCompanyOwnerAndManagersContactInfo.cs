using MediSearch.Core.Application.Users.DTOs;

namespace MediSearch.Infrastructure.Persistence.Users.Queries;

internal static partial class UserSql
{
    internal static readonly string GetCompanyOwnerAndManagersContactInfo =
        $@"
        SELECT 
            u.email                                 AS {nameof(UserContactInfoDto.Email)},
            CONCAT(u.first_name, ' ', u.last_name)  AS {nameof(UserContactInfoDto.FullName)},
            u.username                              AS {nameof(UserContactInfoDto.Username)}
        FROM users u
        WHERE u.company_id = @CompanyId
          AND EXISTS
          (
              SELECT 1
              FROM user_roles ur
              WHERE ur.user_id = u.id
                AND ur.role_id IN (@CompanyOwnerRoleId, @CompanyManagerRoleId)
          )
        ORDER BY u.email;
        ";
}
