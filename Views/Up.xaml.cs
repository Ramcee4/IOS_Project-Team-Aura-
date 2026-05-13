using System;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
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

    // ✅ EMAIL SENDER METHOD
    private async Task SendWelcomeEmailAsync(string userEmail, string userName)
    {
        try
        {
            var fromEmail = "teamauraofficial94@gmail.com";
            var appPassword = "pbwzyzwygscxgtsa";

            var message = new MailMessage();
            message.From = new MailAddress(fromEmail, "Period Tracker");
            message.To.Add(userEmail);
            message.Subject = "Welcome to Period Tracker";
            message.Body = $"Hi {userName},\n\nYou have successfully signed up for Period Tracker.\n\nWe’re happy to have you!\n\n- Team Aura";

            using var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(fromEmail, appPassword),
                EnableSsl = true
            };

            await smtp.SendMailAsync(message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Email failed: {ex.Message}");
        }
    }

    // ✅ SIGN UP CLICKED WITH VALIDATION
    private async void OnSignUpClicked(object sender, EventArgs e)
    {
        string name = NameEntry.Text?.Trim() ?? "";
        string email = EmailEntry.Text?.Trim() ?? "";
        string password = PasswordEntry.Text ?? "";
        string confirmPassword = ConfirmPasswordEntry.Text ?? "";

        // 1. Check if empty
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
        {
            ShowCustomAlert("Error", "Please fill in all fields.");
            return;
        }

        // 2. Email Format Validation
        var emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        if (!Regex.IsMatch(email, emailRegex))
        {
            ShowCustomAlert("Invalid Email", "Please enter a valid email address (e.g., name@gmail.com).");
            return;
        }

        // 3. Password Length Validation
        if (password.Length < 6)
        {
            ShowCustomAlert("Weak Password", "Password must be at least 6 characters.");
            return;
        }

        // 4. Confirm Password Match
        if (password != confirmPassword)
        {
            ShowCustomAlert("Error", "Passwords do not match.");
            return;
        }

        // 5. Database Check for Existing Email
        var existingUser = await _databaseService.GetUserByEmailAsync(email);
        if (existingUser != null)
        {
            ShowCustomAlert("Error", "This email is already registered.");
            return;
        }

        // Save User
        var user = new User
        {
            Name = name,
            Email = email,
            Password = password
        };

        await _databaseService.AddUserAsync(user);
        await SendWelcomeEmailAsync(user.Email, user.Name);

        Preferences.Set("UserId", user.Id);
        Preferences.Set("UserName", user.Name);
        Preferences.Set("UserEmail", user.Email);

        ShowCustomAlert("Success", "Account created successfully.");

        await Task.Delay(1500);
        await Shell.Current.GoToAsync(nameof(Step1Page));
    }

    private async void OnSignInClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(SignInPage));
    }

    // ✅ CUSTOM ALERT LOGIC
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