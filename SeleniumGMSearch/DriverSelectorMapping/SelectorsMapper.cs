using OpenQA.Selenium;

public static class SelectorsMapper
{
    public static Dictionary<DriverSelector, Func<string,By>> SelectorMapping { get; private set; }

    static SelectorsMapper()
    {
        SelectorMapping = new Dictionary<DriverSelector, Func<string, By>>()
        {
            [DriverSelector.XPath] = selector => By.XPath(selector),
            [DriverSelector.CssSelector] = selector => By.CssSelector(selector),
            [DriverSelector.LinkText] = selector => By.LinkText(selector),
            [DriverSelector.PartialLinkText] = selector => By.PartialLinkText(selector),
            [DriverSelector.Name] = selector => By.Name(selector),
            [DriverSelector.Id] = selector => By.Id(selector),
            [DriverSelector.ClassName] = selector => By.ClassName(selector),
            [DriverSelector.TagName] = selector => By.TagName(selector)
        };
    }

}
