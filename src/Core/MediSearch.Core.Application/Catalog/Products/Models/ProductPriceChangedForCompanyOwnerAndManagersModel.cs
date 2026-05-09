namespace MediSearch.Core.Application.Catalog.Products.Models;

public sealed record ProductPriceChangedForCompanyOwnerAndManagersModel(
    string ProductName,
    string PreviousPrice,
    string NewPrice
);
