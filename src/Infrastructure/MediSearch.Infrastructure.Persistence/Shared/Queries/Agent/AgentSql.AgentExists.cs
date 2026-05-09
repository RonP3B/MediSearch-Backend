namespace MediSearch.Infrastructure.Persistence.Shared.Queries.Agent;

internal static partial class AgentSql
{
    public const string AgentExists =
        @$"
        SELECT EXISTS (
            SELECT 1
            FROM (SELECT @AgentId AS id, @AgentTypeId AS type_id) a
            LEFT JOIN users u ON u.id = a.id AND a.type_id = @UserAgentTypeId
            LEFT JOIN companies c ON c.id = a.id AND a.type_id = @CompanyAgentTypeId
            WHERE u.id IS NOT NULL OR c.id IS NOT NULL
        )
        ";
}
