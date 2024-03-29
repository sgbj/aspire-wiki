using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace Wiki.Api.Services;

public class BlobService(BlobServiceClient blobServiceClient, IDistributedCache cache)
{
    private readonly BlobContainerClient _blobContainerClient = blobServiceClient.GetBlobContainerClient("pages");

    public async Task<byte[]> DownloadBlobAsync(string id)
    {
        var cachedValue = await cache.GetAsync($"blobs:{id}");
        if (cachedValue is { })
        {
            return cachedValue;
        }

        var blobClient = _blobContainerClient.GetBlobClient(id);
        BlobDownloadResult result = await blobClient.DownloadContentAsync();
        var data = result.Content.ToArray();
        await cache.SetAsync($"blobs:{id}", data);
        return data;
    }

    public async Task UploadBlobAsync(string id, Stream stream)
    {
        await _blobContainerClient.UploadBlobAsync(id, stream);
        await cache.RemoveAsync($"blobs:{id}");
    }
}
