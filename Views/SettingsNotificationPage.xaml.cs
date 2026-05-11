using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Plugin.LocalNotification;

namespace Team_Aura_Period_Tracker_;

public partial class SettingsNotificationPage : ContentPage
{
    private bool _isLoading = true;

    public SettingsNotificationPage()
    {
        InitializeComponent();
        LoadUserPreferences();
    }

    private void LoadUserPreferences()
    {
        _isLoading = true;

        // I-load ang saved preferences, default is TRUE (On by default)
        PeriodSwitch.IsToggled = Preferences.Get("PeriodReminder", true);
        OvulationSwitch.IsToggled = Preferences.Get("OvulationReminder", true);
        FertileSwitch.IsToggled = Preferences.Get("FertileReminder", true);
        DailySymptomSwitch.IsToggled = Preferences.Get("DailyReminder", true);
        MedicationSwitch.IsToggled = Preferences.Get("MedicationReminder", true);
        ExerciseSwitch.IsToggled = Preferences.Get("ExerciseReminder", true);

        _isLoading = false;
    }

    private void OnSwitchToggled(object sender, ToggledEventArgs e)
    {
        if (_isLoading) return;

        var sw = (Switch)sender;

        if (sw == PeriodSwitch) Preferences.Set("PeriodReminder", e.Value);
        else if (sw == OvulationSwitch) Preferences.Set("OvulationReminder", e.Value);
        else if (sw == FertileSwitch) Preferences.Set("FertileReminder", e.Value);
        else if (sw == DailySymptomSwitch)
        {
            Preferences.Set("DailyReminder", e.Value);
            if (e.Value) ScheduleDailyPulseNotification();
            else LocalNotificationCenter.Current.Cancel(1337);
        }
        else if (sw == MedicationSwitch) Preferences.Set("MedicationReminder", e.Value);
        else if (sw == ExerciseSwitch) Preferences.Set("ExerciseReminder", e.Value);
    }

    private void ScheduleDailyPulseNotification()
    {
        TimeSpan notifyTime = TimeSpan.FromHours(20); // 8:00 PM

        var request = new NotificationRequest
        {
            NotificationId = 1337,
            Title = "Team Aura: Daily Pulse ?",
            Description = "It's 8:00 PM! Don't forget to log your symptoms today.",
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = DateTime.Today.Add(notifyTime),
                RepeatType = NotificationRepeat.Daily
            }
        };

        LocalNotificationCenter.Current.Show(request);
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}