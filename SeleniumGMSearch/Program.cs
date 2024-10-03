using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using static MAP_KEYS.LANG_WRAP_KEY;
using static MAP_KEYS.TECH_KEY;
using static MAP_KEYS.STR_FORMAT_KEY;
using Microsoft.Playwright;
using SeleniumGMSearch;
class Program
{
    static async Task Main(string[] args)
    {
        await using var scraper = new Scraper<IElementHandle>(new PlaywrightDriverService<IElementHandle>(), 
            new AgilityDOMService(), new ConsoleUserInteractor(), new EnglishLanguageWrapper(), new ZipCodeFinder());

        var results = await scraper.ScrapeData();

        foreach (var business in results)
        {
            Console.WriteLine($"Title: {business.Title}, Phone: {business.Phone}, Industry: {business.Industry}, Address: {business.Address}, Company URL: {business.CompanyUrl}, PLusCode: {business.PlusCode}\n\n");
        }

        Console.WriteLine(results.Count);
        Thread.Sleep(1000);

    }


}


public interface IUserInteractor
{
    string[] GetInput();

    void ShowMessage(string message);
}

public class ConsoleUserInteractor : IUserInteractor
{
    public string[] GetInput()
    {
        var input = Console.ReadLine().Split(' ');

        while(input.Length < 1 && input.Length > 2)
        {
            Console.WriteLine("Invalid input, you should input two string: \"{category} {location}\"");
            input = Console.ReadLine().Split(' ');
        }

        return input;
    }

    public void ShowMessage(string message)
    {
        Console.WriteLine(message);
    }
}



public class Scraper<T> : IAsyncDisposable
{
   
    //private const string ADDRESS_ATTRIBUTE = _languageWrapper.StringPropsMapping["bus_address"];
    //private const string AUTHORITY_ATTRIBUTE =
    //private const string PHONE_ATTRIBUTE =
    //private const string OLOC_ATTRIBUTE = "oloc";
    private const string SEARCH_TEMPLATE = "https://www.google.com/maps/search/";
    private const string PLACE_TEMPLATE = "https://www.google.com/maps/place";

    private readonly ILanguageWrapper _languageWrapper;
    private readonly IWebDriverService<T> _driver;
    private readonly IDOMService _domService;
    private readonly IUserInteractor _userInteractor;
    private readonly IZipCodeFinder _zipCodeFinder;
    
    public Scraper(IWebDriverService<T> driver, IDOMService domService, IUserInteractor userInteractor, ILanguageWrapper languageWrapper, IZipCodeFinder zipCodeFinder)
    {
        _driver = driver;
        _domService = domService;
        _userInteractor = userInteractor;
        _languageWrapper = languageWrapper;
        _zipCodeFinder = zipCodeFinder;
    }

    



