using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;

public class SeleniumDriverService : IWebDriverService
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

    public IWebElement FindElement(DriverSelector selector, string query)
    {
        var result = _driver.FindElement(SelectorsMapper.SelectorMapping[selector](query));
        return result;
    }

    public IEnumerable<IWebElement> FindElements(DriverSelector selector, string query)
    {
        var result = _driver.FindElements(SelectorsMapper.SelectorMapping[selector](query));
        return result;
    }

    public void Navigate(string url)
    {
        _driver.Navigate().GoToUrl(url);
        Thread.Sleep(3000);
        //_driver.FindElement(By.XPath("//*[@id=\"yDmH0d\"]/c-wiz/div/div/div/div[2]/div[1]/div[3]/div[1]/div[1]/form[2]/div/div/button/span")).Click();
    }

    public string GetAttribute(IWebElement element, string value)
    {
        return element.GetAttribute(value);
    }

    public void Close()
    { 
        _driver.Close(); 
    }

    public void Click(IWebElement element)
    {
        element.Click();
        Thread.Sleep(1500);
    }

    public string GetPageSource()
    {
        return _driver.PageSource;
    }

    public void ScrollDown(IWebElement element)
    {
        ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", element);
        //((IJavaScriptExecutor)_driver).ExecuteScript("window.scrollBy(0, 1000);");
    }
}
