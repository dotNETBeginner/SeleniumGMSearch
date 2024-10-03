using Microsoft.Playwright;
using System;
using System.Threading.Tasks;

public class PlaywrightDriverService<T> : IWebDriverService<T> where T : IElementHandle
{
    private readonly IPage _page;
    private readonly IBrowser _browser;
    private readonly IBrowserContext _context;

    public PlaywrightDriverService()
    {
        var playwright = Playwright.CreateAsync().Result;

        _browser = playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false,  // Можно выставить в true для headless режима
            Args = new[] {
                "--guest", // Запуск в гостевом режиме
                "--lang=en-US", // Язык интерфейса
                "--no-sandbox",
                "--disable-dev-shm-usage",
                "--remote-debugging-port=9222"
            },
            SlowMo = 1000 // Замедляет действия для наглядности (опционально)
        }).Result;

        _context = _browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize
            {
                Width = 1920,
                Height = 1080
            }
        }).Result;

        _page = _context.NewPageAsync().Result;
    }

    public async Task<T> FindElement(DriverSelector Dselector, string selector)
    {
        await _page.WaitForSelectorAsync(selector); // Ожидание элемента на странице
        return (T)(await _page.QuerySelectorAsync(selector));
    }

    public async Task<IEnumerable<T>> FindElements(DriverSelector Dselector, string selector)
    {
        await _page.WaitForSelectorAsync(selector); // Ожидание элементов на странице
        return (IEnumerable<T>)(await _page.QuerySelectorAllAsync(selector));
    }

    public async Task Navigate(string url)
    {
        await _page.GotoAsync(url);
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle); // Ждем полной загрузки страницы
    }

    public async Task<string> GetAttribute(T element, string attribute)
    {
        return await element.GetAttributeAsync(attribute);
    }

    public async Task Click(T element)
    {
        await element.ClickAsync(); // Клик с задержкой
        Thread.Sleep(1500);
    }

    public async Task<string> GetPageSource()
    {
        return await _page.ContentAsync(); // Получаем весь исходный HTML страницы
    }

    public async Task ScrollDown(T element)
    {
        await element.ScrollIntoViewIfNeededAsync(); // Скроллим до элемента
    }

    public async Task Close()
    {
        await _context.CloseAsync();
        await _browser.CloseAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await Close();
    }
}