using Azure.AI.OpenAI.Assistants;
using Microsoft.Extensions.Options;
using System.Text;
using Wiki.Api.Models;

namespace Wiki.Api.Services;

public record Chat(string Id);
public record ChatMessage(string Role, string Content);

public class ChatOptions
{
    public required string AssistantId { get; set; }
}

public class ChatService(IOptions<ChatOptions> options, AssistantsClient assistantClient, TimeProvider timeProvider)
{
    public async Task<Chat> CreateChatAsync()
    {
        AssistantThread thread = await assistantClient.CreateThreadAsync();
        return new(thread.Id);
    }

    public async Task CreateMessageAsync(string chatId, ChatMessage message)
    {
        await assistantClient.CreateMessageAsync(chatId, MessageRole.User, message.Content);
        ThreadRun run = await assistantClient.CreateRunAsync(chatId, new(options.Value.AssistantId));

        // Wait for it to process
        do
        {
            await Task.Delay(TimeSpan.FromMilliseconds(500), timeProvider);
            run = await assistantClient.GetRunAsync(chatId, run.Id);
        }
        while (run.Status == RunStatus.Queued || run.Status == RunStatus.InProgress);
    }

    public async IAsyncEnumerable<ChatMessage> GetMessagesAsync(string chatId)
    {
        PageableList<ThreadMessage> threadMessages = await assistantClient.GetMessagesAsync(chatId);
        foreach (var threadMessage in threadMessages.Reverse())
        {
            var sb = new StringBuilder();
            foreach (var contentItem in threadMessage.ContentItems)
            {
                if (contentItem is MessageTextContent textItem)
                {
                    sb.AppendLine(textItem.Text);
                }
            }
            yield return new(threadMessage.Role.ToString(), sb.ToString());
        }
    }

    public async Task UpdatePageAsync(Page page)
    {
        await DeletePageAsync(page.Id);
        OpenAIFile file = await assistantClient.UploadFileAsync(new BinaryData(page), OpenAIFilePurpose.Assistants, page.Id);
        await assistantClient.LinkAssistantFileAsync(options.Value.AssistantId, file.Id);
    }

    public async Task DeletePageAsync(string pageId)
    {
        var files = await assistantClient.GetFilesAsync();
        foreach (var file in files.Value)
        {
            if (file.Filename == pageId)
            {
                await assistantClient.UnlinkAssistantFileAsync(options.Value.AssistantId, file.Id);
                await assistantClient.DeleteFileAsync(file.Id);
                return;
            }
        }
    }
}
