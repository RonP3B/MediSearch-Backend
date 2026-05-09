using MediSearch.Core.Application.Shared.DTOs;
using MediSearch.Core.Application.Users.DTOs;

namespace MediSearch.Infrastructure.Persistence.Users.Queries;

internal static partial class UserSql
{
    internal const string GetCompanyUsers =
        $@"
        SELECT
            u.id                 AS {nameof(UserDto.Id)},
            u.external_id        AS {nameof(UserDto.ExternalId)},
            u.first_name         AS {nameof(UserDto.FirstName)},
            u.last_name          AS {nameof(UserDto.LastName)},
            u.username           AS {nameof(UserDto.Username)},
            u.email              AS {nameof(UserDto.Email)},
            u.phone_number       AS {nameof(UserDto.PhoneNumber)},
            u.profile_image_key  AS {nameof(UserDto.ProfileImageKey)},
            u.province           AS {nameof(UserDto.Province)},
            u.municipality       AS {nameof(UserDto.Municipality)},
            u.address            AS {nameof(UserDto.Address)},
            u.company_id         AS {nameof(UserDto.CompanyId)},
            r.id                 AS {nameof(EnumerationDto.Id)},
            r.name               AS {nameof(EnumerationDto.Name)}
        FROM users u
        LEFT JOIN user_roles ur ON ur.user_id = u.id
        LEFT JOIN roles r ON r.id = ur.role_id
        WHERE u.company_id = @CompanyId
        ORDER BY u.last_name, u.first_name;
        ";
}
