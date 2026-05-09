using MediSearch.Core.Application.Users.DTOs;

namespace MediSearch.Infrastructure.Persistence.Users.Queries;

internal static partial class UserSql
{
    internal const string GetCompanyUsersContactInfo =
        $@"
        SELECT 
            u.email                                AS {nameof(UserContactInfoDto.Email)},
            CONCAT(u.first_name, ' ', u.last_name) AS {nameof(UserContactInfoDto.FullName)},
            u.username                             AS {nameof(UserContactInfoDto.Username)}
        FROM users u
        WHERE u.company_id = @CompanyId
        ORDER BY u.email;
        ";
}
