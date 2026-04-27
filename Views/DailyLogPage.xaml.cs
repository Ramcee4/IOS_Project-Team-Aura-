using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Team_Aura_Period_Tracker_;

public partial class DailyLogPage : ContentPage
{
    private Button? _selectedFlowButton;
    private Border? _selectedMoodBorder;
    private readonly HashSet<Button> _selectedSymptoms = new();

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

    private void OnMood1Tapped(object sender, TappedEventArgs e)
    {
        SelectMood(Mood1Border);
    }

    private void OnMood2Tapped(object sender, TappedEventArgs e)
    {
        SelectMood(Mood2Border);
    }

    private void OnMood3Tapped(object sender, TappedEventArgs e)
    {
        SelectMood(Mood3Border);
    }

    private void OnMood4Tapped(object sender, TappedEventArgs e)
    {
        SelectMood(Mood4Border);
    }

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

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        string notes = NotesEditor.Text?.Trim() ?? "";

        await DisplayAlert("Saved", "Your daily log has been saved.", "OK");
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
        await DisplayAlert("Daily", "You are already on Daily.", "OK");
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
}