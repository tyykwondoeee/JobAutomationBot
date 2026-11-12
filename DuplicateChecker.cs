using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

public class DuplicateChecker
{
    private const string LogFile = "applications.json";

    public bool HasApplied(string jobUrl)
    {
        if (!File.Exists(LogFile))
            return false;

        var json = File.ReadAllText(LogFile);
        var entries = JsonSerializer.Deserialize<List<ApplicationEntry>>(json);

        if (entries == null)
            return false;

        return entries.Any(e => e.Url == jobUrl);
    }
}
