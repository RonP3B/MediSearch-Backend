using MediSearch.Core.Application.Accounts.DTOs;

namespace MediSearch.Core.Application.Accounts.Commands.ConfirmAccountEmail;

public sealed record ConfirmAccountEmailCommand(string ExternalUserId, string ConfirmationToken)
    : ICommand<AccountEmailConfirmationDto>;
