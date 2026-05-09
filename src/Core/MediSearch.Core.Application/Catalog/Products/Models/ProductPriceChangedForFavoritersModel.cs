namespace MediSearch.Core.Application.Catalog.Products.Models;

public sealed record ProductPriceChangedForFavoritersModel(
    string ProductName,
    string PreviousPrice,
    string NewPrice
);
