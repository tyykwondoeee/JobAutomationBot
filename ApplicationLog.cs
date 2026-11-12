using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class ApplicationLog
{
    private const string LogFile = "applications.json";

    public void LogApplication(string title, string company, string location, string url, int score)
    {
        var entry = new ApplicationEntry
        {
            Title = title,
            Company = company,
            Location = location,
            Url = url,
            Score = score,
            AppliedAt = DateTime.Now
        };

        List<ApplicationEntry> entries = new();

        if (File.Exists(LogFile))
        {
            var json = File.ReadAllText(LogFile);
            entries = JsonSerializer.Deserialize<List<ApplicationEntry>>(json) ?? new();
        }

        entries.Add(entry);

        File.WriteAllText(LogFile, JsonSerializer.Serialize(entries, new JsonSerializerOptions { WriteIndented = true }));
    }
}

public class ApplicationEntry
{
    public string Title { get; set; }
    public string Company { get; set; }
    public string Location { get; set; }
    public string Url { get; set; }
    public int Score { get; set; }
    public DateTime AppliedAt { get; set; }
}
