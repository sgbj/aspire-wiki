using Microsoft.AspNetCore.Http.HttpResults;
using Wiki.Api.Models;
using Wiki.Api.Services;

namespace Wiki.Api.Endpoints;

public static class PageEndpoints
{
    public static void MapPageEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/pages").WithTags("Pages");

        group.MapGet("/", (PageService pageService) =>
        {
            return pageService.GetAllPagesAsync();
        })
        .WithName("GetAllPages")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<Page>, NotFound>> (string id, PageService pageService) =>
        {
            return await pageService.GetPageByIdAsync(id) is Page page ?
                TypedResults.Ok(page) :
                TypedResults.NotFound();
        })
        .WithName("GetPageById")
        .WithOpenApi();

        group.MapPut("/{id}", async (string id, Page page, PageService pageService) =>
        {
            await pageService.UpdatePageAsync(page);
            return TypedResults.NoContent();
        })
        .WithName("UpdatePage")
        .WithOpenApi();

        group.MapPost("/", async (Page page, PageService pageService) =>
        {
            await pageService.CreatePageAsync(page);
            return TypedResults.Created($"/api/pages/{page.Id}", page);
        })
        .WithName("CreatePage")
        .WithOpenApi();

        group.MapDelete("/{id}", async (string id, PageService pageService) =>
        {
            await pageService.DeletePageAsync(id);
            return TypedResults.NoContent();
        })
        .WithName("DeletePage")
        .WithOpenApi();

        group.MapGet("/search", (string query, SearchService searchService) =>
        {
            return searchService.SearchPagesAsync(query);
        })
        .WithName("SearchPages")
        .WithOpenApi();
    }
}
