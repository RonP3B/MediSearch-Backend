using MediSearch.Core.Application.Users.DTOs;

namespace MediSearch.Infrastructure.Persistence.Accounts.Queries;

internal static partial class AccountSql
{
    public const string GetUserContactInfoByExternalUserId =
        @$"
        SELECT
            u.email                                AS {nameof(UserContactInfoDto.Email)},
            CONCAT(u.first_name, ' ', u.last_name) AS {nameof(UserContactInfoDto.FullName)},
            u.username                             AS {nameof(UserContactInfoDto.Username)}
        FROM users u
        WHERE u.external_id = @ExternalUserId
        ";
}
