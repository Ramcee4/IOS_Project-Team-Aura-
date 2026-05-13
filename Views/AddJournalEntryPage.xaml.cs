using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace Team_Aura_Period_Tracker_;

public partial class AddJournalEntryPage : ContentPage
{
    private readonly DatabaseService _databaseService = new();
    private readonly int _journalId;
    private HealthJournal? _editingJournal;

    public AddJournalEntryPage()
    {
        InitializeComponent();
        _journalId = 0;
    }

    public AddJournalEntryPage(int journalId)
    {
        InitializeComponent();
        _journalId = journalId;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_journalId == 0)
            return;

        _editingJournal = await _databaseService.GetHealthJournalByIdAsync(_journalId);

        if (_editingJournal == null)
            return;

        TitleEntry.Text = _editingJournal.Title;
        MoodEntry.Text = _editingJournal.Mood;
        SleepEntry.Text = _editingJournal.Sleep.Replace(" hrs", "");
        ExerciseEntry.Text = _editingJournal.Exercise;
        MedicationEntry.Text = _editingJournal.Medication;
        NotesEditor.Text = _editingJournal.Notes;

        if (_editingJournal.Energy.Contains("/"))
        {
            string energyValue = _editingJournal.Energy.Split('/')[0];

            if (double.TryParse(energyValue, out double energy))
            {
                EnergySlider.Value = energy;
                EnergyLabel.Text = $"Energy ({Math.Round(energy)}/10)";
            }
        }
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

    // --- CUSTOM ALERT LOGIC ---
    private async void ShowCustomAlert(string title, string message)
    {
        AlertTitleLabel.Text = title;
        AlertMessageLabel.Text = message;
        CustomAlertOverlay.IsVisible = true;
        CustomAlertOverlay.Opacity = 0;
        await CustomAlertOverlay.FadeTo(1, 150);
    }

    private async void OnCustomAlertOkClicked(object sender, EventArgs e)
    {
        await CustomAlertOverlay.FadeTo(0, 150);
        CustomAlertOverlay.IsVisible = false;

        // Kung malampuson ang pag-save o update, balik sa history page
        if (AlertTitleLabel.Text == "Saved" || AlertTitleLabel.Text == "Updated")
        {
            await Navigation.PopAsync();
        }
    }

    // --- SAVE LOGIC ---
    private async void OnSaveClicked(object sender, EventArgs e)
    {
        int userId = Preferences.Get("UserId", 0);
        string username = Preferences.Get("UserName", "");

        if (userId == 0 || string.IsNullOrWhiteSpace(username))
        {
            ShowCustomAlert("Error", "No signed in user found.");
            return;
        }

        string title = TitleEntry.Text?.Trim() ?? "";
        string mood = MoodEntry.Text?.Trim() ?? "";
        string sleep = SleepEntry.Text?.Trim() ?? "";
        string exercise = ExerciseEntry.Text?.Trim() ?? "";
        string medication = MedicationEntry.Text?.Trim() ?? "";
        string notes = NotesEditor.Text?.Trim() ?? "";
        string energy = $"{Math.Round(EnergySlider.Value)}/10";

        if (string.IsNullOrWhiteSpace(title))
        {
            ShowCustomAlert("Error", "Please enter a title.");
            return;
        }

        if (_editingJournal != null)
        {
            _editingJournal.Username = username;
            _editingJournal.Title = title;
            _editingJournal.Mood = mood;
            _editingJournal.Energy = energy;
            _editingJournal.Sleep = string.IsNullOrWhiteSpace(sleep) ? "" : $"{sleep} hrs";
            _editingJournal.Exercise = exercise;
            _editingJournal.Medication = medication;
            _editingJournal.Notes = notes;

            await _databaseService.UpdateHealthJournalAsync(_editingJournal);

            ShowCustomAlert("Updated", "Journal entry updated successfully.");
            return;
        }

        var journal = new HealthJournal
        {
            UserId = userId,
            Username = username,
            Title = title,
            Date = DateTime.Now.ToString("yyyy-MM-dd"),
            Mood = mood,
            Energy = energy,
            Sleep = string.IsNullOrWhiteSpace(sleep) ? "" : $"{sleep} hrs",
            Exercise = exercise,
            Medication = medication,
            Notes = notes
        };

        await _databaseService.SaveHealthJournalAsync(journal);

        ShowCustomAlert("Saved", "Journal entry saved successfully.");
    }
}