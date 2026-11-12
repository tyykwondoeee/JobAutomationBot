using Microsoft.Playwright;
using System;
using System.Threading.Tasks;

public static class Humanizer
{
    private static readonly Random _rand = new Random();

    public static async Task HumanDelay(int min = 80, int max = 250)
    {
        await Task.Delay(_rand.Next(min, max));
    }

    public static async Task HumanMoveMouse(IPage page)
    {
        int steps = _rand.Next(15, 40);

        for (int i = 0; i < steps; i++)
        {
            int x = _rand.Next(0, 800);
            int y = _rand.Next(0, 600);

            await page.Mouse.MoveAsync(x, y);
            await Task.Delay(_rand.Next(5, 20));
        }
    }

    public static async Task HumanScroll(IPage page)
    {
        int amount = _rand.Next(200, 600);
        await page.Mouse.WheelAsync(0, amount);
        await Task.Delay(_rand.Next(200, 500));
    }

    public static async Task HumanClick(IPage page, string selector)
    {
        await HumanMoveMouse(page);
        await HumanDelay(150, 400);
        await page.ClickAsync(selector);
    }

    public static async Task HumanType(IPage page, string selector, string text)
    {
        var locator = page.Locator(selector);

        foreach (char c in text)
        {
            await locator.TypeAsync(c.ToString());
            await Task.Delay(_rand.Next(20, 120));
        }
    }
}

