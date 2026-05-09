namespace MediSearch.Infrastructure.Persistence.Accounts.Queries;

internal static partial class AccountSql
{
    public const string GetExternalUserIdByUsername =
        "SELECT external_id FROM users WHERE normalized_username = @Username;";
}
