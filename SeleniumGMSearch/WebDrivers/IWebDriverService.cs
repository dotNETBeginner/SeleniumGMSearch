using OpenQA.Selenium;

public interface IWebDriverService<T> : IAsyncDisposable
{
    Task<T> FindElement(DriverSelector selector, string query);
    Task<IEnumerable<T>> FindElements(DriverSelector selector, string query);

    Task<string> GetPageSource();

    Task Click(T element);
    Task<string> GetAttribute(T element, string value);

    Task ScrollDown(T element);
    Task Navigate(string url);
}
