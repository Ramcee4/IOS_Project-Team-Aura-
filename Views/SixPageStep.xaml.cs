using Microsoft.Maui.Storage;

namespace Team_Aura_Period_Tracker_;

public partial class SixPageStep : ContentPage
{
    public SixPageStep()
    {
        InitializeComponent();

        string storedName = Preferences.Get("userName", "");

        if (string.IsNullOrWhiteSpace(storedName))
        {
            WelcomeDoneLabel.Text =
                "Welcome to Aura. Your cycle tracking journey starts now. Remember, this app is a tool for awareness and pattern recognition - not a replacement for medical advice.";
        }
        else
        {
            WelcomeDoneLabel.Text =
                $"Welcome to Aura, {storedName}. Your cycle tracking journey starts now. Remember, this app is a tool for awareness and pattern recognition - not a replacement for medical advice.";
        }
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnGetStartedClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Aura", "Setup complete!", "OK");
    }
}