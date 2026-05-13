using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Storage;

namespace Team_Aura_Period_Tracker_;

public partial class DailyLogPage : ContentPage
{
    private Button? _selectedFlowButton;
    private Border? _selectedMoodBorder;
    private readonly HashSet<Button> _selectedSymptoms = new();

    private readonly DatabaseService _databaseService = new();

    public DailyLogPage()
    {
        InitializeComponent();
    }

    private void ResetFlowButtons()
    {
        Button[] buttons = { SpottingButton, LightButton, ModerateButton, HeavyButton };

        foreach (var button in buttons)
        {
            button.BackgroundColor = Color.FromArgb("#F8F8F8");
            button.TextColor = Color.FromArgb("#333333");
            button.BorderColor = Color.FromArgb("#222222");
        }
    }

    private void OnFlowClicked(object sender, EventArgs e)
    {
        if (sender is not Button button)
            return;

        ResetFlowButtons();

        button.BackgroundColor = Color.FromArgb("#E85B73");
        button.TextColor = Colors.White;
        button.BorderColor = Color.FromArgb("#E85B73");

        _selectedFlowButton = button;
    }

    private void ResetMoodBorders()
    {
        Border[] moods = { Mood1Border, Mood2Border, Mood3Border, Mood4Border };

        foreach (var mood in moods)
        {
            mood.BackgroundColor = Color.FromArgb("#F8F8F8");
            mood.Stroke = Color.FromArgb("#2B2B2B");
        }
    }

    private void SelectMood(Border border)
    {
        ResetMoodBorders();

        border.BackgroundColor = Color.FromArgb("#FBE3E8");
        border.Stroke = Color.FromArgb("#E85B73");

        _selectedMoodBorder = border;
    }

    private void OnMood1Tapped(object sender, TappedEventArgs e) => SelectMood(Mood1Border);
    private void OnMood2Tapped(object sender, TappedEventArgs e) => SelectMood(Mood2Border);
    private void OnMood3Tapped(object sender, TappedEventArgs e) => SelectMood(Mood3Border);
    private void OnMood4Tapped(object sender, TappedEventArgs e) => SelectMood(Mood4Border);

    private void OnSymptomClicked(object sender, EventArgs e)
    {
        if (sender is not Button button)
            return;

        if (_selectedSymptoms.Contains(button))
        {
            _selectedSymptoms.Remove(button);

            button.BackgroundColor = Color.FromArgb("#F8F8F8");
            button.TextColor = Color.FromArgb("#333333");
            button.BorderColor = Color.FromArgb("#222222");
        }
        else
        {
            _selectedSymptoms.Add(button);

            button.BackgroundColor = Color.FromArgb("#FBE3E8");
            button.TextColor = Color.FromArgb("#E85B73");
            button.BorderColor = Color.FromArgb("#E85B73");
        }
    }

    private async void ShowCustomAlert(string title, string message)
    {
        CustomAlertTitle.Text = title;
        CustomAlertMessage.Text = message;

        CustomAlertOverlay.Opacity = 0;
        CustomAlertOverlay.IsVisible = true;

        await CustomAlertOverlay.FadeTo(1, 150);
    }

    private async void OnCustomAlertOkClicked(object sender, EventArgs e)
    {
        await CustomAlertOverlay.FadeTo(0, 150);
        CustomAlertOverlay.IsVisible = false;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        int userId = Preferences.Get("UserId", 0);
        string username = Preferences.Get("UserName", "");

        if (userId == 0 || string.IsNullOrWhiteSpace(username))
        {
            ShowCustomAlert("Error", "User not found.");
            return;
        }

        string flow = _selectedFlowButton?.Text ?? "";
        string mood = _selectedMoodBorder?.Content is Label label ? label.Text : "";
        double energy = EnergySlider.Value;
        string notes = NotesEditor.Text?.Trim() ?? "";

        List<string> symptomsList = new();

        foreach (var btn in _selectedSymptoms)
        {
            symptomsList.Add(btn.Text);
        }

        string symptoms = string.Join(",", symptomsList);

        if (string.IsNullOrWhiteSpace(flow) ||
            string.IsNullOrWhiteSpace(mood) ||
            string.IsNullOrWhiteSpace(symptoms) ||
            string.IsNullOrWhiteSpace(notes))
        {
            ShowCustomAlert("Required", "Please input all fields.");
            return;
        }

        var log = new DailyLog
        {
            UserId = userId,
            Username = username,
            Date = DateTime.Now.ToString("yyyy-MM-dd"),
            Flow = flow,
            Mood = mood,
            EnergyLevel = energy,
            Symptoms = symptoms,
            Notes = notes
        };

        await _databaseService.SaveDailyLogAsync(log);

        ClearForm();

        ShowCustomAlert("Success", "Daily pulse entry saved successfully.");
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new HomePage());
    }

    private async void OnHomeTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new HomePage());
    }

    private void OnDailyTapped(object sender, TappedEventArgs e)
    {
        // Already on Daily page
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
        await Navigation.PushAsync(new SettingsPage());
    }

    private void ClearForm()
    {
        ResetFlowButtons();
        ResetMoodBorders();

        _selectedFlowButton = null;
        _selectedMoodBorder = null;
        _selectedSymptoms.Clear();

        Button[] symptomButtons =
        {
        CrampsButton, WeakButton, DullButton, AcneButton, MoodyButton, BlueButton,
        BloatingButton, CravingButton, HeatButton, NauseaButton, RestlessButton,
        OilyButton, FatigueButton, SorenessButton, RushButton, AnxietyButton,
        IrritateButton, SickButton
    };

        foreach (var button in symptomButtons)
        {
            button.BackgroundColor = Color.FromArgb("#F8F8F8");
            button.TextColor = Color.FromArgb("#333333");
            button.BorderColor = Color.FromArgb("#222222");
        }

        EnergySlider.Value = 5;
        NotesEditor.Text = string.Empty;
    }
}