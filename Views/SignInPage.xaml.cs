using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace Team_Aura_Period_Tracker_;

public partial class SignInPage : ContentPage
{
    private bool isPasswordVisible = false;

    public SignInPage()
    {
        InitializeComponent();
    }

    // ?? Toggle Password Visibility
    private void OnTogglePasswordClicked(object sender, EventArgs e)
    {
        isPasswordVisible = !isPasswordVisible;

        PasswordEntry.IsPassword = !isPasswordVisible;

        TogglePasswordBtn.Source = isPasswordVisible
            ? "eye_open.png"
            : "eye_closed.png";
    }

    // ?? SIGN IN
    private async void OnSignInClicked(object sender, EventArgs e)
    {
        string email = EmailEntry.Text?.Trim() ?? "";
        string password = PasswordEntry.Text ?? "";

        string savedEmail = Preferences.Get("UserEmail", "");
        string savedPassword = Preferences.Get("UserPassword", "");

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Error", "Please enter your email and password.", "OK");
            return;
        }

        if (email == savedEmail && password == savedPassword)
        {
            await DisplayAlert("Success", "Sign in successful.", "OK");
            await Shell.Current.GoToAsync(nameof(HomePage));
        }
        else
        {
            await DisplayAlert("Error", "Invalid email or password.", "OK");
        }
    }

    // ?? SIGN UP
    private async void OnSignUpClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(Up));
    }

    // ?? FORGOT PASSWORD
    private async void OnForgotPasswordTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ForgotPasswordPage));
    }
}