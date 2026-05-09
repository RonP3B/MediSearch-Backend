namespace MediSearch.Core.Application.Accounts.DTOs;

public sealed record AccountEmailConfirmationTokenDto
{
    public required string ConfirmationToken { get; init; }
}
