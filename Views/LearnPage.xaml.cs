using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Team_Aura_Period_Tracker_;

public partial class LearnPage : ContentPage
{
    public LearnPage()
    {
        InitializeComponent();
    }

    private void ResetFilters()
    {
        AllButton.BackgroundColor = Color.FromArgb("#F8F8F8");
        CycleHealthButton.BackgroundColor = Color.FromArgb("#F8F8F8");
        ConditionsButton.BackgroundColor = Color.FromArgb("#F8F8F8");

        AllButton.TextColor = Color.FromArgb("#666666");
        CycleHealthButton.TextColor = Color.FromArgb("#666666");
        ConditionsButton.TextColor = Color.FromArgb("#666666");
    }

    private void OnFilterClicked(object sender, EventArgs e)
    {
        if (sender is not Button button)
            return;

        ResetFilters();

        button.BackgroundColor = Color.FromArgb("#F4CDD5");
        button.TextColor = Color.FromArgb("#333333");
    }

    private async void OnArticleOneTapped(object sender, TappedEventArgs e)
    {
        await DisplayAlert(
            "Understanding Your Menstrual Cycle",
            "This article explains the four phases of the menstrual cycle and how hormones affect mood, energy, and body changes.",
            "OK");
    }

    private async void OnArticleTwoTapped(object sender, TappedEventArgs e)
    {
        await DisplayAlert(
            "Managing PCOS Naturally",
            "This article covers lifestyle, nutrition, and wellness habits that may support people managing PCOS.",
            "OK");
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

    private async void OnInsightTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new InsightPage());
    }

    private async void OnLearnTapped(object sender, TappedEventArgs e)
    {
        await DisplayAlert("Learn", "You are already here.", "OK");
    }

    private async void OnSettingsTapped(object sender, TappedEventArgs e)
    {
        await DisplayAlert("Settings", "Open settings page.", "OK");
    }
}