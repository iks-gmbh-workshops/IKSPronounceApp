using Microsoft.CognitiveServices.Speech;

namespace IKSPronounceApp;

internal static class Util
{
    internal static void ScrollToEnd(ScrollView scv, StackLayout s)
    {
        new Timer((object obj) =>
        {
            MainThread.BeginInvokeOnMainThread(async () => await scv.ScrollToAsync(0, s.Height, true));
        }, null, 1, Timeout.Infinite);
    }

    internal static async Task<bool> EnsureMicrophonePermission()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.Microphone>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.Microphone>();
        }
        return status == PermissionStatus.Granted;
    }

    internal static OutputMessage GetInfoText(string text)
    {
        return new OutputMessage { Type = OutputMessageType.Information, Text = text };
    }

    internal static OutputMessage GetNoMatchText()
    {
        return new OutputMessage { Type = OutputMessageType.Information, Text = "SPEECH NOT RECOGNIZED" };
    }

    internal static OutputMessage GetMissingSettingsText()
    {
        return new OutputMessage { Type = OutputMessageType.Information, Text = "SETTINGS MISSING. Fill in the 'Settings' page and restart the app." };
    }

    internal static OutputMessage GetCanceledText(RecognitionResult result)
    {
        var cancellation = CancellationDetails.FromResult(result);
        return GetCancellationDetailsText(cancellation.Reason, cancellation.ErrorCode, cancellation.ErrorDetails);
    }

    internal static OutputMessage GetCanceledText(SpeechSynthesisResult result)
    {
        var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
        return GetCancellationDetailsText(cancellation.Reason, cancellation.ErrorCode, cancellation.ErrorDetails);
    }

    private static OutputMessage GetCancellationDetailsText(CancellationReason reason, CancellationErrorCode errorCode, string errorDetails)
    {
        var text = string.Empty;

        text += $"CANCELED: Reason = {reason}";

        if (reason == CancellationReason.Error)
        {
            text += $", ErrorCode = {errorCode}";
            text += $", ErrorDetails = {errorDetails}";
        }

        return new OutputMessage { Type = OutputMessageType.Error, Text = text };
    }

    internal static OutputMessage GetErrorText(Exception ex)
    {
        return new OutputMessage { Type = OutputMessageType.Error, Text = ex.Message };
    }

    internal static string LoadTextPropertySync(string key, bool isSecure)
    {
       return Task<string>.Run(() => LoadTextProperty(key, isSecure)).Result;
    }

    internal static async Task<string> LoadTextProperty(string key, bool isSecure)
    {
        return isSecure ? await SecureStorage.Default.GetAsync(key) : Preferences.Default.Get(key, string.Empty);
    }

    internal static void SaveTextProperty(string key, string value, bool isSecure)
    {
        if (isSecure)
        {
            if (string.IsNullOrEmpty(value))
            {
                SecureStorage.Default.Remove(key);
            }
            else
            {
                SecureStorage.Default.SetAsync(key, value);
            }
        }
        else
        {
            Preferences.Default.Set(key, value);
        }
    }

    internal static int? GetInt(string value)
    {
        bool success = int.TryParse(value, out int number);
        return success ? number : null;
    }

    internal static OutputMessage EnsureSpeechRecognizer(string speechRecognitionLanguageLocale)
    {
        OutputMessage statusMessage = null;

        if (App.SpeechRecognizer is null
            || App.SpeechConfig.SpeechRecognitionLanguage != speechRecognitionLanguageLocale)
        {
            App.SpeechConfig.SpeechRecognitionLanguage = speechRecognitionLanguageLocale;

            try
            {
                App.SpeechRecognizer = new SpeechRecognizer(App.SpeechConfig, App.AudioConfig);
            }
            catch (Exception ex)
            {
                statusMessage = GetErrorText(ex);
            }
        }

        return statusMessage;
    }
}