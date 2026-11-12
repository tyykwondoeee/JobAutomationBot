using Microsoft.Playwright;

public class IndeedJobSource : IJobSource
{
    public async Task<List<AdzunaJob>> FetchJobsAsync()
    {
        var jobs = new List<AdzunaJob>();

        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new() { Headless = false });
        var page = await browser.NewPageAsync();

        await page.GotoAsync("https://www.indeed.com/jobs?q=C%23+Developer&l=Atlanta%2C+GA", new()
        {
            Timeout = 60000
        });

        // ⭐ Detect Cloudflare / CAPTCHA
        if (await page.Locator("text=Verify you are human").CountAsync() > 0 ||
            await page.Locator("text=unusual traffic").CountAsync() > 0)
        {
            Console.WriteLine("Indeed blocked the scraper (CAPTCHA). Skipping Indeed.");
            return jobs;
        }

        // ⭐ More stable selector
        var jobCards = page.Locator("div.slider_container");

        try
        {
            await jobCards.First.WaitForAsync(new() { Timeout = 15000 });
        }
        catch
        {
            Console.WriteLine("Indeed job cards did not load. Skipping Indeed.");
            return jobs;
        }

        int count = await jobCards.CountAsync();

        for (int i = 0; i < count; i++)
        {
            var card = jobCards.Nth(i);

            var title = await card.Locator("h2.jobTitle").InnerTextAsync();
            var company = await card.Locator(".companyName").InnerTextAsync();
            var location = await card.Locator(".companyLocation").InnerTextAsync();
            var link = await card.Locator("a").GetAttributeAsync("href");

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(link))
                continue;

            jobs.Add(new AdzunaJob
            {
                title = title.Trim(),
                company = new AdzunaCompany { display_name = company.Trim() },
                location = new AdzunaLocation { display_name = location.Trim() },
                redirect_url = "https://indeed.com" + link,
                description = "",
                salary_min = null,
                salary_max = null
            });
        }

        await browser.CloseAsync();
        return jobs;
    }
}
