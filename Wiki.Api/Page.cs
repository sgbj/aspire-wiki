namespace Wiki.Api;

public class Page
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string Title { get; set; }
    public required string Content { get; set; }
}
