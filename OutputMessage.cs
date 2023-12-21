using CommunityToolkit.Mvvm.ComponentModel;

namespace IKSPronounceApp;

internal partial class OutputMessage : ObservableObject
{
    [ObservableProperty] private OutputMessageType type;
    [ObservableProperty] private string text;
    [ObservableProperty] private double score;

    public Color Color
    {
        get
        {
            // Create a gradient from green to red, shifted to more red

            int r = (int)(100 - Score) * 255 / 100;
            int g = (int)Score * 255 / 100;
            int b = 0;

            int shift = 70;

            r += shift;
            if (r > 255) r = 255;

            g -= shift;
            if (g < 0) g = 0;

            return new Color(r, g, b);
        }
    }
}
