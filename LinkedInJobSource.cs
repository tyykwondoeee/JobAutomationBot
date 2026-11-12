using Microsoft.Playwright;

public class LinkedInJobSource : IJobSource
{
	public async Task<List<AdzunaJob>> FetchJobsAsync()
	{
		var jobs = new List<AdzunaJob>();

		using var playwright = await Playwright.CreateAsync();
		var browser = await playwright.Chromium.LaunchAsync(new() { Headless = true });
		var page = await browser.NewPageAsync();

		// LinkedIn job search URL
		await page.GotoAsync("https://www.linkedin.com/jobs/search/?keywords=C%23%20Developer&location=Atlanta%2C%20Georgia");

		// Wait for job cards to load
		await page.WaitForSelectorAsync(".base-card");

		var cards = await page.QuerySelectorAllAsync(".base-card");

		foreach (var card in cards)
		{
			var titleNode = await card.QuerySelectorAsync(".base-search-card__title");
			var companyNode = await card.QuerySelectorAsync(".base-search-card__subtitle");
			var locationNode = await card.QuerySelectorAsync(".job-search-card__location");
			var linkNode = await card.QuerySelectorAsync("a.base-card__full-link");

			var title = await titleNode?.InnerTextAsync();
			var company = await companyNode?.InnerTextAsync();
			var location = await locationNode?.InnerTextAsync();
			var link = await linkNode?.GetAttributeAsync("href");

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
