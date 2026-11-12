using System;
using System.Collections.Generic;
using System.Linq;

public class JobMatcher
{
    private readonly List<string> _skills;

    public JobMatcher(List<string> skills)
    {
        _skills = skills
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s.ToLower())
            .ToList();
    }

    public int ScoreJob(AdzunaJob job)
    {
        if (job == null)
            return 0;

        string title = job.title?.ToLower() ?? "";
        string desc = job.description?.ToLower() ?? "";
        string text = $"{title} {desc}";
        string company = job.company?.display_name?.ToLower() ?? "";
        string location = job.location?.display_name?.ToLower() ?? "";

        double score = 0;

        // Skill matches
        int skillHits = _skills.Count(skill => text.Contains(skill));
        score += Math.Min(skillHits * 6, 60);

        // Title relevance
        if (title.Contains("developer")) score += 10;
        if (title.Contains("software")) score += 7;
        if (title.Contains("engineer")) score += 7;
        if (title.Contains("backend")) score += 5;
        if (title.Contains(".net") || title.Contains("c#")) score += 8;

        // Junior boost
        if (title.Contains("junior") || title.Contains("entry")) score += 8;

        // Senior penalty
        if (title.Contains("senior") || title.Contains("sr")) score -= 15;
        if (title.Contains("lead") || title.Contains("principal") || title.Contains("staff")) score -= 20;

        // Remote/hybrid
        if (text.Contains("remote")) score += 10;
        if (text.Contains("hybrid")) score += 7;

        // Atlanta preference
        if (location.Contains("atlanta")) score += 10;
        if (location.Contains("ga") || location.Contains("georgia")) score += 5;

        // Recruiter penalty
        if (company.Contains("recruit") || company.Contains("staffing") || company.Contains("agency"))
            score -= 10;

        // Generic penalty
        if (!title.Contains("developer") &&
            !title.Contains("engineer") &&
            !title.Contains("software") &&
            skillHits == 0)
        {
            score -= 10;
        }

        return Math.Clamp((int)Math.Round(score), 0, 100);
    }

    public List<(AdzunaJob job, int score)> RankJobs(List<AdzunaJob> jobs)
    {
        return jobs
            .Select(j => (job: j, score: ScoreJob(j)))
            .OrderByDescending(x => x.score)
            .ToList();
    }
}

