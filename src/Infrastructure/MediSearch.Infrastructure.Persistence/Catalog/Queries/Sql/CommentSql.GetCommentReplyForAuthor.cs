using MediSearch.Core.Application.Catalog.Comments.DTOs;

namespace MediSearch.Infrastructure.Persistence.Catalog.Queries.Sql;

internal static partial class CommentSql
{
    public const string GetCommentReplyForAuthor =
        @$"
        SELECT
            parent_author.id                  AS {nameof(CommentReplyForAuthorDto.ParentCommentAuthorUserId)},
            parent_author.email               AS {nameof(CommentReplyForAuthorDto.ParentCommentAuthorEmail)},
            CONCAT(
                parent_author.first_name,
                ' ', parent_author.last_name
            )                                 AS {nameof(CommentReplyForAuthorDto.ParentCommentAuthorFullName)},
            parent_author.username            AS {nameof(CommentReplyForAuthorDto.ParentCommentAuthorUsername)},
            CASE
                WHEN reply_author_company.id IS NOT NULL
                    THEN reply_author_company.name || ' (' || reply_author.username || ')'
                ELSE reply_author.username
            END                               AS {nameof(CommentReplyForAuthorDto.ReplyAuthorName)}
        FROM comments parent_comment
        JOIN users parent_author ON parent_author.id = parent_comment.user_id
        JOIN users reply_author ON reply_author.id = @ReplyAuthorUserId
        LEFT JOIN companies reply_author_company ON reply_author_company.id = reply_author.company_id
        WHERE parent_comment.id = @ParentCommentId;
        ";
}
