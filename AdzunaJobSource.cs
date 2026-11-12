public class AdzunaJobSource : IJobSource
{
    private readonly AdzunaClient _client;

    public AdzunaJobSource(AdzunaClient client)
    {
        _client = client;
    }

    public async Task<List<AdzunaJob>> FetchJobsAsync()
    {
        var results = new List<AdzunaJob>();

        var searches = new[]
        {
            await _client.SearchJobsAsync("C# Developer", "Atlanta, GA"),
            await _client.SearchJobsAsync("Software Developer", "Atlanta, GA"),
            await _client.SearchJobsAsync("Junior Developer", "Atlanta, GA"),
            await _client.SearchJobsAsync("Backend Developer", "Atlanta, GA"),
            await _client.SearchJobsAsync("Developer", "Georgia"),
            await _client.SearchJobsAsync("C# Developer remote", "USA"),
            await _client.SearchJobsAsync("Software Engineer remote", "USA"),
            await _client.SearchJobsAsync("Backend Developer remote", "USA")
        };

        foreach (var s in searches)
            if (s?.results != null)
                results.AddRange(s.results);

        return results;
    }
}
