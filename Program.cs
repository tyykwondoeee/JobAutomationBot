using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

internal static class Program
{
    [DllImport("kernel32.dll")]
    private static extern bool AllocConsole();

    [STAThread]
    static async Task Main()
    {
        AllocConsole();

        var client = new AdzunaClient();

        var mySkills = new List<string>
        {
            "C#", ".NET", "ASP.NET", "SQL", "APIs",
            "JavaScript", "React", "Azure", "Git", "REST"
        };

        // ⭐ ALL JOB SOURCES HERE ⭐
        var sources = new List<IJobSource>
        {
            new AdzunaJobSource(client),
            new LinkedInJobSource(),
            new IndeedJobSource(),
            new DiceJobSource()
        };

        // ⭐ FETCH JOBS FROM ALL SOURCES ⭐
        var allJobs = new List<AdzunaJob>();

        foreach (var source in sources)
        {
            try
            {
                var jobs = await source.FetchJobsAsync();
                allJobs.AddRange(jobs);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching from {source.GetType().Name}: {ex.Message}");
            }
        }

        Console.WriteLine($"Total raw jobs fetched: {allJobs.Count}");

        // ⭐ DEDUPE BY URL ⭐
        var distinctJobs = allJobs
            .Where(j => !string.IsNullOrWhiteSpace(j.redirect_url))
            .GroupBy(j => j.redirect_url!)
            .Select(g => g.First())
            .ToList();

        Console.WriteLine($"Distinct jobs after dedupe: {distinctJobs.Count}");

        // ⭐ FILTERING ⭐
        var filter = new JobFilter
        {
            MinimumSalary = 50000,
            ExcludeSeniorRoles = true,
            ExcludeRecruiters = true,
            ExcludeContractRoles = true,
            ExcludeTaxClearanceJobs = true
        };

        var filtered = distinctJobs
            .Where(job => filter.PassesFilters(job))
            .ToList();

        Console.WriteLine($"Jobs after filters: {filtered.Count}");

        // ⭐ SCORING ⭐
        var matcher = new JobMatcher(mySkills);
        var ranked = matcher.RankJobs(filtered);

        Console.WriteLine("Top 10 ranked jobs:");
        foreach (var (job, score) in ranked.Take(10))
        {
            Console.WriteLine($"  {score,3}% | {job.title} | {job.location?.display_name} | {job.company?.display_name}");
        }

        // ⭐ RESUME PATH ⭐
        string resumePath = System.IO.Path.Combine(
            AppContext.BaseDirectory,
            "Ty Taylor-Resume.pdf"
        );

        Console.WriteLine($"Using resume: {resumePath}");
        Console.WriteLine($"Total ranked jobs passed to GUI: {ranked.Count}");

        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm(ranked, resumePath, mySkills));
    }
}

