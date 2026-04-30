using Microsoft.Maui.Storage;

namespace Team_Aura_Period_Tracker_;

public partial class Step1Page : ContentPage
{
    public Step1Page()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        string storedName = Preferences.Get("UserName", "");

        if (string.IsNullOrWhiteSpace(storedName))
        {
            WelcomeLabel.Text = "Welcome!";
        }
        else
        {
            WelcomeLabel.Text = $"Welcome, {storedName}!";
        }
    }

    private async void OnNextClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(Step2Page));
    }
}