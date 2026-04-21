using System;
using Microsoft.Maui.Controls;

namespace Team_Aura_Period_Tracker_;

public partial class HomePage : ContentPage
{
    public HomePage()
    {
        InitializeComponent();
    }

    private async void OnBellClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Notifications", "Open notifications page.", "OK");
    }

    private async void OnCycleLengthTapped(object sender, EventArgs e)
    {
        await DisplayAlert("Cycle Length", "Open cycle length details.", "OK");
    }

    private async void OnPeriodDaysTapped(object sender, EventArgs e)
    {
        await DisplayAlert("Period Days", "Open period days details.", "OK");
    }

    private async void OnLogTodayTapped(object sender, EventArgs e)
    {
        await DisplayAlert("Log Today", "Open log today page.", "OK");
    }

    private async void OnInsightsTapped(object sender, EventArgs e)
    {
        await DisplayAlert("Insights", "Open insights page.", "OK");
    }

    private async void OnJournalTapped(object sender, EventArgs e)
    {
        await DisplayAlert("Journal", "Open journal page.", "OK");
    }

    private async void OnLearnTapped(object sender, EventArgs e)
    {
        await DisplayAlert("Learn", "Open learn page.", "OK");
    }

    private async void OnWhatsComingTapped(object sender, EventArgs e)
    {
        await DisplayAlert("What’s Coming", "Open upcoming events page.", "OK");
    }

    private async void OnHomeTapped(object sender, EventArgs e)
    {
        await DisplayAlert("Home", "You are already on the Home page.", "OK");
    }

    private async void OnDailyTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new DailyLogPage());
    }

    private async void OnInsightNavTapped(object sender, EventArgs e)
    {
        await DisplayAlert("Insight", "Open insight page.", "OK");
    }

    private async void OnLearnNavTapped(object sender, EventArgs e)
    {
        await DisplayAlert("Learn", "Open learn page.", "OK");
    }

    private async void OnSettingsTapped(object sender, EventArgs e)
    {
        await DisplayAlert("Settings", "Open settings page.", "OK");
    }
}