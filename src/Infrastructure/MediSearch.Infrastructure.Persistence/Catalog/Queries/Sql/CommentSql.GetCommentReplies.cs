using MediSearch.Core.Application.Catalog.Comments.DTOs;

namespace MediSearch.Infrastructure.Persistence.Catalog.Queries.Sql;

internal static partial class CommentSql
{
    public const string GetCommentReplies =
        @$"
        SELECT
            r.id                    AS {nameof(CommentDto.Id)},
            r.parent_comment_id     AS {nameof(CommentDto.ParentCommentId)},
            r.content               AS {nameof(CommentDto.Content)},
            u.id                    AS {nameof(AuthorDto.Id)},
            u.username              AS {nameof(AuthorDto.Username)},
            u.profile_image_key     AS {nameof(AuthorDto.ProfileImageKey)},
            c.id                    AS {nameof(AuthorCompanyDto.Id)},
            c.name                  AS {nameof(AuthorCompanyDto.Name)}
        FROM comments r
        JOIN users u ON u.id = r.user_id
        LEFT JOIN companies c ON c.id = u.company_id
        WHERE r.parent_comment_id = @CommentId
        ORDER BY r.created_at;
        ";
}
