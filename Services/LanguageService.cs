using Story_Teller.IServices;

namespace Story_Teller.Services;

public class LanguageService : ILanguageService
{
    private const string LanguageKey = "SelectedLanguage";

    public string GetLanguage()
        => Preferences.Get(LanguageKey, "ua");

    public void SetLanguage(string language)
        => Preferences.Set(LanguageKey, language);
}