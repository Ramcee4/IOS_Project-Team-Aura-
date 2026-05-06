using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace Team_Aura_Period_Tracker_;

public partial class HomePage : ContentPage
{
    private readonly DatabaseService _databaseService = new();

    public HomePage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        string userName = Preferences.Get("UserName", "User");
        int userId = Preferences.Get("UserId", 0);

        UserNameLabel.Text = $"{userName}!";

        if (userId == 0)
        {
            SetEmptyCycleState();
            return;
        }

        var cycle = await _databaseService.GetUserCycleInfoAsync(userId);

        if (cycle == null)
        {
            SetEmptyCycleState();
            return;
        }

        int cycleLengthDays = cycle.CycleLengthDays;
        int periodDays = cycle.PeriodDays;
        string lastDateOfPeriod = cycle.LastDateOfPeriod;

        Preferences.Set("LastDateOfPeriod", lastDateOfPeriod);
        Preferences.Set("CycleLengthDays", cycleLengthDays);
        Preferences.Set("PeriodDays", periodDays);
        Preferences.Set("CycleType", cycle.CycleType);

        CycleLengthValueLabel.Text = cycleLengthDays.ToString();
        PeriodDaysValueLabel.Text = periodDays.ToString();

        CycleLengthSubtitleLabel.Text = "days average";
        PeriodDaysSubtitleLabel.Text = "days average";

        UpdateWhatsComing(lastDateOfPeriod, cycleLengthDays, periodDays);
    }

    private void SetEmptyCycleState()
    {
        CycleLengthValueLabel.Text = "--";
        PeriodDaysValueLabel.Text = "--";

        CycleLengthSubtitleLabel.Text = "days average";
        PeriodDaysSubtitleLabel.Text = "days average";

        StartPeriodCountdownLabel.Text = "--";

        FertileWindowCountdownLabel.Text = "Not set";
        FertileWindowDayLabel.Text = "--";
        NextPeriodCountdownLabel.Text = "Not set";
        NextPeriodDayLabel.Text = "--";
    }

    private void UpdateWhatsComing(string lastDateOfPeriod, int cycleLengthDays, int periodDays)
    {
        if (string.IsNullOrWhiteSpace(lastDateOfPeriod) ||
            !DateTime.TryParse(lastDateOfPeriod, out DateTime lastPeriodDate) ||
            cycleLengthDays <= 0 ||
            periodDays <= 0)
        {
            SetEmptyCycleState();
            return;
        }

        DateTime today = DateTime.Today;

        int daysSinceLastPeriod = (today - lastPeriodDate.Date).Days;

        while (daysSinceLastPeriod < 0)
        {
            daysSinceLastPeriod += cycleLengthDays;
        }

        int currentCycleDay = (daysSinceLastPeriod % cycleLengthDays) + 1;

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

        int daysUntilNextPeriod = cycleLengthDays - currentCycleDay + 1;

        if (currentCycleDay <= periodDays)
        {
            StartPeriodCountdownLabel.Text = $"Day {currentCycleDay}";
            NextPeriodCountdownLabel.Text = $"In {cycleLengthDays - currentCycleDay + 1} days";
        }
        else
        {
            StartPeriodCountdownLabel.Text = daysUntilNextPeriod == 1
                ? "Tomorrow"
                : $"In {daysUntilNextPeriod} days";

            NextPeriodCountdownLabel.Text = daysUntilNextPeriod == 1
                ? "Tomorrow"
                : $"In {daysUntilNextPeriod} days";
        }

        FertileWindowCountdownLabel.Text = daysUntilFertileWindow == 0
            ? "Today"
            : $"In {daysUntilFertileWindow} days";

        FertileWindowDayLabel.Text = $"Day {fertileStartDay}–{fertileEndDay}";
        NextPeriodDayLabel.Text = $"Day {cycleLengthDays}";
    }

    private async void OnStartPeriodTapped(object sender, TappedEventArgs e)
    {
        int userId = Preferences.Get("UserId", 0);

        if (userId == 0)
        {
            await DisplayAlert("Error", "No user found.", "OK");
            return;
        }

        string result = await DisplayPromptAsync(
            "Start Period",
            "Enter the date your period started.\nFormat: yyyy-MM-dd",
            "Save",
            "Cancel",
            DateTime.Today.ToString("yyyy-MM-dd"),
            keyboard: Keyboard.Text);

        if (string.IsNullOrWhiteSpace(result))
            return;

        if (!DateTime.TryParse(result, out DateTime newPeriodDate))
        {
            await DisplayAlert("Invalid Date", "Please enter a valid date like 2026-05-06.", "OK");
            return;
        }

        string formattedDate = newPeriodDate.ToString("yyyy-MM-dd");

        await _databaseService.UpdateLastPeriodDateAsync(userId, formattedDate);

        Preferences.Set("LastDateOfPeriod", formattedDate);

        await DisplayAlert("Saved", "Your new period start date has been updated.", "OK");

        OnAppearing();
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