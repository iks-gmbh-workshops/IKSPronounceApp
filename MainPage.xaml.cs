namespace IKSPronounceApp;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private void BtnClose_Clicked(object sender, EventArgs e)
    {
        Application.Current.Quit();
    }
}

