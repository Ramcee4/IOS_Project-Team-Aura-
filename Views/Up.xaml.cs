using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace Team_Aura_Period_Tracker_;

public partial class Up : ContentPage
{
    public Up()
    {
        InitializeComponent();
    }

    private void OnPasswordEyeClicked(object sender, EventArgs e)
    {
        PasswordEntry.IsPassword = !PasswordEntry.IsPassword;
        PasswordEye.Source = PasswordEntry.IsPassword ? "eye_closed.png" : "eye_open.png";
    }

    private void OnConfirmPasswordEyeClicked(object sender, EventArgs e)
    {
        ConfirmPasswordEntry.IsPassword = !ConfirmPasswordEntry.IsPassword;
        ConfirmPasswordEye.Source = ConfirmPasswordEntry.IsPassword ? "eye_closed.png" : "eye_open.png";
    }

    private async void OnSignUpClicked(object sender, EventArgs e)
    {
        string name = NameEntry.Text?.Trim() ?? "";
        string email = EmailEntry.Text?.Trim() ?? "";
        string password = PasswordEntry.Text ?? "";
        string confirmPassword = ConfirmPasswordEntry.Text ?? "";

        if (string.IsNullOrWhiteSpace(name) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(confirmPassword))
        {
            await DisplayAlert("Error", "Please fill in all fields.", "OK");
            return;
        }

        if (password != confirmPassword)
        {
            await DisplayAlert("Error", "Passwords do not match.", "OK");
            return;
        }

        Preferences.Set("UserName", name);
        Preferences.Set("UserEmail", email);
        Preferences.Set("UserPassword", password);

        await DisplayAlert("Success", "Account created successfully.", "OK");

        await Shell.Current.GoToAsync(nameof(Step1Page));
    }

    private async void OnSignInClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(SignInPage));
    }
}