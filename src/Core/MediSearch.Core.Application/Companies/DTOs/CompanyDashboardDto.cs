namespace MediSearch.Core.Application.Companies.DTOs;

public sealed record CompanyDashboardDto
{
    public required long ProductsCount { get; init; }
    public required long UsersCount { get; init; }
    public required long ChatsCount { get; init; }
    public required long OtherTypeCompaniesCount { get; init; }
    public required IReadOnlyList<ItemCountDto> TopProductsByQuantity { get; init; }
    public required IReadOnlyList<ItemCountDto> TopClassifications { get; init; }
    public required IReadOnlyList<ItemCountDto> TopProductsWithMostFavorites { get; init; }
    public required IReadOnlyList<ItemCountDto> TopProductsWithMostInteractions { get; init; }
    public required IReadOnlyList<ItemCountDto> ProvincesWithMostOtherTypeCompanies { get; init; }
}
