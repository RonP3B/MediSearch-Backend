using MediSearch.Core.Application.Chat.Commands.MarkChatRoomAsRead;

namespace MediSearch.Presentation.WebApi.Chat.Endpoints;

internal sealed class MarkChatRoomAsReadEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPatch(MarkChatRoomAsRead, "{chatRoomId:guid}/read-status");
    }

    [EndpointSummary("Mark Chat Room As Read")]
    [EndpointDescription(
        "Marks all unread messages in the specified chat room as read for the current authenticated user."
    )]
    public static async Task<NoContent> MarkChatRoomAsRead(Guid chatRoomId, ISender sender)
    {
        await sender.Send(new MarkChatRoomAsReadCommand(chatRoomId));
        return TypedResults.NoContent();
    }
}
