using MediSearch.Core.Application.Catalog.Comments.DTOs;

namespace MediSearch.Infrastructure.Persistence.Catalog.Queries.Sql;

internal static partial class CommentSql
{
    public const string GetCommentPostedForCompany =
        @$"
        SELECT
            seller_company.id            AS {nameof(CommentPostedForCompanyDto.SellerCompanyId)},
            seller_company.name          AS {nameof(CommentPostedForCompanyDto.SellerCompanyName)},
            author.company_id            AS {nameof(CommentPostedForCompanyDto.AuthorCompanyId)},
            product.name                 AS {nameof(CommentPostedForCompanyDto.ProductName)},
            CASE
                WHEN author_company.id IS NOT NULL
                    THEN CONCAT(author.username, ' (', author_company.name, ')')
                ELSE author.username
            END                          AS {nameof(CommentPostedForCompanyDto.CommentAuthorName)}
        FROM products product
        -- Resolve the seller company through the product captured in the event payload.
        JOIN companies seller_company ON seller_company.id = product.company_id
        JOIN users author ON author.id = @AuthorUserId
        LEFT JOIN companies author_company ON author_company.id = author.company_id
        WHERE product.id = @ProductId;
        ";
}
