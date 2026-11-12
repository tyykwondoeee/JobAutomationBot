using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class DailySummary
{
    private const string LogFile = "applications.json";

    public void PrintSummary()
    {
        if (!File.Exists(LogFile))
        {
            Console.WriteLine("No applications logged today.");
            return;
        }

        var json = File.ReadAllText(LogFile);
        var entries = JsonSerializer.Deserialize<List<ApplicationEntry>>(json);

        if (entries == null || entries.Count == 0)
        {
            Console.WriteLine("No applications logged.");
            return;
        }

        Console.WriteLine("\n===== DAILY APPLICATION SUMMARY =====\n");

        foreach (var entry in entries)
        {
            Console.WriteLine($"Title: {entry.Title}");
            Console.WriteLine($"Company: {entry.Company}");
            Console.WriteLine($"Location: {entry.Location}");
            Console.WriteLine($"Match Score: {entry.Score}");
            Console.WriteLine($"Applied At: {entry.AppliedAt}");
            Console.WriteLine($"Link: {entry.Url}");
            Console.WriteLine("-----------------------------------");
        }

        Console.WriteLine($"Total Applications: {entries.Count}");
    }
}
