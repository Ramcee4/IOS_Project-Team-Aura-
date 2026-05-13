using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace Team_Aura_Period_Tracker_;

public partial class SignInPage : ContentPage
{
    private readonly DatabaseService _databaseService;

    public SignInPage()
    {
        InitializeComponent();
        _databaseService = new DatabaseService();
    }

    // 1. PASSWORD VISIBILITY TOGGLE
    private void OnTogglePasswordClicked(object sender, EventArgs e)
    {
        PasswordEntry.IsPassword = !PasswordEntry.IsPassword;

        // Siguroha nga 'TogglePasswordBtn' ang x:Name sa imong ImageButton sa XAML
        TogglePasswordBtn.Source = PasswordEntry.IsPassword ? "eye_closed.png" : "eye_open.png";
    }

    // 2. SIGN IN WITH USER INPUT VALIDATION
    private async void OnSignInClicked(object sender, EventArgs e)
    {
        string email = EmailEntry.Text?.Trim() ?? "";
        string password = PasswordEntry.Text ?? "";

        // --- START VALIDATION ---

        // A. Check if empty fields
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            ShowCustomAlert("Error", "Please fill in all fields.");
            return;
        }

        // B. Email Format Validation (Dapat naay @ ug domain)
        var emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        if (!Regex.IsMatch(email, emailRegex))
        {
            ShowCustomAlert("Invalid Email", "Please enter a valid email address.");
            return;
        }

        // C. Password Length Validation (Minimum 6 characters)
        if (password.Length < 6)
        {
            ShowCustomAlert("Invalid Password", "Password must be at least 6 characters.");
            return;
        }

        // --- END VALIDATION ---

        // 3. DATABASE VERIFICATION
        var user = await _databaseService.GetUserByEmailAsync(email);

        if (user != null && user.Password == password)
        {
            // Save Session
            Preferences.Set("UserId", user.Id);
            Preferences.Set("UserName", user.Name);
            Preferences.Set("UserEmail", user.Email);
            Preferences.Set("UserPassword", user.Password);

            ShowCustomAlert("Success", "Sign in successful.");

            await Task.Delay(1000);

            // Reset navigation stack to Home
            await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
        }
        else
        {
            ShowCustomAlert("Login Failed", "Invalid email or password.");
        }
    }

    // 4. NAVIGATION
    private async void OnSignUpClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(Up));
    }

    private async void OnForgotPasswordTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ForgotPasswordPage));
    }

    // 5. CUSTOM ALERT LOGIC (Stylish Popup)
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
}