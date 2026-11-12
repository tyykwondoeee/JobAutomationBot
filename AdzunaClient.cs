
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

public class AdzunaClient
{
    // Load credentials securely from Windows environment variables.
    private readonly string _appId;
    private readonly string _appKey;

    private readonly HttpClient _http = new HttpClient();

    public AdzunaClient()
    {
        _appId = Environment.GetEnvironmentVariable("ADZUNA_APP_ID")
            ?? throw new InvalidOperationException(
                "ADZUNA_APP_ID environment variable is missing.");

        _appKey = Environment.GetEnvironmentVariable("ADZUNA_APP_KEY")
            ?? throw new InvalidOperationException(
                "ADZUNA_APP_KEY environment variable is missing.");
    }

    public async Task<AdzunaResponse?> SearchJobsAsync(
        string query,
        string location)
    {
        string url =
            "https://api.adzuna.com/v1/api/jobs/us/search/1" +
            $"?app_id={Uri.EscapeDataString(_appId)}" +
            $"&app_key={Uri.EscapeDataString(_appKey)}" +
            "&results_per_page=20" +
            $"&what={Uri.EscapeDataString(query)}" +
            $"&where={Uri.EscapeDataString(location)}";

        var response = await _http.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            return null;

        // Read raw bytes to avoid charset=utf8 issues.
        var bytes = await response.Content.ReadAsByteArrayAsync();
        var json = System.Text.Encoding.UTF8.GetString(bytes);

        return JsonSerializer.Deserialize<AdzunaResponse>(json);
    }
}
