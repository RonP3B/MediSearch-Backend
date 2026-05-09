namespace MediSearch.Core.Application.Accounts.Commands.RequestPasswordReset;

public sealed record RequestPasswordResetCommand(string Username) : ICommand;
