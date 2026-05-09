namespace MediSearch.Core.Application.Chat.DTOs;

public sealed record ChatRoomParticipantNamesDto
{
    public required string CompanyName { get; init; }
    public required string CounterpartyName { get; init; }
}
