using MediSearch.Core.Application.Chat.DTOs;

namespace MediSearch.Core.Application.Chat.Queries.GetChatRoomMessages;

[Authorize]
public sealed record GetChatRoomMessagesQuery(Guid ChatRoomId) : IQuery<ChatRoomMessagesDto>;
