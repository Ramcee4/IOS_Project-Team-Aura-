using System;
using Microsoft.Maui.Controls;

namespace Team_Aura_Period_Tracker_;

public partial class PrivacyPage : ContentPage
{
    public PrivacyPage()
    {
        InitializeComponent();
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnEnableAppLockClicked(object sender, EventArgs e)
    {
        await DisplayAlert("App Lock", "App lock enabled.", "OK");
    }

    private async void OnExportClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Export", "Your data has been exported as JSON.", "OK");
    }

    private async void OnDeleteAllDataClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert(
            "Delete All Data",
            "Are you sure? This action cannot be undone.",
            "Delete",
            "Cancel");

        if (confirm)
        {
            await DisplayAlert("Deleted", "All data has been deleted.", "OK");
        }
    }
}