using System;
using Microsoft.Maui.Controls;

namespace Team_Aura_Period_Tracker_;

public partial class ProfilePage : ContentPage
{
    public ProfilePage()
    {
        InitializeComponent();
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
        await DisplayAlert("Saved", "Profile updated successfully.", "OK");
        await Navigation.PopAsync();
    }
}