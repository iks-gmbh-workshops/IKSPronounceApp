using CommunityToolkit.Mvvm.ComponentModel;

namespace IKSPronounceApp;

internal partial class SettingsModel : ObservableObject
{
    [ObservableProperty] internal string speechSubscriptionKey;
    [ObservableProperty] internal string speechServiceRegion;
    [ObservableProperty] internal int? initialSilenceTimeoutMs;
    [ObservableProperty] internal int? endSilenceTimeoutMs;

    internal SettingsModel()
    {
        Load();
    }

    public void Load()
    {
        SpeechSubscriptionKey = Util.LoadTextPropertySync("speech_subscription_key", true);
        SpeechServiceRegion = Util.LoadTextPropertySync("speech_service_region", true);
        InitialSilenceTimeoutMs = Util.GetInt(Util.LoadTextPropertySync("initial_silence_timeout_ms", true));
        EndSilenceTimeoutMs = Util.GetInt(Util.LoadTextPropertySync("end_silence_timeout_ms", true));

        if (SpeechSubscriptionKey is null
            || SpeechServiceRegion is null
            || InitialSilenceTimeoutMs is null
            || EndSilenceTimeoutMs is null)
        {
            App.ErrorMessage = Util.GetMissingSettingsText();
        }
    }

    public void Save(string property)
    {
        switch (property)
        {
            case "SpeechSubscriptionKey":
                Util.SaveTextProperty("speech_subscription_key", SpeechSubscriptionKey, true);
                break;
            case "SpeechServiceRegion":
                Util.SaveTextProperty("speech_service_region", SpeechServiceRegion, true);
                break;
            case "InitialSilenceTimeoutMs":
                Util.SaveTextProperty("initial_silence_timeout_ms", InitialSilenceTimeoutMs.ToString(), true);
                break;
            case "EndSilenceTimeoutMs":
                Util.SaveTextProperty("end_silence_timeout_ms", EndSilenceTimeoutMs.ToString(), true);
                break;
            default:
                throw new ArgumentException($"Property '{property}' unknown.");
        }
    }
}
