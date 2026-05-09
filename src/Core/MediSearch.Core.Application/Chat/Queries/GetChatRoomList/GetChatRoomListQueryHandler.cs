using MediSearch.Core.Application.Chat.DTOs;
using MediSearch.Core.Application.Chat.Ports;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Core.Application.Chat.Queries.GetChatRoomList;

public sealed class GetChatRoomListQueryHandler(
    IChatQueryService chatQueryService,
    ICurrentUser currentUser
) : IQueryHandler<GetChatRoomListQuery, IReadOnlyList<ChatListItemDto>>
{
    private readonly IChatQueryService _chatQueryService = chatQueryService;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<IReadOnlyList<ChatListItemDto>> Handle(
        GetChatRoomListQuery query,
        CancellationToken cancellationToken
    )
    {
        Agent currentAgent = _currentUser.ToAgent();

        return await _chatQueryService.GetChatRoomListAsync(
            currentAgent.AgentId,
            currentAgent.AgentTypeId,
            cancellationToken
        );
    }
}
