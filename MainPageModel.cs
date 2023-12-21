using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.PronunciationAssessment;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace IKSPronounceApp;

internal partial class MainPageModel : ObservableObject
{
    [ObservableProperty] internal ObservableCollection<LanguageVoiceItem> languageVoiceItems;

    [ObservableProperty] internal ObservableCollection<OutputMessage> outputMessages = [];

    internal MainPageModel()
    {
        // Initialize lists of selectable values
        LanguageVoiceItems =
        [
            new("Chinese", "zh-Hant", "zh-TW"),
            new("English", "en", "en-US"),
            new("French", "fr", "fr-FR"),
            new ("German", "de", "de-DE"),
            new ("Italian", "it", "it-IT"),
            new ("Spanish", "es", "es-ES")
        ];
    }

    public static async Task<string> GetText(LanguageVoiceItem languageVoiceItem)
    {
        string resultText = null;

        var today = DateTime.Today;
        var year = today.Year.ToString("D4");
        var month = today.Month.ToString("D2");
        var day = today.Day.ToString("D2");

        var httpClient = new HttpClient();
        var url = $"https://{languageVoiceItem.LanguageCode}.wikipedia.org/api/rest_v1/feed/featured/{year}/{month}/{day}";
        var response = await httpClient.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            var contentJson = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(contentJson);

            var texts = jsonDoc.RootElement.GetProperty("onthisday");
            var numberOfTexts = texts.EnumerateArray().Count();

            // Pick random text
            var random = new Random();
            var randomNumber = random.Next(0, numberOfTexts);
            resultText = texts[randomNumber].GetProperty("text").ToString();
        }

        return resultText;
    }

    public async Task Speak(string text, string languageLocale)
    {
        if (string.IsNullOrEmpty(text)) { return; }

        // Ensure microphone access
        if (!await Util.EnsureMicrophonePermission()) { return; }

        OutputMessages.Clear();
        OutputMessages.Add(Util.GetInfoText("Listening..."));

        try
        {
            var statusMessage = Util.EnsureSpeechRecognizer(languageLocale);

            if (statusMessage is not null)
            {
                OutputMessages.RemoveAt(OutputMessages.Count - 1);
                OutputMessages.Add(statusMessage);
            }
            else
            {
                App.PronunciationConfig = new PronunciationAssessmentConfig(text, GradingSystem.HundredMark, Granularity.Word, true);
                App.PronunciationConfig.ApplyTo(App.SpeechRecognizer);

                var result = await App.SpeechRecognizer.RecognizeOnceAsync();

                OutputMessages.RemoveAt(OutputMessages.Count - 1);

                // Output results
                switch (result.Reason)
                {
                    case ResultReason.RecognizedSpeech:
                        var pronunciationResult = PronunciationAssessmentResult.FromResult(result);
                        OutputMessages.Add(new OutputMessage { Type = OutputMessageType.Score, Text = $"Overall Score: {pronunciationResult.PronunciationScore}", Score = pronunciationResult.PronunciationScore });
                        OutputMessages.Add(new OutputMessage { Type = OutputMessageType.Information });
                        OutputMessages.Add(new OutputMessage { Type = OutputMessageType.Score, Text = $"Accuracy: {pronunciationResult.AccuracyScore}", Score = pronunciationResult.AccuracyScore });
                        OutputMessages.Add(new OutputMessage { Type = OutputMessageType.Score, Text = $"Completeness: {pronunciationResult.CompletenessScore}", Score = pronunciationResult.CompletenessScore });
                        OutputMessages.Add(new OutputMessage { Type = OutputMessageType.Score, Text = $"Fluency: {pronunciationResult.FluencyScore}", Score = pronunciationResult.FluencyScore });
                        OutputMessages.Add(new OutputMessage { Type = OutputMessageType.Information });

                        foreach (var word in pronunciationResult.Words)
                        {
                            OutputMessages.Add(new OutputMessage { Type = OutputMessageType.Score, Text = word.Word + ": " + word.AccuracyScore + ((word.ErrorType != "None") ? " (" + word.ErrorType + ")" : string.Empty), Score = word.AccuracyScore });
                        }
                        break;

                    case ResultReason.NoMatch:
                        OutputMessages.Add(Util.GetNoMatchText());
                        break;

                    case ResultReason.Canceled:
                        OutputMessages.Add(Util.GetCanceledText(result));
                        break;

                    default:
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            OutputMessages.Add(Util.GetErrorText(ex));
        }
        finally
        {
        }
    }

    internal async Task Listen(string text, LanguageVoiceItem languageVoiceItem)
    {
        if (string.IsNullOrEmpty(text)) { return; }

        if (App.SpeechConfig.SpeechSynthesisLanguage != languageVoiceItem.LanguageLocale)
        {
            App.SpeechConfig.SpeechSynthesisLanguage = languageVoiceItem.LanguageLocale;
        }

        using var synthesizer = new SpeechSynthesizer(App.SpeechConfig);
        using var result = await synthesizer.SpeakTextAsync(text);

        switch (result.Reason)
        {
            case ResultReason.SynthesizingAudioCompleted:
                break;

            case ResultReason.Canceled:
                OutputMessages.Add(Util.GetCanceledText(result));
                break;

            default:
                break;
        }
    }
}
