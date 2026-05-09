namespace MediSearch.Core.Application.Shared.DTOs;

public sealed record AgentSummaryDto
{
    public required Guid AgentId { get; init; }
    public required int AgentTypeId { get; init; }
    public required string Name { get; init; }
    public required string ImageKey { get; init; }
}
