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

    private async void OnPrivacyTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new PrivacyPage());
    }

    private async void OnReportsTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new ReportsPage());
    }

    private async void OnNotificationsTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new SettingsNotificationPage());
    }

    private async void OnLogoutTapped(object sender, TappedEventArgs e)
    {
        bool confirm = await DisplayAlert("Logout", "Are you sure?", "Yes", "No");

        if (confirm)
        {
            Application.Current.MainPage = new NavigationPage(new SignInPage());
        }
    }
}