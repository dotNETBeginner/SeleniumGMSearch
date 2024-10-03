
using MAP_KEYS;


public abstract class ILanguageWrapper
{
    public Dictionary<LANG_WRAP_KEY, string> StringPropsMapping { get; set; }
    public Dictionary<TECH_KEY, string> TechPropsMapping { get; set; }

    public Dictionary<STR_FORMAT_KEY, string> FormatPropsMapping {  get; set; }

    protected ILanguageWrapper()
    {
        StringPropsMapping = new Dictionary<LANG_WRAP_KEY, string>();
        TechPropsMapping = new Dictionary<TECH_KEY, string>();
        FormatPropsMapping = new Dictionary<STR_FORMAT_KEY, string>();
        InitMap();
    }

    protected abstract void InitMap();
}
