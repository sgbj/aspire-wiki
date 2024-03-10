using Confluent.Kafka;

namespace Wiki.Api;

public class ConsumerWorker(IConsumer<string, string> consumer, ILogger<ConsumerWorker> logger) : BackgroundService
{
    // https://blog.stephencleary.com/2020/05/backgroundservice-gotcha-startup.html
    protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.Run(() =>
    {
        consumer.Subscribe(["page-updated"]);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = consumer.Consume(stoppingToken);

                switch (result.Topic)
                {
                    case "page-updated":
                        logger.LogInformation("Page updated.");
                        break;

                    default:
                        logger.LogWarning("Unhandled topic {Topic}.", result.Topic);
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
