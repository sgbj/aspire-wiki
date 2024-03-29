using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Models;
using Wiki.Api.Models;

namespace Wiki.Api.Services;

public class SearchService(SearchIndexClient searchIndexClient)
{
    private readonly SearchClient _searchClient = searchIndexClient.GetSearchClient("pages");

    public async IAsyncEnumerable<Page> SearchPagesAsync(string query)
    {
        SearchResults<Page> response = await _searchClient.SearchAsync<Page>(query);
        var results = response.GetResultsAsync();
        await foreach (var result in results.AsPages())
        {
            foreach (var value in result.Values)
            {
                yield return value.Document;
            }
        }
    }

    public async Task UpdatePageAsync(Page page)
    {
        await _searchClient.MergeOrUploadDocumentsAsync([page]);
    }

    public async Task DeletePageAsync(string pageId)
    {
        await _searchClient.DeleteDocumentsAsync("id", [pageId]);
    }
}
