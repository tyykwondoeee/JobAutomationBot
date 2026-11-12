using Microsoft.Playwright;
using System;
using System.Threading.Tasks;

public class AutoApply
{
    private const string SessionPath = "playwright-session";

    private readonly QuestionAnswering _qa = new QuestionAnswering();
    private readonly HybridResumeEngine _resumeEngine = new HybridResumeEngine();

    public async Task<bool> ApplyToJobAsync(
        string applyUrl,
        string resumePath,
        string companyName,
        string jobTitle,
        string jobDescription,
        List<string> skills)
    {
        try
        {
            var playwright = await Playwright.CreateAsync();

            var browser = await playwright.Chromium.LaunchPersistentContextAsync(
                SessionPath,
                new BrowserTypeLaunchPersistentContextOptions
                {
                    Headless = false
                });

            var page = await browser.NewPageAsync();
            await page.GotoAsync(applyUrl);

            await Humanizer.HumanScroll(page);
            await Humanizer.HumanDelay(500, 1200);

            Console.WriteLine($"Opened apply page: {applyUrl}");

            if (applyUrl.Contains("linkedin.com"))
                return await ApplyLinkedInEasyApply(page, resumePath, companyName, jobTitle, jobDescription, skills);

            if (applyUrl.Contains("indeed.com"))
                return await ApplyIndeedEasyApply(page, resumePath, companyName, jobTitle, jobDescription, skills);

            if (applyUrl.Contains("workday"))
                return await ApplyWorkday(page, resumePath, companyName, jobTitle, jobDescription, skills);

            if (applyUrl.Contains("greenhouse"))
                return await ApplyGreenhouse(page, resumePath, companyName, jobTitle, jobDescription, skills);

            if (applyUrl.Contains("lever"))
                return await ApplyLever(page, resumePath, companyName, jobTitle, jobDescription, skills);

            return await ApplyGeneric(page, resumePath, companyName, jobTitle, jobDescription, skills);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Auto-apply failed: {ex.Message}");
            return false;
        }
    }

    // ---------------------------------------------------------
    // LINKEDIN EASY APPLY
    // ---------------------------------------------------------

