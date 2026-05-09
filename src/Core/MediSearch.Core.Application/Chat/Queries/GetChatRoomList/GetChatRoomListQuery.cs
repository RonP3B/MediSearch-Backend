using MediSearch.Core.Application.Chat.DTOs;

namespace MediSearch.Core.Application.Chat.Queries.GetChatRoomList;

[Authorize]
public sealed record GetChatRoomListQuery : IQuery<IReadOnlyList<ChatListItemDto>>;
