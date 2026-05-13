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

    private void OnTogglePasswordClicked(object sender, EventArgs e)
    {
        PasswordEntry.IsPassword = !PasswordEntry.IsPassword;
        TogglePasswordBtn.Source = PasswordEntry.IsPassword ? "eye_closed.png" : "eye_open.png";
    }

    private async void OnSignInClicked(object sender, EventArgs e)
    {
        string email = EmailEntry.Text?.Trim() ?? "";
        string password = PasswordEntry.Text ?? "";

        // VALIDATION
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            ShowCustomAlert("Error", "Please fill in all fields.");
            return;
        }

        var emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        if (!Regex.IsMatch(email, emailRegex))
        {
            ShowCustomAlert("Invalid Email", "Please enter a valid email address.");
            return;
        }

        // DATABASE CHECK (Using your User model)
        var user = await _databaseService.GetUserByEmailAsync(email);

        if (user != null && user.Password == password)
        {
            // Save Session
            Preferences.Set("UserId", user.Id);
            Preferences.Set("UserName", user.Name);
            Preferences.Set("UserEmail", user.Email);

            ShowCustomAlert("Success", "Sign in successful.");

         

            // THE STABLE FIX:
            // I-reset ang MainPage ngadto sa bag-ong AppShell instance
            // Kini makatangtang sa Login page sa navigation stack para dili na kabalik ang user
            var shell = new AppShell();
            Application.Current.MainPage = shell;

            // Siguroha nga ang Route="HomePage" naa sa imong AppShell.xaml
            await shell.GoToAsync("//HomePage");
        }
        else
        {
            ShowCustomAlert("Login Failed", "Invalid email or password.");
        }
    }

    private async void OnSignUpClicked(object sender, EventArgs e)
    {
        // Siguroha nga SignUpPage ang ngalan sa imong XAML file
        await Navigation.PushAsync(new Up());
    }

    private async void OnForgotPasswordTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ForgotPasswordPage());
    }

    // CUSTOM ALERT LOGIC (Consistent Styling)
    private async void ShowCustomAlert(string title, string message)
    {
        CustomAlertTitle.Text = title;
        CustomAlertMessage.Text = message;
        CustomAlertOverlay.IsVisible = true;
        CustomAlertOverlay.Opacity = 0;
        await CustomAlertOverlay.FadeTo(1, 150);
    }

    private async void OnCustomAlertOkClicked(object sender, EventArgs e)
    {
        await CustomAlertOverlay.FadeTo(0, 150);
        CustomAlertOverlay.IsVisible = false;
    }
}