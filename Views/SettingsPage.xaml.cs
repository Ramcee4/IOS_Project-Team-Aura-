using System;
using Microsoft.Maui.Controls;

namespace Team_Aura_Period_Tracker_;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnProfileTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new ProfilePage());
    }

    private async void OnReportsTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new ReportsPage());
    }

    private async void OnNotificationsTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new SettingsNotificationPage());
    }

    // --- CUSTOM LOGOUT ALERT LOGIC ---

    private void OnLogoutTapped(object sender, TappedEventArgs e)
    {
        ShowOverlay();
    }

    private void OnConfirmLogoutClicked(object sender, EventArgs e)
    {
        HideOverlay();
        // Clear session if needed (optional)
        // Preferences.Clear(); 

        Application.Current.MainPage = new NavigationPage(new SignInPage());
    }

    private void OnCancelAlertClicked(object sender, EventArgs e)
    {
        HideOverlay();
    }

    // Helper Methods para sa Animation sa Overlay
    private async void ShowOverlay()
    {
        AlertOverlay.Opacity = 0;
        AlertOverlay.IsVisible = true;
        await AlertOverlay.FadeTo(1, 150);
    }

    private async void HideOverlay()
    {
        await AlertOverlay.FadeTo(0, 150);
        AlertOverlay.IsVisible = false;
    }
}