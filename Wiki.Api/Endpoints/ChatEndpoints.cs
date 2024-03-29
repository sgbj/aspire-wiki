using Wiki.Api.Services;

namespace Wiki.Api.Endpoints;

public static class ChatEndpoints
{
    public static void MapChatEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/chat").WithTags("Chat");

        group.MapPost("/", async (ChatService chatService) =>
        {
            return await chatService.CreateChatAsync();
        })
        .WithName("CreateChat")
        .WithOpenApi();

        group.MapPost("/{id}/messages", async (string id, ChatMessage message, ChatService chatService) =>
        {
            await chatService.CreateMessageAsync(id, message);
        })
        .WithName("CreateMessage")
        .WithOpenApi();

        group.MapGet("/{id}/messages", (string id, ChatService chatService) =>
        {
            return chatService.GetMessagesAsync(id);
        })
        .WithName("GetMessages")
        .WithOpenApi();
    }
}