    private async Task<bool> ApplyLinkedInEasyApply(
        IPage page,
        string resumePath,
        string companyName,
        string jobTitle,
        string jobDescription,
        List<string> skills)
    {
        Console.WriteLine("Detected LinkedIn Easy Apply.");

        try
        {
            await page.WaitForSelectorAsync("div.jobs-easy-apply-modal", new() { Timeout = 8000 });

            await TryUploadResume(page, resumePath);
            await AnswerQuestions(page, companyName, jobTitle, jobDescription, skills);
            await HandleLinkedInDropdowns(page);
            await NavigateLinkedInSteps(page);

            await TryClick(page, "button[aria-label='Submit application']");
            await TryClick(page, "button:has-text('Submit')");

            Console.WriteLine("LinkedIn Easy Apply submitted.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"LinkedIn Easy Apply failed: {ex.Message}");
            return false;
        }
    }

    private async Task HandleLinkedInDropdowns(IPage page)
    {
        try
        {
            var dropdowns = await page.QuerySelectorAllAsync("select");

            foreach (var dropdown in dropdowns)
            {
                var label = await dropdown.EvaluateAsync<string>("el => el.closest('label')?.innerText || ''");
                string lower = label.ToLower();

                if (lower.Contains("authorization"))
                    await dropdown.SelectOptionAsync(new[] { "Yes" });

                if (lower.Contains("sponsor"))
                    await dropdown.SelectOptionAsync(new[] { "No" });

                if (lower.Contains("relocate"))
                    await dropdown.SelectOptionAsync(new[] { "Yes" });

                if (lower.Contains("experience"))
                    await dropdown.SelectOptionAsync(new[] { "0-1 years" });

                if (lower.Contains("salary"))
                    await dropdown.SelectOptionAsync(new[] { "50000" });
            }
        }
        catch
        {
            Console.WriteLine("No LinkedIn dropdowns found.");
        }
    }

    private async Task NavigateLinkedInSteps(IPage page)
    {
        string[] nextButtons =
        {
            "button:has-text('Next')",
            "button:has-text('Continue')",
            "button:has-text('Review')"
        };

        while (true)
        {
            await Humanizer.HumanScroll(page);
            await Humanizer.HumanDelay(200, 600);

            bool clicked = false;

            foreach (var selector in nextButtons)
            {
                try
                {
                    var button = await ErrorRecovery.RetrySelector(page, selector);
                    if (button != null)
                    {
                        await Humanizer.HumanClick(page, selector);
                        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                        clicked = true;
                        break;
                    }
                }
                catch { }
            }

            if (!clicked)
                break;
        }
    }

    // ---------------------------------------------------------
    // INDEED EASY APPLY
    // ---------------------------------------------------------

    private async Task<bool> ApplyIndeedEasyApply(
        IPage page,
        string resumePath,
        string companyName,
        string jobTitle,
        string jobDescription,
        List<string> skills)
    {
        Console.WriteLine("Detected Indeed Easy Apply.");

        try
        {
            await page.WaitForSelectorAsync("div.ia-BasePage", new() { Timeout = 8000 });

            await TryUploadResume(page, resumePath);
            await AnswerQuestions(page, companyName, jobTitle, jobDescription, skills);
            await HandleIndeedRadios(page);
            await NavigateIndeedSteps(page);

            await TryClick(page, "button:has-text('Submit your application')");
            await TryClick(page, "button:has-text('Submit')");

            Console.WriteLine("Indeed Easy Apply submitted.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Indeed Easy Apply failed: {ex.Message}");
            return false;
        }
    }

    private async Task HandleIndeedRadios(IPage page)
    {
        try
        {
            var radios = await page.QuerySelectorAllAsync("input[type='radio']");

            foreach (var radio in radios)
            {
                var label = await radio.EvaluateAsync<string>("el => el.closest('label')?.innerText || ''");
                string lower = label.ToLower();

                if (lower.Contains("authorized"))
                    await radio.CheckAsync();

                if (lower.Contains("sponsor"))
                    await radio.CheckAsync();

                if (lower.Contains("relocate"))
                    await radio.CheckAsync();
            }
        }
        catch
        {
            Console.WriteLine("No Indeed radio buttons found.");
        }
    }

    private async Task NavigateIndeedSteps(IPage page)
    {
        string[] nextButtons =
        {
            "button:has-text('Continue')",
            "button:has-text('Next')",
            "button:has-text('Review')"
        };

        while (true)
        {
            await Humanizer.HumanScroll(page);
            await Humanizer.HumanDelay(200, 600);

            bool clicked = false;

            foreach (var selector in nextButtons)
            {
                try
                {
                    var button = await ErrorRecovery.RetrySelector(page, selector);
                    if (button != null)
                    {
                        await Humanizer.HumanClick(page, selector);
                        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                        clicked = true;
                        break;
                    }
                }
                catch { }
            }

            if (!clicked)
                break;
        }
    }

    // ---------------------------------------------------------
    // MULTI-STEP + QUESTION ANSWERING
    // ---------------------------------------------------------

    private async Task<bool> NavigateMultiStep(IPage page, string companyName, string jobTitle, string jobDescription, List<string> skills)
    {
        string[] nextButtons =
        {
            "button:has-text('Next')",
            "button:has-text('Continue')",
            "button:has-text('Save and Continue')",
            "button:has-text('Save & Continue')",
            "button:has-text('Proceed')",
            "button:has-text('Review')"
        };

        while (true)
        {
            await AnswerQuestions(page, companyName, jobTitle, jobDescription, skills);

            await Humanizer.HumanScroll(page);
            await Humanizer.HumanDelay(200, 600);

            bool clicked = false;

            foreach (var selector in nextButtons)
            {
                try
                {
                    var button = await ErrorRecovery.RetrySelector(page, selector);
                    if (button != null)
                    {
                        await Humanizer.HumanClick(page, selector);
                        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                        clicked = true;
                        break;
                    }
                }
                catch { }
            }

            if (!clicked)
                break;
        }

        return true;
    }

    private async Task AnswerQuestions(IPage page, string companyName, string jobTitle, string jobDescription, List<string> skills)
    {
        try
        {
            var inputs = await page.QuerySelectorAllAsync("textarea, input[type='text']");

            foreach (var input in inputs)
            {
                var label = await input.EvaluateAsync<string>("el => el.closest('label')?.innerText || ''");

                if (!string.IsNullOrWhiteSpace(label))
                {
                    string answer = _qa.GenerateAnswer(label, companyName);

                    if (label.ToLower().Contains("yourself"))
                        answer = _resumeEngine.GenerateTailoredSummary(jobTitle, companyName, skills);

                    if (label.ToLower().Contains("fit") || label.ToLower().Contains("qualified"))
                        answer = _resumeEngine.GenerateSkillsMatch(jobDescription, skills);

                    if (label.ToLower().Contains("cover") || label.ToLower().Contains("letter"))
                        answer = _resumeEngine.GenerateMiniCoverLetter(jobTitle, companyName, skills);

                    string selector = await input.EvaluateAsync<string>("el => el.tagName.toLowerCase() === 'textarea' ? 'textarea' : 'input[type=\"text\"]'");

                    await Humanizer.HumanType(page, selector, answer);

                    Console.WriteLine($"Answered question: {label}");
                }
            }
        }
        catch
        {
            Console.WriteLine("No questions detected.");
        }
    }

    // ---------------------------------------------------------
    // HELPERS
    // ---------------------------------------------------------

    private async Task TryUploadResume(IPage page, string resumePath)
    {
        try
        {
            await Humanizer.HumanDelay(300, 700);

            bool refreshed = await ErrorRecovery.AutoRefreshIfStuck(page, "input[type='file']");
            if (refreshed)
                await Humanizer.HumanDelay(500, 900);

            await page.SetInputFilesAsync("input[type='file']", resumePath);
            Console.WriteLine("Resume uploaded.");
        }
        catch
        {
            Console.WriteLine("No resume upload field found.");
        }
    }

    private async Task TryClick(IPage page, string selector)
    {
        try
        {
            bool success = await ErrorRecovery.RetryClick(page, selector);

            if (!success)
                Console.WriteLine($"Failed to click: {selector}");
        }
        catch { }
    }

    // ---------------------------------------------------------
    // ⭐ MISSING ATS METHODS (NOW FIXED)
    // ---------------------------------------------------------

    private async Task<bool> ApplyWorkday(
        IPage page,
        string resumePath,
        string companyName,
        string jobTitle,
        string jobDescription,
        List<string> skills)
    {
        try
        {
            await Humanizer.HumanScroll(page);

            if (await page.Locator("input[type='file']").CountAsync() > 0)
            {
                await page.SetInputFilesAsync("input[type='file']", resumePath);
                await Humanizer.HumanDelay();
            }

            var continueButtons = page.Locator("button:has-text('Continue'), button:has-text('Next')");
            if (await continueButtons.CountAsync() > 0)
            {
                await continueButtons.First.ClickAsync();
                await Humanizer.HumanDelay();
            }

            var submit = page.Locator("button:has-text('Submit')");
            if (await submit.CountAsync() > 0)
            {
                await submit.First.ClickAsync();
                await Humanizer.HumanDelay();
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    private async Task<bool> ApplyGreenhouse(
        IPage page,
        string resumePath,
        string companyName,
        string jobTitle,
        string jobDescription,
        List<string> skills)
    {
        try
        {
            await Humanizer.HumanScroll(page);

            if (await page.Locator("input[type='file']").CountAsync() > 0)
            {
                await page.SetInputFilesAsync("input[type='file']", resumePath);
                await Humanizer.HumanDelay();
            }

            if (await page.Locator("input[name='first_name']").CountAsync() > 0)
                await page.FillAsync("input[name='first_name']", "Tywain");

            if (await page.Locator("input[name='last_name']").CountAsync() > 0)
                await page.FillAsync("input[name='last_name']", "Taylor");

            var submit = page.Locator("button:has-text('Submit Application')");
            if (await submit.CountAsync() > 0)
            {
                await submit.First.ClickAsync();
                await Humanizer.HumanDelay();
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    private async Task<bool> ApplyLever(
        IPage page,
        string resumePath,
        string companyName,
        string jobTitle,
        string jobDescription,
        List<string> skills)
    {
        try
        {
            await Humanizer.HumanScroll(page);

            if (await page.Locator("input[type='file']").CountAsync() > 0)
            {
                await page.SetInputFilesAsync("input[type='file']", resumePath);
                await Humanizer.HumanDelay();
            }

            var next = page.Locator("button:has-text('Next')");
            if (await next.CountAsync() > 0)
            {
                await next.First.ClickAsync();
                await Humanizer.HumanDelay();
            }

            var submit = page.Locator("button:has-text('Submit Application')");
            if (await submit.CountAsync() > 0)
            {
                await submit.First.ClickAsync();
                await Humanizer.HumanDelay();
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    private async Task<bool> ApplyGeneric(
        IPage page,
        string resumePath,
        string companyName,
        string jobTitle,
        string jobDescription,
        List<string> skills)
    {
        try
        {
            await Humanizer.HumanScroll(page);

            if (await page.Locator("input[type='file']").CountAsync() > 0)
            {
                await page.SetInputFilesAsync("input[type='file']", resumePath);
                await Humanizer.HumanDelay();
            }

            var submit = page.Locator("button:has-text('Apply'), button:has-text('Submit')");
            if (await submit.CountAsync() > 0)
            {
                await submit.First.ClickAsync();
                await Humanizer.HumanDelay();
            }

            return true;
        }
        catch
        {
            return false;
        }
    }
}
