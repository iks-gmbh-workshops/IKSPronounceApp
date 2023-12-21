using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace IKSPronounceApp;

internal partial class SettingsViewModel : ObservableObject
{
    [ObservableProperty] private SettingsModel model;

    public SettingsViewModel()
    {
        Model = App.SettingsModel;
    }

    [RelayCommand]
    public void Save(string property)
    {
        Model.Save(property);
    }
}