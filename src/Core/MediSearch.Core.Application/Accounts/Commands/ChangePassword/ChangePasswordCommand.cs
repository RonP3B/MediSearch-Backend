namespace MediSearch.Core.Application.Accounts.Commands.ChangePassword;

[Authorize]
public sealed record ChangePasswordCommand(string CurrentPassword, string NewPassword) : ICommand;