    public async Task<List<BusinessInfo>> ScrapeData() 
    {
        string[] query = _userInteractor.GetInput();
        int quantity = 40;

        await _driver.Navigate($"{SEARCH_TEMPLATE}{query[0]}+{query[1]}");



        var acceptButton = await _driver.FindElement(DriverSelector.XPath, $"//button[contains(@aria-label, '{_languageWrapper.TechPropsMapping[INIT_ACCEPT_KEY]}')]");
                                  
        await _driver.Click(acceptButton);

        var divToScroll = await _driver.FindElement(DriverSelector.XPath, $"//div[contains(@role, 'feed')]");
        var links = await _driver.FindElements(DriverSelector.XPath, $"//a[contains(@href, '{PLACE_TEMPLATE}')]");
        var results = new List<BusinessInfo>();

        Thread.Sleep(2000);
        int prevCount = 0;
        int count = links.Count();
        while (links.Count() < quantity && count - prevCount > 0) 
        {
            Console.WriteLine("SCROLLED");
            await _driver.ScrollDown(links.Last());
            links = await _driver.FindElements(DriverSelector.XPath, $"//a[contains(@href, '{PLACE_TEMPLATE}')]");
            Thread.Sleep(2000);
            prevCount = links.Count();
        }
        

        Console.WriteLine(links.Count());

        foreach (var link in links)
        {
            try
            {
                await _driver.Click(link);

                _domService.LoadHtml(await _driver.GetPageSource());
                var result = new Dictionary<string, string>();
                await DFS(_domService.GetDocumentNode(), result, "data-item-id", _languageWrapper.StringPropsMapping[ADDRESS_ATTRIBUTE_KEY],
                    _languageWrapper.StringPropsMapping[AUTHORITY_ATTRIBUTE_KEY],
                    _languageWrapper.StringPropsMapping[PHONE_ATTRIBUTE_KEY],
                    _languageWrapper.StringPropsMapping[OLOC_ATTRIBUTE_KEY]);
                Thread.Sleep(500);

                //foreach (var node in result)
                //{
                //    Console.WriteLine($"Найден элемент с class='my-class': {node.InnerText.Trim()}");
                //}

                var label = _driver.GetAttribute(link, "aria-label").Result.Replace($"{_languageWrapper.FormatPropsMapping[ADD_TRIM_KEY]}", string.Empty);
                var zipCode = _zipCodeFinder.FindZipCode(_languageWrapper.StringPropsMapping[ADDRESS_ATTRIBUTE_KEY]);

                string titleText = label;
                //Console.WriteLine(titleText);

                results.Add(new BusinessInfo
                {
                    Title = titleText,
                    Phone = result.ContainsKey(_languageWrapper.StringPropsMapping[PHONE_ATTRIBUTE_KEY])
                    ? result[_languageWrapper.StringPropsMapping[PHONE_ATTRIBUTE_KEY]].Replace("?","") : string.Empty,

                    Address = result.ContainsKey(_languageWrapper.StringPropsMapping[ADDRESS_ATTRIBUTE_KEY])
                    ? result[_languageWrapper.StringPropsMapping[ADDRESS_ATTRIBUTE_KEY]].Replace("?", "").Replace(zipCode,"") : string.Empty,

                    ZipCode = zipCode,

                    CompanyUrl = result.ContainsKey(_languageWrapper.StringPropsMapping[AUTHORITY_ATTRIBUTE_KEY])
                    ? result[_languageWrapper.StringPropsMapping[AUTHORITY_ATTRIBUTE_KEY]].Replace("?", "") : string.Empty,

                    PlusCode = result.ContainsKey(_languageWrapper.StringPropsMapping[OLOC_ATTRIBUTE_KEY])
                    ? result[_languageWrapper.StringPropsMapping[OLOC_ATTRIBUTE_KEY]].Replace("?", "") : string.Empty
                });

            }
            catch
            {

            }

        }
        return results;
    }


    private async Task DFS(HtmlNode node, Dictionary<string, string> result, string attribute,params string[] valuesToFind )
    {

        var dataItemIdValue = node.GetAttributeValue(attribute, "");

        foreach(var value in valuesToFind)
        {
            if(dataItemIdValue.Contains(value))
            {
                result[value] = node.InnerText.Trim();
            }
        }

        //if(dataItemIdValue == addressArg)
        //{
        //    result[addressArg] = node.InnerText.Trim();
        //}
        //else if(dataItemIdValue == authorityArg)
        //{
        //    result[authorityArg] = node.InnerText.Trim();
        //}
        //if (dataItemIdValue.Contains(phoneArg))
        //{
        //    result[phoneArg] = node.InnerText.Trim();
        //}
        //else if (dataItemIdValue == oloc)
        //{
        //    result[oloc] = node.InnerText.Trim();
        //}


        // Рекурсивно обходим все дочерние узлы
        foreach (var child in node.ChildNodes)
        {
            await DFS(child, result, attribute, valuesToFind); // Рекурсивный вызов
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _driver.DisposeAsync();
    }
}



public class BusinessInfo
{
    public string Title { get; set; }
    public string PlusCode {  get; set; }
    public string Phone { get; set; }
    public string Industry { get; set; }
    public string Address { get; set; }
    public string CompanyUrl { get; set; }
    public string ZipCode { get; set; }

    public override int GetHashCode()
    {
        return HashCode.Combine(PlusCode,Title, CompanyUrl);
    }

    public override bool Equals(object? obj)
    {
        return obj is BusinessInfo bobj &&
            bobj.PlusCode == PlusCode && bobj.Title == Title && bobj.CompanyUrl==CompanyUrl;
    }
}