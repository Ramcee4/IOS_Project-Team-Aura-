using System;
using Microsoft.Maui.Controls;

namespace Team_Aura_Period_Tracker_;

public partial class ForgotPasswordPage : ContentPage
{
    private readonly DatabaseService _databaseService;
    private readonly EmailService _emailService;

    public ForgotPasswordPage()
    {
        InitializeComponent();
        _databaseService = new DatabaseService();
        _emailService = new EmailService();
    }

    private async void OnSendResetLinkClicked(object sender, EventArgs e)
    {
        string email = ResetEmailEntry.Text?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(email))
        {
            ShowCustomAlert("Error", "Please enter your email.");
            return;
        }

        var user = await _databaseService.GetUserByEmailAsync(email);

        if (user == null)
        {
            ShowCustomAlert("Error", "Email not found in database.");
            return;
        }

        try
        {
            Random random = new Random();
            string generatedOtp = random.Next(100000, 999999).ToString();

            await _emailService.SendOtpEmailAsync(email, generatedOtp);

            await Navigation.PushAsync(new OtpVerificationPage(email, generatedOtp));
        }
        catch (Exception ex)
        {
            ShowCustomAlert("Error", $"Failed to send OTP email.\n{ex.Message}");
        }
    }

    private async void OnBackToSignInClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
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
}