using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Wiki.Api;

public static class PageEndpoints
{
    public static void MapPageEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Pages").WithTags(nameof(Page));

        group.MapGet("/", async (WikiDbContext db) =>
        {
            return await db.Pages.ToListAsync();
        })
        .WithName("GetAllPages")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<Page>, NotFound>> (string id, WikiDbContext db) =>
        {
            return await db.Pages.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is Page model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetPageById")
        .WithOpenApi();

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (string id, Page page, WikiDbContext db) =>
        {
            var affected = await db.Pages
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.Title, page.Title)
                    .SetProperty(m => m.Content, page.Content)
                    );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdatePage")
        .WithOpenApi();

        group.MapPost("/", async (Page page, WikiDbContext db) =>
        {
            db.Pages.Add(page);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/Page/{page.Id}", page);
        })
        .WithName("CreatePage")
        .WithOpenApi();

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (string id, WikiDbContext db) =>
        {
            var affected = await db.Pages
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeletePage")
        .WithOpenApi();
    }
}
