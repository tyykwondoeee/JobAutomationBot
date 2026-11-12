using System;
using System.Collections.Generic;
using System.Linq;

public class HybridResumeEngine
{
    public string GenerateTailoredSummary(string jobTitle, string company, List<string> skills)
    {
        return
            $"I'm a motivated software developer with hands-on experience in {string.Join(", ", skills)}. " +
            $"I'm excited about the {jobTitle} role at {company} because it aligns with my strengths in building real-world applications, " +
            $"solving problems, and contributing to meaningful projects.";
    }

    public string GenerateSkillsMatch(string jobDescription, List<string> skills)
    {
        var matched = skills
            .Where(skill => jobDescription.ToLower().Contains(skill.ToLower()))
            .ToList();

        if (matched.Count == 0)
            matched.Add("software development fundamentals");

        return "Key skills I bring to this role include: " +
               string.Join(", ", matched) + ".";
    }

    public string GenerateMiniCoverLetter(string jobTitle, string company, List<string> skills)
    {
        return
            $"I'm excited to apply for the {jobTitle} position at {company}. " +
            $"My background in {string.Join(", ", skills)} has prepared me to contribute effectively to your team. " +
            $"I'm passionate about building scalable, reliable software and I'm eager for the opportunity to grow with {company}.";
    }
}
