using MediSearch.Core.Application.Chat.DTOs;
using MediSearch.Core.Application.Chat.Queries.GetChatRoomMessages;

namespace MediSearch.Presentation.WebApi.Chat.Endpoints;

internal sealed class GetChatRoomMessagesEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetChatRoomMessages, "{chatRoomId:guid}/messages");
    }

    [EndpointSummary("Get Chat Room Messages")]
    [EndpointDescription("Gets all messages for a chat room.")]
    public static async Task<Ok<ChatRoomMessagesDto>> GetChatRoomMessages(
        Guid chatRoomId,
        ISender sender
    )
    {
        var result = await sender.Send(new GetChatRoomMessagesQuery(chatRoomId));
        return TypedResults.Ok(result);
    }
}
