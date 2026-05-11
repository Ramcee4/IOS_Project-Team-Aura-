using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace Team_Aura_Period_Tracker_;

public partial class InsightPage : ContentPage
{
    private readonly DatabaseService _databaseService = new();

    public InsightPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        int userId = Preferences.Get("UserId", 0);
        if (userId == 0)
        {
            SetEmptyState();
            return;
        }

        var cycle = await _databaseService.GetUserCycleInfoAsync(userId);
        var logs = await _databaseService.GetDailyLogsByUserAsync(userId);
        var journals = await _databaseService.GetHealthJournalsByUserAsync(userId);

        if (cycle == null || !DateTime.TryParse(cycle.LastDateOfPeriod, out DateTime lastPeriod))
        {
            SetEmptyState();
            return;
        }

        // 1. CYCLE CALCULATION (Consistent sa imong logic)
        int cycleLength = cycle.CycleLengthDays;
        int periodDays = cycle.PeriodDays;
        int daysSince = (DateTime.Today - lastPeriod.Date).Days;
        while (daysSince < 0) daysSince += cycleLength;
        int currentDay = (daysSince % cycleLength) + 1;
        int ovulationDay = cycleLength - 14;
        if (ovulationDay < 1) ovulationDay = cycleLength / 2;

        // 2. SET PHASE & DATES
        string phase = GetPhase(currentDay, periodDays, ovulationDay, cycleLength);
        PhaseLabel.Text = phase;
        DayLabel.Text = $"Day {currentDay} of {cycleLength}";

        // 3. GENERATE PERSONALIZED INSIGHT (The new logic)
        PhaseTipLabel.Text = GenerateSmartInsight(phase, logs, journals);

