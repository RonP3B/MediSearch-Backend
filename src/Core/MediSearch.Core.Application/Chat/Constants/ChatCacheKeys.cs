namespace MediSearch.Core.Application.Chat.Constants;

internal static class ChatCacheKeys
{
    public static string ParticipantAgents(Guid chatRoomId) =>
        $"chat-room:{chatRoomId}:participant-agents";
}
