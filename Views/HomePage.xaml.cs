using System;
using System.Linq;
using System.Threading.Tasks;
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

        // I-sync ang preferences gikan sa database result
        int cycleLengthDays = cycle.CycleLengthDays;
        int periodDays = cycle.PeriodDays;
        string lastDateOfPeriod = cycle.LastDateOfPeriod;

        Preferences.Set("LastDateOfPeriod", lastDateOfPeriod);
        Preferences.Set("CycleLengthDays", cycleLengthDays);
        Preferences.Set("PeriodDays", periodDays);

        // Tawgon ang logic para sa Dashboard Updates
        UpdateWhatsComing(lastDateOfPeriod, cycleLengthDays, periodDays);

        // Check for Red Dot notification
        await CheckForNotifications(userId, lastDateOfPeriod, cycleLengthDays);
    }

    private void SetEmptyCycleState()
    {
        CycleLengthValueLabel.Text = "--";
        PeriodDaysValueLabel.Text = "5"; // Fixed base sa imong gusto
        CycleLengthSubtitleLabel.Text = "days average";
        PeriodDaysSubtitleLabel.Text = "days average";
        StartPeriodCountdownLabel.Text = "--";
        FertileWindowCountdownLabel.Text = "Not set";
        FertileWindowDayLabel.Text = "--";
        NextPeriodCountdownLabel.Text = "Not set";
        NextPeriodDayLabel.Text = "--";
        NotificationDot.IsVisible = false;
    }

    private void UpdateWhatsComing(string lastDateOfPeriod, int cycleLengthDays, int periodDays)
    {
        // 1. SAFETY CHECK
        if (string.IsNullOrWhiteSpace(lastDateOfPeriod) ||
            lastDateOfPeriod == "string.Empty" ||
            !DateTime.TryParse(lastDateOfPeriod, out DateTime lastPeriodDate))
        {
            SetEmptyCycleState();
            return;
        }

        DateTime today = DateTime.Today;

        // Timaan kung karon ba gyud gipislit ang button sa Home (Reset Trigger)
        bool isPeriodStartedToday = lastPeriodDate.Date == today;

        // 2. BASIC CALCULATIONS (Base gyud sa Input Date)
        int ovulationDay = cycleLengthDays - 14;
        if (ovulationDay < 1) ovulationDay = cycleLengthDays / 2;

        int fertileStartDay = Math.Max(1, ovulationDay - 5);
        int fertileEndDay = Math.Min(cycleLengthDays, ovulationDay + 1);

        // --- KANI ANG FIX PARA SA DATES ---
        // Mag-ihap ta diretso gikan sa lastPeriodDate (e.g., May 1)
        DateTime fertileTargetDate = lastPeriodDate.AddDays(fertileStartDay - 1);
        DateTime nextPeriodTargetDate = lastPeriodDate.AddDays(cycleLengthDays);

        // Countdown base sa Today
        int daysUntilFertileWindow = (fertileTargetDate - today).Days;
        int daysUntilNextPeriod = (nextPeriodTargetDate - today).Days;

        // 3. PERIOD DAYS CARD LOGIC
        if (isPeriodStartedToday)
        {
            // Mahimo lang 5 kung gipislit ang button karon
            PeriodDaysValueLabel.Text = "5";
            PeriodDaysSubtitleLabel.Text = "Day 1 of 5";
            PeriodDaysValueLabel.TextColor = Color.FromArgb("#E85B73");
        }
        else
        {
            // I-display ang gi-input sa Step 3 (e.g., 7 o unsa man to)
            PeriodDaysValueLabel.Text = periodDays.ToString();
            PeriodDaysSubtitleLabel.Text = "days average";
            PeriodDaysValueLabel.TextColor = Color.FromArgb("#333333");
        }

        // 4. CYCLE LENGTH CARD
        CycleLengthValueLabel.Text = cycleLengthDays.ToString();
        CycleLengthSubtitleLabel.Text = "days average";

        // 5. START PERIOD BUTTON (Clean)
        StartPeriodCountdownLabel.Text = "--";

        // 6. WHAT'S COMING SECTION (Direct Display)
        // Fertile Window
        if (daysUntilFertileWindow == 0) FertileWindowCountdownLabel.Text = $"Today ({fertileTargetDate:MMM dd})";
        else if (daysUntilFertileWindow > 0) FertileWindowCountdownLabel.Text = $"In {daysUntilFertileWindow} days ({fertileTargetDate:MMM dd})";
        else FertileWindowCountdownLabel.Text = $"Started ({fertileTargetDate:MMM dd})"; // Kung nilabay na gamay

        FertileWindowDayLabel.Text = $"Day {fertileStartDay}–{fertileEndDay}";

        // Next Period
        if (daysUntilNextPeriod == 1) NextPeriodCountdownLabel.Text = $"Tomorrow ({nextPeriodTargetDate:MMM dd})";
        else if (daysUntilNextPeriod > 0) NextPeriodCountdownLabel.Text = $"In {daysUntilNextPeriod} days ({nextPeriodTargetDate:MMM dd})";
        else NextPeriodCountdownLabel.Text = $"Due ({nextPeriodTargetDate:MMM dd})";

        NextPeriodDayLabel.Text = $"Day {cycleLengthDays}";
    }
    private void OnStartPeriodTapped(object sender, TappedEventArgs e)
    {
        // I-show ang stylish alert overlay
        StartPeriodAlertOverlay.IsVisible = true;
    }

    private void OnCancelAlertClicked(object sender, EventArgs e)
    {
        // Undo button: I-hide ang popup
        StartPeriodAlertOverlay.IsVisible = false;
    }

    private async void OnConfirmStartClicked(object sender, EventArgs e)
    {
        // Yes button: I-update ang date to TODAY
        StartPeriodAlertOverlay.IsVisible = false;
        int userId = Preferences.Get("UserId", 0);
        if (userId == 0) return;

        string todayFormatted = DateTime.Today.ToString("yyyy-MM-dd");

        await _databaseService.UpdateLastPeriodDateAsync(userId, todayFormatted);
        Preferences.Set("LastDateOfPeriod", todayFormatted);
        Preferences.Set("NotificationBellClicked", false); // Refresh the dot

        // Refresh ang tibuok dashboard UI
        OnAppearing();
    }

    // --- NOTIFICATIONS & NAVIGATION ---

    private async Task CheckForNotifications(int userId, string lastDate, int cycleLength)
    {
        var logs = await _databaseService.GetDailyLogsByUserAsync(userId);
        string today = DateTime.Now.ToString("yyyy-MM-dd");
        bool hasLoggedToday = logs.Any(l => l.Date == today);

        bool isPeriodComing = false;
        if (DateTime.TryParse(lastDate, out DateTime lastPeriod))
        {
            int daysSince = (DateTime.Today - lastPeriod.Date).Days;
            while (daysSince < 0) daysSince += cycleLength;
            int currentDay = (daysSince % cycleLength) + 1;
            int daysUntilNext = cycleLength - currentDay + 1;
            if (daysUntilNext <= 3) isPeriodComing = true;
        }

        bool wasClicked = Preferences.Get("NotificationBellClicked", false);
        NotificationDot.IsVisible = (!hasLoggedToday || isPeriodComing) && !wasClicked;
    }

    private async void OnBellClicked(object sender, EventArgs e)
    {
        NotificationDot.IsVisible = false;
        Preferences.Set("NotificationBellClicked", true);
        await Navigation.PushAsync(new NotificationsPage());
    }

    private async void OnLogTodayTapped(object sender, EventArgs e) => await Navigation.PushAsync(new DailyPulseHistoryPage());
    private async void OnInsightsTapped(object sender, EventArgs e) => await Navigation.PushAsync(new InsightPage());
    private async void OnJournalTapped(object sender, EventArgs e) => await Navigation.PushAsync(new JournalPage());
    private async void OnLearnTapped(object sender, EventArgs e) => await Navigation.PushAsync(new LearnPage());

    // Bottom Nav
    private void OnHomeTapped(object sender, EventArgs e) { /* Current Page */ }
    private async void OnDailyTapped(object sender, EventArgs e) => await Navigation.PushAsync(new DailyLogPage());
    private async void OnInsightNavTapped(object sender, EventArgs e) => await Navigation.PushAsync(new InsightPage());
    private async void OnLearnNavTapped(object sender, EventArgs e) => await Navigation.PushAsync(new LearnPage());
    private async void OnSettingsTapped(object sender, EventArgs e) => await Navigation.PushAsync(new SettingsPage());

    // Stat Alerts
    // --- CUSTOM STAT ALERTS (CONSISTENT STYLE) ---

    private void OnCycleLengthTapped(object sender, EventArgs e)
    {
        StatAlertEmoji.Text = "?";
        StatAlertTitle.Text = "Cycle Length";
        StatAlertMessage.Text = $"Your average cycle length is {CycleLengthValueLabel.Text} days. This is the time from the first day of one period to the first day of the next.";
        StatInfoAlertOverlay.IsVisible = true;
    }

    private void OnPeriodDaysTapped(object sender, EventArgs e)
    {
        StatAlertEmoji.Text = "?";
        StatAlertTitle.Text = "Period Days";
        StatAlertMessage.Text = $"Your average period duration is {PeriodDaysValueLabel.Text} days. This tracks how long your bleeding typically lasts each month.";
        StatInfoAlertOverlay.IsVisible = true;
    }

    private void OnCloseStatAlertClicked(object sender, EventArgs e)
    {
        StatInfoAlertOverlay.IsVisible = false;
    }
}