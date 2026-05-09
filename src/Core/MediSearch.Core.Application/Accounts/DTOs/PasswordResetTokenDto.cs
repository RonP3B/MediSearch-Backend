namespace MediSearch.Core.Application.Accounts.DTOs;

public sealed record PasswordResetTokenDto
{
    public required string ResetToken { get; init; }
}
