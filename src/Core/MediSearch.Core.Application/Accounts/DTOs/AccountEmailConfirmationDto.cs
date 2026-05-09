using MediSearch.Core.Application.Accounts.Enums;

namespace MediSearch.Core.Application.Accounts.DTOs;

public sealed record AccountEmailConfirmationDto
{
    public required AccountEmailConfirmationStatus ActivationStatus { get; init; }
}
