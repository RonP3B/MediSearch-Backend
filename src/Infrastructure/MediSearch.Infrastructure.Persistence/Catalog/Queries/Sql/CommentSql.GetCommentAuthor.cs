using MediSearch.Core.Application.Catalog.Comments.DTOs;

namespace MediSearch.Infrastructure.Persistence.Catalog.Queries.Sql;

internal static partial class CommentSql
{
    public const string GetCommentAuthor =
        @$"
        SELECT
            u.id                     AS {nameof(AuthorDto.Id)},
            u.username               AS {nameof(AuthorDto.Username)},
            u.profile_image_key      AS {nameof(AuthorDto.ProfileImageKey)},
            c.id                     AS {nameof(AuthorCompanyDto.Id)},
            c.name                   AS {nameof(AuthorCompanyDto.Name)}
        FROM users u
        LEFT JOIN companies c ON c.id = u.company_id
        WHERE u.id = @UserId;
        ";
}
