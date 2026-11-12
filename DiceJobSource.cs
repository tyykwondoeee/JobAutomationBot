using Microsoft.Playwright;

public class DiceJobSource : IJobSource
{
    public async Task<List<AdzunaJob>> FetchJobsAsync()
    {
        var jobs = new List<AdzunaJob>();

        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new() { Headless = true });
        var page = await browser.NewPageAsync();

        await page.GotoAsync("https://www.dice.com/jobs?q=C%23+Developer&location=Atlanta%2C+GA");
        await page.WaitForSelectorAsync(".card");

        var cards = await page.QuerySelectorAllAsync(".card");

        foreach (var card in cards)
        {
            var titleNode = await card.QuerySelectorAsync(".card-title-link");
            var companyNode = await card.QuerySelectorAsync(".card-company");
            var locationNode = await card.QuerySelectorAsync(".card-location");

            var title = await titleNode?.InnerTextAsync();
            var company = await companyNode?.InnerTextAsync();
            var location = await locationNode?.InnerTextAsync();
            var link = await titleNode?.GetAttributeAsync("href");

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(link))
                continue;

            jobs.Add(new AdzunaJob
            {
                title = title.Trim(),
                company = new AdzunaCompany { display_name = company?.Trim() },
                location = new AdzunaLocation { display_name = location?.Trim() },
                redirect_url = link,
                description = "",
                salary_min = null,
                salary_max = null
            });
        }

        await browser.CloseAsync();
        return jobs;
    }
}
