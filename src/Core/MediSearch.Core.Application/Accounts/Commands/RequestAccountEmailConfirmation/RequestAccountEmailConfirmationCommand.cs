namespace MediSearch.Core.Application.Accounts.Commands.RequestAccountEmailConfirmation;

public sealed record RequestAccountEmailConfirmationCommand(string Username) : ICommand;
