using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace Team_Aura_Period_Tracker_;

public partial class HomePage : ContentPage
{
    public HomePage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        string userName = Preferences.Get("UserName", "User");
        UserNameLabel.Text = $"{userName}!";

        int cycleLengthDays = Preferences.Get("CycleLengthDays", 28);
        int periodDays = Preferences.Get("PeriodDays", 5);

        CycleLengthValueLabel.Text = cycleLengthDays.ToString();
        PeriodDaysValueLabel.Text = periodDays.ToString();

        CycleLengthSubtitleLabel.Text = "days average";
        PeriodDaysSubtitleLabel.Text = "days average";
    }

    private async void OnBellClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Notifications", "Open notifications page.", "OK");
    }

    private async void OnCycleLengthTapped(object sender, EventArgs e)
    {
        await DisplayAlert("Cycle Length", $"Your cycle length is {CycleLengthValueLabel.Text} days.", "OK");
    }

    private async void OnPeriodDaysTapped(object sender, EventArgs e)
    {
        await DisplayAlert("Period Days", $"Your period lasts {PeriodDaysValueLabel.Text} days.", "OK");
    }

    private async void OnLogTodayTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new DailyLogPage());
    }

    private async void OnInsightsTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new InsightPage());
    }

    private async void OnJournalTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new JournalPage());
    }

    private async void OnLearnTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new LearnPage());
    }

    private async void OnWhatsComingTapped(object sender, EventArgs e)
    {
        await DisplayAlert("What’s Coming", "Open upcoming events page.", "OK");
    }

    private async void OnHomeTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//HomePage");
    }

    private async void OnDailyTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new DailyLogPage());
    }

    private async void OnInsightNavTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new InsightPage());
    }

    private async void OnLearnNavTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new LearnPage());
    }

    private async void OnSettingsTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SettingsPage());
    }
}