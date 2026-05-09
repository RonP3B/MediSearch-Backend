using MediSearch.Core.Application.Accounts.DTOs;

namespace MediSearch.Core.Application.Accounts.Commands.RefreshAccessToken;

public sealed record RefreshAccessTokenCommand(string RefreshToken)
    : ICommand<RefreshedAccessTokenDto>;
