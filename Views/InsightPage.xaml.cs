using System;
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

        if (cycle == null ||
            string.IsNullOrWhiteSpace(cycle.LastDateOfPeriod) ||
            cycle.CycleLengthDays <= 0 ||
            cycle.PeriodDays <= 0 ||
            !DateTime.TryParse(cycle.LastDateOfPeriod, out DateTime lastPeriod))
        {
            SetEmptyState();
            return;
        }

        int cycleLength = cycle.CycleLengthDays;
        int periodDays = cycle.PeriodDays;

        int daysSince = (DateTime.Today - lastPeriod.Date).Days;

        while (daysSince < 0)
        {
            daysSince += cycleLength;
        }

        int currentDay = (daysSince % cycleLength) + 1;

        int ovulationDay = cycleLength - 14;

        if (ovulationDay < 1)
        {
            ovulationDay = cycleLength / 2;
        }

        string phase = GetPhase(currentDay, periodDays, ovulationDay, cycleLength);

        PhaseLabel.Text = phase;
        DayLabel.Text = $"Day {currentDay} of {cycleLength}";
        PhaseTipLabel.Text = GetPhaseTip(phase);

        int daysUntilOvulation = ovulationDay - currentDay;

        if (daysUntilOvulation < 0)
        {
            daysUntilOvulation += cycleLength;
        }

        OvulationLabel.Text = daysUntilOvulation == 0
            ? $"Today (Day {ovulationDay})"
            : $"In {daysUntilOvulation} days (Day {ovulationDay})";

        int daysUntilNextPeriod = cycleLength - currentDay + 1;

        if (currentDay <= periodDays)
        {
            NextPeriodLabel.Text = $"Current period day {currentDay}. Next cycle in {daysUntilNextPeriod} days.";
        }
        else
        {
            NextPeriodLabel.Text = daysUntilNextPeriod == 1
                ? $"Tomorrow (Day {cycleLength})"
                : $"In {daysUntilNextPeriod} days (Day {cycleLength})";
        }
    }

    private void SetEmptyState()
    {
        PhaseLabel.Text = "Not set";
        DayLabel.Text = "Complete cycle information first.";
        PhaseTipLabel.Text = "Go back to setup and enter your last period date, cycle length, and period days.";
        OvulationLabel.Text = "Not set";
        NextPeriodLabel.Text = "Not set";
    }

    private string GetPhase(int currentDay, int periodDays, int ovulationDay, int cycleLength)
    {
        int fertileStartDay = Math.Max(1, ovulationDay - 5);
        int fertileEndDay = Math.Min(cycleLength, ovulationDay + 1);

        if (currentDay <= periodDays)
            return "Menstrual Phase";

        if (currentDay >= fertileStartDay && currentDay <= fertileEndDay)
            return "Fertile Window";

        if (currentDay < fertileStartDay)
            return "Follicular Phase";

        return "Luteal Phase";
    }

    private string GetPhaseTip(string phase)
    {
        return phase switch
        {
            "Menstrual Phase" => "Rest and recover. Your body is renewing.",
            "Follicular Phase" => "Energy is rising. Great time to start new things.",
            "Fertile Window" => "Fertility is higher during this window.",
            "Luteal Phase" => "Slow down and take care of yourself.",
            _ => ""
        };
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnHomeTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new HomePage());
    }

    private async void OnDailyTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new DailyLogPage());
    }

    private async void OnLearnTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new LearnPage());
    }

    private async void OnSettingsTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new SettingsPage());
    }
}