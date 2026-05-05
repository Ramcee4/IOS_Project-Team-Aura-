using System;
using Microsoft.Maui.Controls;

namespace Team_Aura_Period_Tracker_;

public partial class OtpVerificationPage : ContentPage
{
    private readonly string email;
    private string correctOtp;
    private bool goToNewPasswordPage = false;

    private readonly EmailService _emailService;

    public OtpVerificationPage(string userEmail, string otp)
    {
        InitializeComponent();

        email = userEmail;
        correctOtp = otp;
        _emailService = new EmailService();
    }

    private void ShowCustomAlert(string title, string message, bool navigateNext = false)
    {
        CustomAlertTitle.Text = title;
        CustomAlertMessage.Text = message;
        goToNewPasswordPage = navigateNext;
        CustomAlertOverlay.IsVisible = true;
    }

    private async void OnCustomAlertOkClicked(object sender, EventArgs e)
    {
        CustomAlertOverlay.IsVisible = false;

        if (goToNewPasswordPage)
        {
            await Navigation.PushAsync(new NewPasswordPage(email));
        }
    }

    private void OnVerifyOtpClicked(object sender, EventArgs e)
    {
        string enteredOtp = OtpEntry.Text?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(enteredOtp))
        {
            ShowCustomAlert("Error", "Please enter the OTP.");
            return;
        }

        if (enteredOtp != correctOtp)
        {
            ShowCustomAlert("Invalid OTP", "The OTP you entered is incorrect.");
            return;
        }

        ShowCustomAlert("Success", "OTP verified successfully.", true);
    }

    private async void OnResendOtpClicked(object sender, EventArgs e)
    {
        try
        {
            Random random = new Random();
            correctOtp = random.Next(100000, 999999).ToString();

            await _emailService.SendOtpEmailAsync(email, correctOtp);

            ShowCustomAlert("Success", "A new OTP has been sent to your Gmail.");
        }
        catch (Exception ex)
        {
            ShowCustomAlert("Error", $"Failed to resend OTP.\n{ex.Message}");
        }
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}