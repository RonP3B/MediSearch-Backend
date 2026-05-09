namespace MediSearch.Core.Application.Chat.Commands.MarkChatRoomAsRead;

[Authorize]
public sealed record MarkChatRoomAsReadCommand(Guid ChatRoomId) : ICommand;
