using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace Team_Aura_Period_Tracker_;

public partial class Up : ContentPage
{
    private readonly DatabaseService _databaseService;

    public Up()
    {
        InitializeComponent();
        _databaseService = new DatabaseService();
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
            ShowCustomAlert("Error", "Please fill in all fields.");
            return;
        }

        if (password != confirmPassword)
        {
            ShowCustomAlert("Error", "Passwords do not match.");
            return;
        }

        var existingUser = await _databaseService.GetUserByEmailAsync(email);

        if (existingUser != null)
        {
            ShowCustomAlert("Error", "This email is already registered.");
            return;
        }

        var user = new User
        {
            Name = name,
            Email = email,
            Password = password
        };

        await _databaseService.AddUserAsync(user);

        Preferences.Set("UserId", user.Id);
        Preferences.Set("UserName", user.Name);
        Preferences.Set("UserEmail", user.Email);
        Preferences.Set("UserPassword", user.Password);

        ShowCustomAlert("Success", "Account created successfully.");

        await Task.Delay(1000);

        await Shell.Current.GoToAsync(nameof(Step1Page));
    }

    private async void OnSignInClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(SignInPage));
    }
}