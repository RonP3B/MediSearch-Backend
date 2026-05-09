using MediSearch.Core.Application.Companies.DTOs;
using MediSearch.Core.Application.Shared.DTOs;

namespace MediSearch.Infrastructure.Persistence.Companies.Queries;

internal static partial class CompanySql
{
    public const string GetCompanyDashboard =
        @$"
        -- 1) Existence check for the company
        SELECT EXISTS(
            SELECT 1
            FROM companies c
            WHERE c.id = @CompanyId
        );

        -- 2) Products count for this company
        SELECT COUNT(*) AS {nameof(CompanyDashboardDto.ProductsCount)}
        FROM products
        WHERE company_id = @CompanyId;
 
        -- 3) Users count for this company
        SELECT COUNT(*) AS {nameof(CompanyDashboardDto.UsersCount)}
        FROM users 
        WHERE company_id = @CompanyId;
 
        -- 4) Chat rooms where this company participates 
        SELECT COUNT(DISTINCT cr.id) AS {nameof(CompanyDashboardDto.ChatsCount)}
        FROM chat_rooms cr
        JOIN chat_room_participants crp ON crp.chat_room_id = cr.id
        WHERE crp.participant_id = @CompanyId 
            AND crp.participant_type_id = @CompanyAgentTypeId;
 
        -- 5) Other-type companies count
        SELECT COUNT(*) AS {nameof(CompanyDashboardDto.OtherTypeCompaniesCount)}
        FROM companies
        WHERE company_type_id <> (SELECT company_type_id FROM companies WHERE id = @CompanyId);
 
        -- 6) Top provinces with most companies of the other type
        SELECT 
            province AS {nameof(ItemCountDto.Name)},
            COUNT(*) AS {nameof(ItemCountDto.Quantity)}
        FROM companies
        WHERE company_type_id <> (SELECT company_type_id FROM companies WHERE id = @CompanyId)
        GROUP BY province
        ORDER BY {nameof(ItemCountDto.Quantity)} DESC
        LIMIT @TopProvinces;
 
        -- 7) Top products by quantity for this company
        SELECT 
            name AS {nameof(ItemCountDto.Name)},
            quantity AS {nameof(ItemCountDto.Quantity)}
        FROM products
        WHERE company_id = @CompanyId
        ORDER BY {nameof(ItemCountDto.Quantity)} DESC
        LIMIT @TopProducts;
 
        -- 8) Top classifications used by this company's products
        SELECT
            pc.name AS {nameof(ItemCountDto.Name)},
            COUNT(*) AS {nameof(ItemCountDto.Quantity)}
        FROM products p
        JOIN product_classifications pc ON p.classification_id = pc.id
        WHERE p.company_id = @CompanyId
        GROUP BY pc.name
        ORDER BY {nameof(ItemCountDto.Quantity)} DESC
        LIMIT @TopClassifications;
 
        -- 9) Top products with most interactions (comments + replies)
        SELECT 
            p.name AS {nameof(ItemCountDto.Name)},
            COUNT(c.id) AS {nameof(ItemCountDto.Quantity)}
        FROM products p
        LEFT JOIN comments c ON c.product_id = p.id
        WHERE p.company_id = @CompanyId
        GROUP BY p.name
        ORDER BY {nameof(ItemCountDto.Quantity)} DESC
        LIMIT @TopProductInteractions;
 
        -- 10) Top products with most favorites
        SELECT 
            p.name AS {nameof(ItemCountDto.Name)},
            COUNT(pf.*) AS {nameof(ItemCountDto.Quantity)}
        FROM products p
        LEFT JOIN product_favorites pf ON pf.product_id = p.id
        WHERE p.company_id = @CompanyId
        GROUP BY p.name
        ORDER BY {nameof(ItemCountDto.Quantity)} DESC
        LIMIT @TopFavoriteProducts;
        ";
}
