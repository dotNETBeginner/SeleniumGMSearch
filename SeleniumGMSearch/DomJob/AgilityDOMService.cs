using HtmlAgilityPack;

public class AgilityDOMService : IDOMService
{
    private HtmlDocument _document;

    public AgilityDOMService()
    {
        _document = new HtmlDocument();
    }

    public HtmlNode GetDocumentNode()
    {
        return _document.DocumentNode;
    }

    public void LoadHtml(string htmlSource)
    {
        _document.LoadHtml(htmlSource);
    }
}
