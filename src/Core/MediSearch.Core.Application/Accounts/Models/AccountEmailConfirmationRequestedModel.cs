namespace MediSearch.Core.Application.Accounts.Models;

public sealed record AccountEmailConfirmationRequestedModel(
    string ExternalUserId,
    string ConfirmationToken,
    string FullName
);
