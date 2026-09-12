using MediSearch.Core.Application.Companies.DTOs;

namespace MediSearch.Infrastructure.Persistence.Companies.Queries;

internal static partial class CompanySql
{
    public const string LookupCompaniesByName =
        @$"
        SELECT
            c.id                    AS {nameof(CompanyLookupDto.Id)},
            c.name                  AS {nameof(CompanyLookupDto.Name)},
            c.image_key             AS {nameof(CompanyLookupDto.ImageKey)},
            c.company_type_id       AS {nameof(CompanyLookupDto.CompanyTypeId)}
        FROM companies c
        WHERE c.normalized_name ILIKE @Name
            AND c.id != @UserCompanyId
            AND c.company_type_id = 
                CASE 
                     WHEN (
                        SELECT company_type_id
                        FROM companies
                        WHERE id = @UserCompanyId
                    ) = @PharmacyTypeId
                    THEN @LaboratoryTypeId
                    ELSE @PharmacyTypeId
                END
        ORDER BY c.normalized_name;
        ";
}
