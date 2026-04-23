using System;
using Microsoft.Maui.Controls;

namespace Team_Aura_Period_Tracker_;

public partial class AddJournalEntryPage : ContentPage
{
    public AddJournalEntryPage()
    {
        InitializeComponent();
    }

    private void OnEnergyValueChanged(object sender, ValueChangedEventArgs e)
    {
        EnergyLabel.Text = $"Energy ({Math.Round(e.NewValue)}/10)";
    }

    private async void OnBackTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        var entry = new JournalEntry
        {
            Title = string.IsNullOrWhiteSpace(TitleEntry.Text) ? "New Entry" : TitleEntry.Text.Trim(),
            DateText = DateTime.Now.ToString("yyyy-MM-dd"),
            Mood = string.IsNullOrWhiteSpace(MoodEntry.Text) ? "Not set" : MoodEntry.Text.Trim(),
            Energy = $"{Math.Round(EnergySlider.Value)}/10",
            Sleep = string.IsNullOrWhiteSpace(SleepEntry.Text) ? "Not set" : $"{SleepEntry.Text.Trim()} hrs",
            Exercise = string.IsNullOrWhiteSpace(ExerciseEntry.Text) ? "Not set" : ExerciseEntry.Text.Trim(),
            Medication = string.IsNullOrWhiteSpace(MedicationEntry.Text) ? "None" : MedicationEntry.Text.Trim(),
            Notes = string.IsNullOrWhiteSpace(NotesEditor.Text) ? "No notes added." : NotesEditor.Text.Trim()
        };

        JournalEntryStore.Entries.Insert(0, entry);

        await DisplayAlert("Saved", "Journal entry saved.", "OK");
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

    private async void OnInsightTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new InsightPage());
    }

    private async void OnLearnTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new LearnPage());
    }

    private async void OnSettingsTapped(object sender, TappedEventArgs e)
    {
        await DisplayAlert("Settings", "Open settings page.", "OK");
    }
}