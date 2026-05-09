namespace MediSearch.Core.Application.Accounts.Models;

public sealed record CompanyUserWelcomeModel(
    string FullName,
    string CompanyName,
    string Username,
    string ExternalUserId,
    string PasswordResetToken
);
