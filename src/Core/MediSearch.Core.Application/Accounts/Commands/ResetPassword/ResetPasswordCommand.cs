namespace MediSearch.Core.Application.Accounts.Commands.ResetPassword;

public sealed record ResetPasswordCommand(
    string ExternalUserId,
    string ResetToken,
    string NewPassword
) : ICommand;
