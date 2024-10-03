using static MAP_KEYS.LANG_WRAP_KEY;
using static MAP_KEYS.TECH_KEY;
using static MAP_KEYS.STR_FORMAT_KEY;

public class EnglishLanguageWrapper : ILanguageWrapper
{
    public EnglishLanguageWrapper()
        : base()
    {
    }

    protected override void InitMap()
    {
        StringPropsMapping[ADDRESS_ATTRIBUTE_KEY] = "address";
        StringPropsMapping[AUTHORITY_ATTRIBUTE_KEY] = "authority";
        StringPropsMapping[PHONE_ATTRIBUTE_KEY] = "phone:tel";
        StringPropsMapping[OLOC_ATTRIBUTE_KEY] = "oloc";
        TechPropsMapping[INIT_ACCEPT_KEY] = "Accept all";



        FormatPropsMapping[ADD_TRIM_KEY] = "·Visited link";
    }
}
