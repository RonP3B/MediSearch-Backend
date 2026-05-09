namespace MediSearch.Core.Domain.Chat.ChatRooms;

public interface IChatRoomRepository : IRepository
{
    Task<ChatRoom?> GetByIdOrDefaultAsync(
        EntityId<ChatRoom> id,
        CancellationToken cancellationToken
    );
    Task<bool> ExistsByParticipantAgentsAsync(
        Agent participantAgentA,
        Agent participantAgentB,
        CancellationToken cancellationToken
    );
    Task<bool> IsChatAllowedAsync(
        Agent participantAgentA,
        Agent participantAgentB,
        CancellationToken cancellationToken
    );
    void Add(ChatRoom chatRoom);
}
