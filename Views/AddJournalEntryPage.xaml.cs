using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;
using System.Diagnostics;

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
        if (_journalId == 0) return;

        try
        {
            _editingJournal = await _databaseService.GetHealthJournalByIdAsync(_journalId);
            if (_editingJournal != null)
            {
                TitleEntry.Text = _editingJournal.Title ?? "";
                MoodEntry.Text = _editingJournal.Mood ?? "";
                SleepEntry.Text = _editingJournal.Sleep?.Replace(" hrs", "") ?? "";
                ExerciseEntry.Text = _editingJournal.Exercise ?? "";
                MedicationEntry.Text = _editingJournal.Medication ?? "";
                NotesEditor.Text = _editingJournal.Notes ?? "";
            }
        }
        catch (Exception ex) { Debug.WriteLine($"Load Error: {ex.Message}"); }
    }

    private void OnEnergyChanged(object sender, ValueChangedEventArgs e)
    {
        if (EnergyLabel != null)
            EnergyLabel.Text = $"Energy ({Math.Round(e.NewValue)}/10)";
    }

    private async void OnBackClicked(object sender, EventArgs e) => await GoBackSafe();
    private async void OnCancelClicked(object sender, EventArgs e) => await GoBackSafe();

    // --- NAVIGATION LOGIC (HYBRID FIX) ---
    private async Task GoBackSafe()
    {
        try
        {
            // Sulayan ang Shell Navigation (Standard)
            await Shell.Current.GoToAsync("..");
        }
        catch
        {
            // Kon mo-fail ang Shell (pananglitan wala gi-register ang route), gamit ang PopAsync
            if (Navigation.NavigationStack.Count > 0)
            {
                await Navigation.PopAsync();
            }
        }
    }

    private void ShowCustomAlert(string title, string message)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            // Siguroha nga dili NULL ang labels aron dili mo crashout
            if (AlertTitleLabel != null && AlertMessageLabel != null)
            {
                AlertTitleLabel.Text = title;
                AlertMessageLabel.Text = message;
                CustomAlertOverlay.IsVisible = true;
                CustomAlertOverlay.Opacity = 0;
                await CustomAlertOverlay.FadeTo(1, 150);
            }
        });
    }

    private async void OnCustomAlertOkClicked(object sender, EventArgs e)
    {
        await CustomAlertOverlay.FadeTo(0, 100);
        CustomAlertOverlay.IsVisible = false;

        // Mobalik ra kon malampuson ang Save/Update
        if (AlertTitleLabel.Text == "Saved" || AlertTitleLabel.Text == "Updated")
        {
            await GoBackSafe();
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        try
        {
            int userId = Preferences.Get("UserId", 0);
            string username = Preferences.Get("UserName", "User");

            string title = TitleEntry.Text?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(title))
            {
                ShowCustomAlert("Error", "Please enter a title.");
                return;
            }

            // Data Preparation
            string energyStr = $"{Math.Round(EnergySlider.Value)}/10";
            string sleepStr = string.IsNullOrWhiteSpace(SleepEntry.Text) ? "" : $"{SleepEntry.Text} hrs";

            if (_editingJournal != null)
            {
                _editingJournal.Title = title;
                _editingJournal.Mood = MoodEntry.Text ?? "";
                _editingJournal.Energy = energyStr;
                _editingJournal.Sleep = sleepStr;
                _editingJournal.Exercise = ExerciseEntry.Text ?? "";
                _editingJournal.Medication = MedicationEntry.Text ?? "";
                _editingJournal.Notes = NotesEditor.Text ?? "";

                await _databaseService.UpdateHealthJournalAsync(_editingJournal);
                ShowCustomAlert("Updated", "Journal entry updated successfully.");
            }
            else
            {
                var journal = new HealthJournal
                {
                    UserId = userId,
                    Username = username,
                    Title = title,
                    Date = DateTime.Now.ToString("yyyy-MM-dd"),
                    Mood = MoodEntry.Text ?? "",
                    Energy = energyStr,
                    Sleep = sleepStr,
                    Exercise = ExerciseEntry.Text ?? "",
                    Medication = MedicationEntry.Text ?? "",
                    Notes = NotesEditor.Text ?? ""
                };

                await _databaseService.SaveHealthJournalAsync(journal);
                ShowCustomAlert("Saved", "Journal entry saved successfully.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"CRASH LOG: {ex.Message}");
            ShowCustomAlert("Database Error", "Check database. Details: " + ex.Message);
        }
    }
}