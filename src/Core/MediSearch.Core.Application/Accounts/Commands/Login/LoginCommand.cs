using MediSearch.Core.Application.Accounts.DTOs;

namespace MediSearch.Core.Application.Accounts.Commands.Login;

public sealed record LoginCommand(string Username, string Password)
    : ICommand<AuthenticationTokensDto>;
