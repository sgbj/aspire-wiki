using Confluent.Kafka;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Caching.Distributed;
using System.Net;
using System.Text.Json;
using Wiki.Api.Models;

namespace Wiki.Api.Services;

public class PageService(CosmosClient cosmosClient, IDistributedCache cache, IProducer<string, string> kafkaProducer)
{
    private readonly Container _pages = cosmosClient.GetContainer("wiki", "pages");

    public async Task<List<Page>> GetAllPagesAsync()
    {
        // Get from cache
        var cachedValue = await cache.GetStringAsync("pages");
        if (cachedValue is { })
        {
            return JsonSerializer.Deserialize<List<Page>>(cachedValue)!;
        }

        var pages = new List<Page>();
        var feedIterator = _pages.GetItemLinqQueryable<Page>().ToFeedIterator();
        while (feedIterator.HasMoreResults)
        {
            var feedResponse = await feedIterator.ReadNextAsync();
            foreach (var page in feedResponse)
            {
                pages.Add(page);
            }
        }
        // Cache
        await cache.SetStringAsync("pages", JsonSerializer.Serialize(pages));
        return pages;
    }

    public async Task<Page?> GetPageByIdAsync(string id)
    {
        var cachedValue = await cache.GetStringAsync($"pages:{id}");
        if (cachedValue is { })
        {
            return JsonSerializer.Deserialize<Page>(cachedValue);
        }

        try
        {
            Page page = await _pages.ReadItemAsync<Page>(id, new(id), new());
            await cache.SetStringAsync($"pages:{id}", JsonSerializer.Serialize(page));
            return page;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task CreatePageAsync(Page page)
    {
        await _pages.CreateItemAsync(page);
        // Evict cache
        await cache.RemoveAsync("pages");
        await kafkaProducer.ProduceAsync(ConsumerWorker.PageUpdatedTopic, new() { Value = page.Id });
    }

    public async Task UpdatePageAsync(Page page)
    {
        await _pages.UpsertItemAsync(page);
        await cache.RemoveAsync($"pages:{page.Id}");
        await cache.RemoveAsync("pages");
        await kafkaProducer.ProduceAsync(ConsumerWorker.PageUpdatedTopic, new() { Value = page.Id });
    }

    public async Task DeletePageAsync(string id)
    {
        await _pages.DeleteItemAsync<Page>(id, new(id));
        await cache.RemoveAsync($"pages:{id}");
        await cache.RemoveAsync("pages");
        await kafkaProducer.ProduceAsync(ConsumerWorker.PageDeletedTopic, new() { Value = id });
    }
}
