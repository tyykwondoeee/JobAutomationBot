using System;

public class JobFilter
{
    public int MinimumSalary { get; set; } = 0;
    public bool ExcludeSeniorRoles { get; set; } = true;
    public bool ExcludeRecruiters { get; set; } = true;
    public bool ExcludeContractRoles { get; set; } = true;
    public bool ExcludeTaxClearanceJobs { get; set; } = true;

    public bool PassesFilters(AdzunaJob job)
    {
        if (job == null)
            return false;

        string title = job.title?.ToLower() ?? "";
        string desc = job.description?.ToLower() ?? "";
        string company = job.company?.display_name?.ToLower() ?? "";

        // Salary
        if (MinimumSalary > 0 && job.salary_min.HasValue)
        {
            if (job.salary_min.Value < MinimumSalary)
                return false;
        }

        // Senior roles
        if (ExcludeSeniorRoles &&
            (title.Contains("senior") || title.Contains("lead") || title.Contains("principal") || title.Contains("staff")))
            return false;

        // Recruiters
        if (ExcludeRecruiters &&
            (company.Contains("recruit") || company.Contains("staffing") || company.Contains("agency")))
            return false;

        // Contract roles (no contract_type in your model)
        if (ExcludeContractRoles &&
            (desc.Contains("contract") || desc.Contains("1099")))
            return false;

        // Clearance
        if (ExcludeTaxClearanceJobs &&
            (desc.Contains("security clearance") || desc.Contains("clearance") || desc.Contains("tax clearance")))
            return false;

        return true;
    }
}