        // 4. UPDATE REMAINING LABELS
        UpdateDatesUI(currentDay, ovulationDay, cycleLength, periodDays);
    }

    private string GenerateSmartInsight(string phase, List<DailyLog> logs, List<HealthJournal> journals)
    {
        string baseTip = GetPhaseTip(phase);
        var recentLogs = logs.Take(7).ToList(); // Tan-awon nato ang last 7 days entries
        var recentJournals = journals.Take(7).ToList();

        if (!recentLogs.Any() && !recentJournals.Any()) return baseTip;

        // --- 1. PHYSICAL SYMPTOM CHECKING ---
        // Atong i-check ang mga keyword base sa imong buttons
        bool hasPain = recentLogs.Any(l => l.Symptoms.Contains("cramps", StringComparison.OrdinalIgnoreCase) ||
                                           l.Symptoms.Contains("Soreness", StringComparison.OrdinalIgnoreCase) ||
                                           l.Symptoms.Contains("Dull", StringComparison.OrdinalIgnoreCase));

        bool hasStomachIssue = recentLogs.Any(l => l.Symptoms.Contains("Bloating", StringComparison.OrdinalIgnoreCase) ||
                                                  l.Symptoms.Contains("Nausea", StringComparison.OrdinalIgnoreCase) ||
                                                  l.Symptoms.Contains("Craving", StringComparison.OrdinalIgnoreCase));

        bool hasSkinOrHeat = recentLogs.Any(l => l.Symptoms.Contains("acne", StringComparison.OrdinalIgnoreCase) ||
                                                l.Symptoms.Contains("Oily", StringComparison.OrdinalIgnoreCase) ||
                                                l.Symptoms.Contains("Heat", StringComparison.OrdinalIgnoreCase));

        // --- 2. EMOTIONAL & ENERGY CHECKING ---
        bool isLowMood = recentLogs.Any(l => l.Symptoms.Contains("Blue", StringComparison.OrdinalIgnoreCase) ||
                                             l.Symptoms.Contains("Moody", StringComparison.OrdinalIgnoreCase) ||
                                             l.Symptoms.Contains("Anxiety", StringComparison.OrdinalIgnoreCase)) ||
                         recentJournals.Any(j => j.Mood.Contains("sad", StringComparison.OrdinalIgnoreCase) ||
                                                 j.Mood.Contains("anxious", StringComparison.OrdinalIgnoreCase));

        bool isExhausted = recentLogs.Any(l => l.Symptoms.Contains("Fatigue", StringComparison.OrdinalIgnoreCase) ||
                                               l.Symptoms.Contains("Weak", StringComparison.OrdinalIgnoreCase)) ||
                           recentJournals.Any(j => j.Energy.StartsWith("1") || j.Energy.StartsWith("2") || j.Energy.StartsWith("3"));

        // --- 3. SMART ADVICE GENERATOR ---
        // Menstrual Phase + Pain
        if (phase == "Menstrual Phase" && hasPain)
            return "You've logged pain symptoms like cramps. Try anti-inflammatory foods, ginger tea, and keep your abdomen warm.";

        // Luteal Phase (PMS) + Mood/Skin
        if (phase == "Luteal Phase")
        {
            if (hasSkinOrHeat) return "Hormonal shifts are causing oily skin or acne. Keep hydrated and use a gentle cleanser.";
            if (isLowMood) return "You've been feeling moody or anxious. This is common before your period. Try deep breathing or light walking.";
        }

        // Any Phase + Exhaustion
        if (isExhausted)
            return "You're logging high fatigue. Your body needs more iron and rest. Aim for at least 8 hours of sleep tonight.";

        // Any Phase + Stomach Issues
        if (hasStomachIssue)
            return "Bloating or nausea detected. Avoid salty foods and try peppermint tea to soothe your digestive system.";

        return baseTip;
    }

    private void UpdateDatesUI(int currentDay, int ovulationDay, int cycleLength, int periodDays)
    {
        int daysUntilOvulation = ovulationDay - currentDay;
        if (daysUntilOvulation < 0) daysUntilOvulation += cycleLength;
        OvulationLabel.Text = daysUntilOvulation == 0 ? "Today" : $"In {daysUntilOvulation} days";

        int daysUntilNextPeriod = cycleLength - currentDay + 1;
        NextPeriodLabel.Text = currentDay <= periodDays
            ? $"Current period day {currentDay}."
            : $"In {daysUntilNextPeriod} days.";
    }

    // --- SUPPORT METHODS (Consistent style) ---
    private string GetPhase(int currentDay, int periodDays, int ovulationDay, int cycleLength)
    {
        int fertileStartDay = Math.Max(1, ovulationDay - 5);
        int fertileEndDay = Math.Min(cycleLength, ovulationDay + 1);
        if (currentDay <= periodDays) return "Menstrual Phase";
        if (currentDay >= fertileStartDay && currentDay <= fertileEndDay) return "Fertile Window";
        return currentDay < fertileStartDay ? "Follicular Phase" : "Luteal Phase";
    }

    private string GetPhaseTip(string phase)
    {
        return phase switch
        {
            "Menstrual Phase" => "Rest and recover. Your body is renewing.",
            "Follicular Phase" => "Energy is rising. Great time to start new things.",
            "Fertile Window" => "Fertility is higher during this window.",
            "Luteal Phase" => "Slow down and take care of yourself.",
            _ => "Keep tracking to see your pattern."
        };
    }

    private void SetEmptyState()
    {
        PhaseLabel.Text = "Not set";
        DayLabel.Text = "Complete cycle information first.";
        PhaseTipLabel.Text = "Go back to setup and enter your cycle details.";
    }

    // --- NAVIGATION (UNCHANGED) ---
    private async void OnBackClicked(object sender, EventArgs e) => await Navigation.PopAsync();
    private async void OnHomeTapped(object sender, TappedEventArgs e) => await Navigation.PushAsync(new HomePage());
    private async void OnDailyTapped(object sender, TappedEventArgs e) => await Navigation.PushAsync(new DailyLogPage());
    private async void OnLearnTapped(object sender, TappedEventArgs e) => await Navigation.PushAsync(new LearnPage());
    private async void OnSettingsTapped(object sender, TappedEventArgs e) => await Navigation.PushAsync(new SettingsPage());
}