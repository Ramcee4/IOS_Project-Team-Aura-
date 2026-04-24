using System;
using Microsoft.Maui.Controls;

namespace Team_Aura_Period_Tracker_;

public partial class AddJournalEntryPage : ContentPage
{
    public AddJournalEntryPage()
    {
        InitializeComponent();
    }

    private void OnEnergyChanged(object sender, ValueChangedEventArgs e)
    {
        EnergyLabel.Text = $"Energy ({Math.Round(e.NewValue)}/10)";
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        string title = TitleEntry.Text?.Trim() ?? "";
        string mood = MoodEntry.Text?.Trim() ?? "";
        string sleep = SleepEntry.Text?.Trim() ?? "";
        string exercise = ExerciseEntry.Text?.Trim() ?? "";
        string medication = MedicationEntry.Text?.Trim() ?? "";
        string notes = NotesEditor.Text?.Trim() ?? "";
        string energy = $"{Math.Round(EnergySlider.Value)}/10";

        if (string.IsNullOrWhiteSpace(title))
        {
            await DisplayAlert("Error", "Please enter a title.", "OK");
            return;
        }

        await DisplayAlert("Saved", "Journal entry saved.", "OK");

        await Navigation.PopAsync();
    }
}