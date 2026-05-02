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
        string lastDateOfPeriod = Preferences.Get("LastDateOfPeriod", "");

        CycleLengthValueLabel.Text = cycleLengthDays.ToString();
        PeriodDaysValueLabel.Text = periodDays.ToString();

        CycleLengthSubtitleLabel.Text = "days average";
        PeriodDaysSubtitleLabel.Text = "days average";

        UpdateWhatsComing(lastDateOfPeriod, cycleLengthDays);
    }

    private void UpdateWhatsComing(string lastDateOfPeriod, int cycleLengthDays)
    {
        if (string.IsNullOrWhiteSpace(lastDateOfPeriod) ||
            !DateTime.TryParse(lastDateOfPeriod, out DateTime lastPeriodDate) ||
            cycleLengthDays <= 0)
        {
            FertileWindowCountdownLabel.Text = "Not set";
            FertileWindowDayLabel.Text = "--";
            NextPeriodCountdownLabel.Text = "Not set";
            NextPeriodDayLabel.Text = "--";
            return;
        }

        DateTime today = DateTime.Today;

        int daysSinceLastPeriod = (today - lastPeriodDate.Date).Days;

        while (daysSinceLastPeriod < 0)
        {
            daysSinceLastPeriod += cycleLengthDays;
        }

        int currentCycleDay = (daysSinceLastPeriod % cycleLengthDays) + 1;

        // More accurate estimate: ovulation is usually around 14 days before next period
        int ovulationDay = cycleLengthDays - 14;

        if (ovulationDay < 1)
        {
            ovulationDay = cycleLengthDays / 2;
        }

        int fertileStartDay = Math.Max(1, ovulationDay - 5);
        int fertileEndDay = Math.Min(cycleLengthDays, ovulationDay + 1);

        int daysUntilFertileWindow = fertileStartDay - currentCycleDay;

        if (daysUntilFertileWindow < 0)
        {
            daysUntilFertileWindow += cycleLengthDays;
        }

        int daysUntilNextPeriod = cycleLengthDays - currentCycleDay;

        if (daysUntilNextPeriod == 0)
        {
            daysUntilNextPeriod = cycleLengthDays;
        }

        FertileWindowCountdownLabel.Text = daysUntilFertileWindow == 0
            ? "Today"
            : $"In {daysUntilFertileWindow} days";

        FertileWindowDayLabel.Text = $"Day {fertileStartDay}–{fertileEndDay}";

        NextPeriodCountdownLabel.Text = $"In {daysUntilNextPeriod} days";
        NextPeriodDayLabel.Text = $"Day {cycleLengthDays}";
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
        await Navigation.PushAsync(new DailyPulseHistoryPage());
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

    private void OnHomeTapped(object sender, EventArgs e)
    {
        // Already on Home page
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