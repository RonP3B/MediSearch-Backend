using MediSearch.Core.Application.Chat.DTOs;

namespace MediSearch.Core.Application.Chat.Ports;

public interface IChatQueryService : IQueryService
{
    Task<IReadOnlyList<ChatListItemDto>> GetChatRoomListAsync(
        Guid currentAgentId,
        int currentAgentTypeId,
        CancellationToken cancellationToken
    );

    Task<IReadOnlyList<AgentSummaryDto>> GetParticipantAgentsAsync(
        Guid chatRoomId,
        CancellationToken cancellationToken
    );

    Task<IReadOnlyList<Guid>> GetParticipantChatRoomIdsAsync(
        Guid participantId,
        int participantTypeId,
        CancellationToken cancellationToken
    );

    Task<ChatRoomParticipantNamesDto> GetChatRoomParticipantNamesAsync(
        Guid chatRoomId,
        Guid recipientCompanyId,
        CancellationToken cancellationToken
    );

    Task<ChatRoomMessagesDto?> GetChatRoomMessagesOrDefaultAsync(
        Guid chatRoomId,
        Guid currentAgentId,
        int currentAgentTypeId,
        CancellationToken cancellationToken
    );
}
