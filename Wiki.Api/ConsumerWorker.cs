using Confluent.Kafka;
using Wiki.Api.Services;

namespace Wiki.Api;

public class ConsumerWorker(PageService pageService, SearchService searchService, ChatService chatService, 
    IConsumer<string, string> kafkaConsumer, ILogger<ConsumerWorker> logger) : BackgroundService
{
    public const string 
        PageUpdatedTopic = "page-updated",
        PageDeletedTopic = "page-deleted";

    // https://blog.stephencleary.com/2020/05/backgroundservice-gotcha-startup.html
    protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.Run(async () =>
    {
        kafkaConsumer.Subscribe([PageUpdatedTopic, PageDeletedTopic]);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = kafkaConsumer.Consume(stoppingToken);

                switch (result.Topic)
                {
                    case PageUpdatedTopic:
                        var page = await pageService.GetPageByIdAsync(result.Message.Value);
                        if (page is { })
                        {
                            await searchService.UpdatePageAsync(page);
                            await chatService.UpdatePageAsync(page);
                        }
                        break;

                    case PageDeletedTopic:
                        await searchService.DeletePageAsync(result.Message.Value);
                        await chatService.DeletePageAsync(result.Message.Value);
                        break;

                    default:
                        logger.LogWarning("Unhandled topic {topic}.", result.Topic);
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error in message consumer.");
            }
        }
    });
}
