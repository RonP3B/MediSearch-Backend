using MediSearch.Core.Application.Chat.Commands.CreateChatRoom;
using MediSearch.Core.Application.Chat.DTOs;

namespace MediSearch.Presentation.WebApi.Chat.Endpoints;

internal sealed class CreateChatRoomEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(CreateChatRoom);
    }

    [EndpointSummary("Create Chat Room")]
    [EndpointDescription("Creates a chat room for the current agent and the selected recipient.")]
    public static async Task<Created<ChatRoomCreatedDto>> CreateChatRoom(
        CreateChatRoomRequest request,
        ISender sender,
        LinkGenerator linkGenerator,
        HttpContext httpContext
    )
    {
        var createdChatRoom = await sender.Send(
            new CreateChatRoomCommand(request.RecipientAgentId, request.RecipientAgentTypeId)
        );

        return TypedResults.Created(
            linkGenerator.GetUriByName(
                httpContext,
                nameof(GetChatRoomMessagesEndpoint.GetChatRoomMessages),
                new { chatRoomId = createdChatRoom.Id }
            ),
            createdChatRoom
        );
    }
}

internal sealed record CreateChatRoomRequest
{
    public required Guid RecipientAgentId { get; init; }
    public required int RecipientAgentTypeId { get; init; }
}
