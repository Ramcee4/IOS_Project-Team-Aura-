using System;
using Microsoft.Maui.Controls;

namespace Team_Aura_Period_Tracker_;

public partial class ForgotPasswordPage : ContentPage
{
    public ForgotPasswordPage()
    {
        InitializeComponent();
    }

    private async void OnSendResetLinkClicked(object sender, EventArgs e)
    {
        string email = ResetEmailEntry.Text?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(email))
        {
            await DisplayAlert("Error", "Please enter your email.", "OK");
            return;
        }

        await DisplayAlert("Reset Password", "A reset link has been sent to your email.", "OK");
    }

    private async void OnBackToSignInClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}