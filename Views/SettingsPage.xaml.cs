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
        // Mahimo nimong e-clear ang Preferences diri kon gusto nimo
        // Preferences.Clear(); 

        // Reset the App to the SignInPage
        Application.Current.MainPage = new NavigationPage(new SignInPage());
    }

    private void OnCancelAlertClicked(object sender, EventArgs e)
    {
        HideOverlay();
    }

    // Methods for Overlay Animation
    private async void ShowOverlay()
    {
        AlertOverlay.IsVisible = true;
        AlertOverlay.Opacity = 0;
        await AlertOverlay.FadeTo(1, 150);
    }

    private async void HideOverlay()
    {
        await AlertOverlay.FadeTo(0, 150);
        AlertOverlay.IsVisible = false;
    }
}