using Wiki.Api.Services;

namespace Wiki.Api.Endpoints;

public record Blob(string Location);

public static class BlobEndpoints
{
    public static void MapBlobEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/blobs").WithTags("Blobs");

        group.MapGet("/{id}", async (string id, BlobService blobService) =>
        {
            var stream = await blobService.DownloadBlobAsync(id);
            return TypedResults.File(stream);
        })
        .WithName("DownloadBlob")
        .WithOpenApi();

        group.MapPost("/", async (IFormFile file, BlobService blobService) =>
        {
            await using var stream = file.OpenReadStream();
            await blobService.UploadBlobAsync(file.FileName, stream);
            return TypedResults.Ok(new Blob($"/api/blobs/{file.FileName}"));
        })
        .WithName("UploadBlob")
        .WithOpenApi()
        .DisableAntiforgery();
    }
}
