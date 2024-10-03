using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System.Collections.Generic;

public class SeleniumDriverService<T> : IWebDriverService<T> where T : IWebElement
    
{
    private readonly IWebDriver _driver;
    //private WebDriverWait wait;

    public SeleniumDriverService()
    {
        var options = new ChromeOptions();

        // options.AddArgument("--headless");
        //options.AddArgument("--start-fullscreen");
        //options.AddArgument(@"--user-data-dir=C:\Users\Джиджа\AppData\Local\Google\Chrome\User Data");
        //options.AddArgument("--profile-directory=Profile 1");
        options.AddArgument("--guest");
        options.AddArgument("--lang=en-US");
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage"); // это может помочь в некоторых системах
        options.AddArgument("--remote-debugging-port=9222");

        // options.AddArgument("--disable-gpu"); // Отключить использование GPU (необходимо для стабильного headless режима)
         options.AddArgument("--window-size=1920,1080");
        _driver = new ChromeDriver(options);
        //_driver.Manage().Window.Maximize();
    }

    public async Task<T> FindElement(DriverSelector selector, string query)
    {
        var result = _driver.FindElement(SelectorsMapper.SelectorMapping[selector](query));
        return (T)result;
    }

    public async Task<IEnumerable<T>> FindElements(DriverSelector selector, string query)
    {
        var result = await Task.Run(() => _driver.FindElements(SelectorsMapper.SelectorMapping[selector](query)));
        return (IEnumerable<T>)result;
    }

    public async Task Navigate(string url)
    {
        await Task.Run(() => _driver.Navigate().GoToUrl(url));
        Thread.Sleep(3000);
        //_driver.FindElement(By.XPath("//*[@id=\"yDmH0d\"]/c-wiz/div/div/div/div[2]/div[1]/div[3]/div[1]/div[1]/form[2]/div/div/button/span")).Click();
    }

    public Task<string> GetAttribute(T element, string value)
    {
        return Task.Run(() => element.GetAttribute(value));
    }

    public async Task Click(T element)
    {
        await Task.Run(() => element.Click());
        Thread.Sleep(1500);
    }

    public async Task<string> GetPageSource()
    {
        return await Task.Run(() => _driver.PageSource);
    }

    public async Task ScrollDown(T element)
    {
        await Task.Run(() => ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", element));
        //((IJavaScriptExecutor)_driver).ExecuteScript("window.scrollBy(0, 1000);");
    }

    public async ValueTask DisposeAsync()
    {
        await Task.Run(() => _driver.Close());
    }
}
