namespace MediSearch.Core.Application.Accounts.Models;

public sealed record PasswordResetRequestedModel(
    string ExternalUserId,
    string ResetToken,
    string FullName
);
