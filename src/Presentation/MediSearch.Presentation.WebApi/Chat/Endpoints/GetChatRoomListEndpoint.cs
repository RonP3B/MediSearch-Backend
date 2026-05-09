using MediSearch.Core.Application.Chat.DTOs;
using MediSearch.Core.Application.Chat.Queries.GetChatRoomList;

namespace MediSearch.Presentation.WebApi.Chat.Endpoints;

internal sealed class GetChatRoomListEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetChatRoomList);
    }

    [EndpointSummary("Get Chat Room List")]
    [EndpointDescription("Gets the chat room list for the current agent.")]
    public static async Task<Ok<IReadOnlyList<ChatListItemDto>>> GetChatRoomList(ISender sender)
    {
        var result = await sender.Send(new GetChatRoomListQuery());
        return TypedResults.Ok(result);
    }
}
