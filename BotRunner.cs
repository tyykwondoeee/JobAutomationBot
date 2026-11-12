using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class BotRunner
{
    private readonly AutoApply _autoApply = new AutoApply();
    private readonly ApplicationLog _logger = new ApplicationLog();
    private readonly DailySummary _summary = new DailySummary();
    private readonly DuplicateChecker _duplicateChecker = new DuplicateChecker();

    // Lowered threshold from 50 to 20
    private const int MinScoreToApply = 20;

    public async Task RunAsync(
        List<(AdzunaJob job, int score)> ranked,
        string resumePath,
        List<string> mySkills,
        CancellationToken token)
    {
        Console.WriteLine($"BotRunner starting with {ranked.Count} ranked jobs.");
        int considered = 0;
        int applied = 0;
        int skippedLowScore = 0;
        int skippedDuplicate = 0;

        foreach (var (job, score) in ranked)
        {
            if (token.IsCancellationRequested)
            {
                Console.WriteLine("Bot stopped safely by user.");
                break;
            }

            considered++;

            if (score < MinScoreToApply)
            {
                skippedLowScore++;
                Console.WriteLine($"SKIP (score {score}% < {MinScoreToApply}%): {job.title} | {job.company?.display_name}");
                continue;
            }

            if (_duplicateChecker.HasApplied(job.redirect_url!))
            {
                skippedDuplicate++;
                Console.WriteLine($"SKIP (duplicate): {job.title} | {job.company?.display_name}");
                continue;
            }

            Console.WriteLine($"APPLYING ({score}%): {job.title} | {job.company?.display_name} | {job.location?.display_name}");
            Console.WriteLine($"URL: {job.redirect_url}");

            bool success = await _autoApply.ApplyToJobAsync(
                job.redirect_url!,
                resumePath,
                job.company?.display_name ?? "the company",
                job.title!,
                job.description!,
                mySkills
            );

            if (success)
            {
                applied++;

                _logger.LogApplication(
                    job.title!,
                    job.company?.display_name ?? "Unknown",
                    job.location?.display_name ?? "Unknown",
                    job.redirect_url!,
                    score
                );

                Console.WriteLine("✅ Application submitted.\n");
            }
            else
            {
                Console.WriteLine("❌ Application failed.\n");
            }
        }

        Console.WriteLine("BotRunner finished.");
        Console.WriteLine($"  Considered:        {considered}");
        Console.WriteLine($"  Applied:           {applied}");
        Console.WriteLine($"  Skipped (low score): {skippedLowScore}");
        Console.WriteLine($"  Skipped (duplicate): {skippedDuplicate}");

        _summary.PrintSummary();
    }
}
