using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace Team_Aura_Period_Tracker_;

public partial class SignInPage : ContentPage
{
    private readonly DatabaseService _databaseService;
    private bool isPasswordVisible = false;

    public SignInPage()
    {
        InitializeComponent();
        _databaseService = new DatabaseService();
    }

    private void OnTogglePasswordClicked(object sender, EventArgs e)
    {
        isPasswordVisible = !isPasswordVisible;

        PasswordEntry.IsPassword = !isPasswordVisible;

        TogglePasswordBtn.Source = isPasswordVisible
            ? "eye_open.png"
            : "eye_closed.png";
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

    private async void OnSignInClicked(object sender, EventArgs e)
    {
        string email = EmailEntry.Text?.Trim() ?? "";
        string password = PasswordEntry.Text ?? "";

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            ShowCustomAlert("Error", "Please enter your email and password.");
            return;
        }

        var user = await _databaseService.GetUserByEmailAsync(email);

        if (user == null || user.Password != password)
        {
            ShowCustomAlert("Error", "Invalid email or password.");
            return;
        }

        Preferences.Set("UserName", user.Name);
        Preferences.Set("UserEmail", user.Email);
        Preferences.Set("UserPassword", user.Password);

        ShowCustomAlert("Success", "Sign in successful.");

        await Task.Delay(1000);

        await Shell.Current.GoToAsync(nameof(HomePage));
    }

    private async void OnSignUpClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(Up));
    }

    private async void OnForgotPasswordTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ForgotPasswordPage));
    }
}