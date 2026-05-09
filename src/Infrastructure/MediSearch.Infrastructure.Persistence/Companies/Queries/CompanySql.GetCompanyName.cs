namespace MediSearch.Infrastructure.Persistence.Companies.Queries;

internal static partial class CompanySql
{
    public const string GetCompanyName =
        @"
        SELECT name
        FROM companies
        WHERE id = @CompanyId;
        ";
}
