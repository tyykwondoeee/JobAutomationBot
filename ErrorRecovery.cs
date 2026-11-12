using Microsoft.Playwright;
using System;
using System.Threading.Tasks;

public static class ErrorRecovery
{
    private static readonly Random _rand = new Random();

    public static async Task<bool> RetryClick(IPage page, string selector, int attempts = 4)
    {
        for (int i = 0; i < attempts; i++)
        {
            try
            {
                await Humanizer.HumanDelay(150, 400);
                await Humanizer.HumanMoveMouse(page);

                var element = await page.QuerySelectorAsync(selector);
                if (element != null)
                {
                    await element.ClickAsync();
                    return true;
                }
            }
            catch
            {
                await Task.Delay(_rand.Next(200, 600));
            }
        }

        return false;
    }

    public static async Task<IElementHandle?> RetrySelector(IPage page, string selector, int attempts = 5)
    {
        for (int i = 0; i < attempts; i++)
        {
            var element = await page.QuerySelectorAsync(selector);
            if (element != null)
                return element;

            await Task.Delay(_rand.Next(200, 500));
        }

        return null;
    }

    public static async Task<bool> AutoRefreshIfStuck(IPage page, string selector)
    {
        var element = await page.QuerySelectorAsync(selector);

        if (element == null)
        {
            Console.WriteLine("Page appears stuck. Refreshing...");
            await page.ReloadAsync();
            await Task.Delay(_rand.Next(800, 1500));
            return true;
        }

        return false;
    }

    public static async Task<bool> SafeNavigation(Func<Task<bool>> action, int maxAttempts = 3)
    {
        for (int i = 0; i < maxAttempts; i++)
        {
            if (await action())
                return true;

            await Task.Delay(_rand.Next(300, 700));
        }

        return false;
    }
}
