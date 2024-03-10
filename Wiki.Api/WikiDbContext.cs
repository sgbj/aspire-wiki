using Microsoft.EntityFrameworkCore;

namespace Wiki.Api;

public class WikiDbContext(DbContextOptions<WikiDbContext> options) : DbContext(options)
{
    public DbSet<Page> Pages => Set<Page>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Page>()
            .HasData([
                new() { Title = "Page 1", Content = "Content 1" },
                new() { Title = "Page 2", Content = "Content 2" },
                new() { Title = "Page 3", Content = "Content 3" }
            ]);
    }
}
