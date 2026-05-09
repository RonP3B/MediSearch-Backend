using MediSearch.Core.Application.Users.DTOs;

namespace MediSearch.Core.Application.Catalog.Products.Notifications.ProductPriceChangedForCompanyOwnerAndManagers;

public sealed record ProductPriceChangedForCompanyOwnerAndManagersNotification(
    string ProductName,
    string PreviousPrice,
    string NewPrice,
    IReadOnlyList<UserContactInfoDto> CompanyOwnerAndManagersContactInfo,
    string IdempotencyKey
) : Notification(IdempotencyKey);
