using System;
using Microsoft.Maui.Controls;

namespace Team_Aura_Period_Tracker_;

public partial class NewPasswordPage : ContentPage
{
    private readonly string email;
    private readonly DatabaseService _databaseService;
    private bool goBackToLogin = false;

    public NewPasswordPage(string userEmail)
    {
        InitializeComponent();
        email = userEmail;
        _databaseService = new DatabaseService();
    }

    private void ShowCustomAlert(string title, string message, bool navigateBack = false)
    {
        CustomAlertTitle.Text = title;
        CustomAlertMessage.Text = message;
        goBackToLogin = navigateBack;
        CustomAlertOverlay.IsVisible = true;
    }

    private async void OnCustomAlertOkClicked(object sender, EventArgs e)
    {
        CustomAlertOverlay.IsVisible = false;

        if (goBackToLogin)
        {
            await Navigation.PopToRootAsync();
        }
    }

    private async void OnUpdatePasswordClicked(object sender, EventArgs e)
    {
        string newPassword = NewPasswordEntry.Text?.Trim() ?? "";
        string confirmPassword = ConfirmPasswordEntry.Text?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(newPassword) ||
            string.IsNullOrWhiteSpace(confirmPassword))
        {
            ShowCustomAlert("Error", "Please fill in both password fields.");
            return;
        }

        if (newPassword.Length < 6)
        {
            ShowCustomAlert("Error", "Password must be at least 6 characters.");
            return;
        }

        if (newPassword != confirmPassword)
        {
            ShowCustomAlert("Error", "New password and confirm password do not match.");
            return;
        }

        bool passwordChanged = await _databaseService.ChangePasswordByEmailAsync(
            email,
            newPassword,
            confirmPassword);

        if (!passwordChanged)
        {
            ShowCustomAlert("Error", "Email not found. Please try again.");
            return;
        }

        ShowCustomAlert("Success", "Password updated successfully.", true);
    }
}