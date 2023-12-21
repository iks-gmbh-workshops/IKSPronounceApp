using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace IKSPronounceApp;

internal partial class MainPageViewModel : ObservableObject
{
    [ObservableProperty] private MainPageModel model;

    #region Model-independent fields

    // Selectable items with default values
    [ObservableProperty] private LanguageVoiceItem selectedLanguageVoiceItem = new("English", "en", "en-US");

    // Free-text entry
    [ObservableProperty] private string inputText = string.Empty;

    // Flag for readiness state
    [ObservableProperty] private bool isReady = true;
    [ObservableProperty] private bool isBusy = false;

    #endregion

    #region Model-dependent fields

    // Text output
    [ObservableProperty] private ObservableCollection<OutputMessage> outputMessages = [];

    #endregion

    public MainPageViewModel()
    {
        if (App.ErrorMessage is not null)
        {
            IsReady = false;
            OutputMessages.Add(App.ErrorMessage);
        }
        else
        {
            Model = new MainPageModel();
            OutputMessages = Model.OutputMessages;

            Load();
        }
    }

    private void Load()
    {
        var languageLocale = Util.LoadTextPropertySync("pronunciate_language_locale", false);
        if (languageLocale == string.Empty)
        {
            languageLocale = SelectedLanguageVoiceItem.LanguageLocale;
        }
        SelectedLanguageVoiceItem = Model.LanguageVoiceItems.ToList().Find(x => x.LanguageLocale == languageLocale);
    }

    [RelayCommand]
    public void Save(string property)
    {
        switch (property)
        {
            case "LanguageVoiceItem":
                Util.SaveTextProperty("pronunciate_language_locale", SelectedLanguageVoiceItem.LanguageLocale, false);
                break;
            default:
                throw new ArgumentException($"Property '{property}' unknown.");
        }
    }

    [RelayCommand]
    public async Task GetText()
    {
        IsReady = false;
        IsBusy = true;
        InputText = await MainPageModel.GetText(SelectedLanguageVoiceItem);
        IsReady = true;
        IsBusy = false;
    }

    [RelayCommand]
    public async Task Speak()
    {
        IsReady = false;
        IsBusy = true;
        await Model.Speak(InputText, SelectedLanguageVoiceItem.LanguageLocale);
        IsReady = true;
        IsBusy = false;
    }

    [RelayCommand]
    public async Task Listen()
    {
        IsReady = false;
        IsBusy = true;
        await Model.Listen(InputText, SelectedLanguageVoiceItem);
        IsReady = true;
        IsBusy = false;
    }
}