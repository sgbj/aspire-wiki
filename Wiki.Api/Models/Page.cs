using Azure.Search.Documents.Indexes;
using Newtonsoft.Json;

namespace Wiki.Api.Models;

public class Page
{
    [JsonProperty("id")]
    [SimpleField(IsKey = true, IsFilterable = true)]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [SearchableField(IsSortable = true)]
    public required string Title { get; set; }

    [SearchableField]
    public required string Content { get; set; }
}
