using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using static MAP_KEYS.LANG_WRAP_KEY;
using static MAP_KEYS.TECH_KEY;
using static MAP_KEYS.STR_FORMAT_KEY;
class Program
{
    static void Main(string[] args)
    {
        var scraper = new Scraper(new SeleniumDriverService(), new AgilityDOMService(), new ConsoleUserInteractor(), new EnglishLanguageWrapper());

        var results = scraper.ScrapeData();

        foreach (var business in results)
        {
            Console.WriteLine($"Title: {business.Title}, Phone: {business.Phone}, Industry: {business.Industry}, Address: {business.Address}, Company URL: {business.CompanyUrl}, PLusCode: {business.PlusCode}\n\n");
        }

        Console.WriteLine(results.Count);
        Thread.Sleep(10000000);
        scraper.Close();

        
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



public class Scraper
{
   
    //private const string ADDRESS_ATTRIBUTE = _languageWrapper.StringPropsMapping["bus_address"];
    //private const string AUTHORITY_ATTRIBUTE =
    //private const string PHONE_ATTRIBUTE =
    //private const string OLOC_ATTRIBUTE = "oloc";
    private const string SEARCH_TEMPLATE = "https://www.google.com/maps/search/";
    private const string PLACE_TEMPLATE = "https://www.google.com/maps/place";

    private readonly ILanguageWrapper _languageWrapper;
    private readonly IWebDriverService _driver;
    private readonly IDOMService _domService;
    private readonly IUserInteractor _userInteractor;

    public Scraper(IWebDriverService driver, IDOMService domService, IUserInteractor userInteractor, ILanguageWrapper languageWrapper)
    {
        _driver = driver;
        _domService = domService;
        _userInteractor = userInteractor;
        _languageWrapper = languageWrapper;
    }

    



    public List<BusinessInfo> ScrapeData()
    {
        string[] query = _userInteractor.GetInput();
        int quantity = 40;

        _driver.Navigate($"{SEARCH_TEMPLATE}{query[0]}+{query[1]}");



        var acceptButton = _driver.FindElement(DriverSelector.XPath, $"//button[contains(@aria-label, '{_languageWrapper.TechPropsMapping[INIT_ACCEPT_KEY]}')]");
                                  
        _driver.Click(acceptButton);

        var divToScroll = _driver.FindElement(DriverSelector.XPath, $"//div[contains(@role, 'feed')]");
        var links = _driver.FindElements(DriverSelector.XPath, $"//a[contains(@href, '{PLACE_TEMPLATE}')]");
        var results = new List<BusinessInfo>();

        Thread.Sleep(2000);
        while(links.Count() < quantity)
        {
            Console.WriteLine("SCROLLED");
            _driver.ScrollDown(links.Last());
            links = _driver.FindElements(DriverSelector.XPath, $"//a[contains(@href, '{PLACE_TEMPLATE}')]");
            Thread.Sleep(2000);
        }

        Console.WriteLine(links.Count());

        foreach (var link in links)
        {
            try
            {
                    _driver.Click(link);

                _domService.LoadHtml(_driver.GetPageSource());
                var result = new Dictionary<string, string>();
                DFS(_domService.GetDocumentNode(), result, "data-item-id", _languageWrapper.StringPropsMapping[ADDRESS_ATTRIBUTE_KEY],
                    _languageWrapper.StringPropsMapping[AUTHORITY_ATTRIBUTE_KEY],
                    _languageWrapper.StringPropsMapping[PHONE_ATTRIBUTE_KEY],
                    _languageWrapper.StringPropsMapping[OLOC_ATTRIBUTE_KEY]);


                //foreach (var node in result)
                //{
                //    Console.WriteLine($"Найден элемент с class='my-class': {node.InnerText.Trim()}");
                //}

                var label = _driver.GetAttribute(link, "aria-label").Replace($"{_languageWrapper.FormatPropsMapping[ADD_TRIM_KEY]}", string.Empty);

                string titleText = label;
                //Console.WriteLine(titleText);

                results.Add(new BusinessInfo
                {
                    Title = titleText,
                    Phone = result.ContainsKey(_languageWrapper.StringPropsMapping[PHONE_ATTRIBUTE_KEY])
                    ? result[_languageWrapper.StringPropsMapping[PHONE_ATTRIBUTE_KEY]] : string.Empty,

                    Address = result.ContainsKey(_languageWrapper.StringPropsMapping[ADDRESS_ATTRIBUTE_KEY])
                    ? result[_languageWrapper.StringPropsMapping[ADDRESS_ATTRIBUTE_KEY]] : string.Empty,

                    CompanyUrl = result.ContainsKey(_languageWrapper.StringPropsMapping[AUTHORITY_ATTRIBUTE_KEY])
                    ? result[_languageWrapper.StringPropsMapping[AUTHORITY_ATTRIBUTE_KEY]] : string.Empty,

                    PlusCode = result.ContainsKey(_languageWrapper.StringPropsMapping[OLOC_ATTRIBUTE_KEY])
                    ? result[_languageWrapper.StringPropsMapping[OLOC_ATTRIBUTE_KEY]] : string.Empty
                });

            }
            catch
            {

            }

        }
        return results;
    }


    public void Close()
    {
        _driver.Close();
    }

    private void DFS(HtmlNode node, Dictionary<string, string> result, string attribute,params string[] valuesToFind )
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
            DFS(child, result, attribute, valuesToFind); // Рекурсивный вызов
        }
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