using MediSearch.Core.Application.Chat.DTOs;
using MediSearch.Core.Application.Chat.Ports;
using MediSearch.Core.Domain.Chat.ChatRooms;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Core.Application.Chat.Queries.GetChatRoomMessages;

public sealed class GetChatRoomMessagesQueryHandler(
    IChatQueryService chatQueryService,
    ICurrentUser currentUser
) : IQueryHandler<GetChatRoomMessagesQuery, ChatRoomMessagesDto>
{
    private readonly IChatQueryService _chatQueryService = chatQueryService;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<ChatRoomMessagesDto> Handle(
        GetChatRoomMessagesQuery query,
        CancellationToken cancellationToken
    )
    {
        Agent currentAgent = _currentUser.ToAgent();

        var result = await _chatQueryService.GetChatRoomMessagesOrDefaultAsync(
            query.ChatRoomId,
            currentAgent.AgentId,
            currentAgent.AgentTypeId,
            cancellationToken
        );

        if (result is null)
        {
            throw NotFoundException.Entity(nameof(ChatRoom), nameof(ChatRoom.Id), query.ChatRoomId);
        }

        return result;
    }
}
