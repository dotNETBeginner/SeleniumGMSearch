using OpenQA.Selenium;

public interface IWebDriverService
{
    IWebElement FindElement(DriverSelector selector, string query);
    IEnumerable<IWebElement> FindElements(DriverSelector selector, string query);

    string GetPageSource();

    void Click(IWebElement element);
    string GetAttribute(IWebElement element, string value);

    void ScrollDown(IWebElement element);

    void Close();
    void Navigate(string url);
}
