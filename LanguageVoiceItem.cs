using CommunityToolkit.Mvvm.ComponentModel;

namespace IKSPronounceApp;

internal partial class LanguageVoiceItem : ObservableObject
{
    [ObservableProperty] internal string languageName; // Reader-friendly language name
    [ObservableProperty] internal string languageCode; // Language code (for speech translation)
    [ObservableProperty] internal string languageLocale; // Language-Locale (for speech-to-text and text-to-speech)

    [ObservableProperty] internal string languageNameAndLocale;

    internal LanguageVoiceItem(string languageName, string languageCode, string languageLocale)
    {
        LanguageName = languageName;
        LanguageCode = languageCode;
        LanguageLocale = languageLocale;

        LanguageNameAndLocale = $"{LanguageName} [{languageLocale}]";
    }
}
