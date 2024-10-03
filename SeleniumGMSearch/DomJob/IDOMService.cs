using HtmlAgilityPack;

public interface IDOMService
{
    HtmlNode GetDocumentNode();
    void LoadHtml(string htmlSource);
}
