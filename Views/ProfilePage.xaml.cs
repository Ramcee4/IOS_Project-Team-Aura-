using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace Team_Aura_Period_Tracker_;

public partial class ProfilePage : ContentPage
{
    private readonly DatabaseService _databaseService;

    public ProfilePage()
    {
        InitializeComponent();
        _databaseService = new DatabaseService();
        LoadProfile();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadProfile();
    }

    private void LoadProfile()
    {
        NameEntry.Text = Preferences.Get("UserName", "");
        EmailEntry.Text = Preferences.Get("UserEmail", "");
        PasswordEntry.Text = Preferences.Get("UserPassword", "");
        DisableEditing();
    }

    private void DisableEditing()
    {
        NameEntry.IsEnabled = false;
        EmailEntry.IsEnabled = false;
        PasswordEntry.IsEnabled = false;
    }

    private void OnEditNameClicked(object sender, EventArgs e)
    {
        NameEntry.IsEnabled = true;
        NameEntry.Focus();
    }

    private void OnEditEmailClicked(object sender, EventArgs e)
    {
        EmailEntry.IsEnabled = true;
        EmailEntry.Focus();
    }

    private void OnEditPasswordClicked(object sender, EventArgs e)
    {
        PasswordEntry.IsEnabled = true;
        PasswordEntry.Focus();
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        try
        {
            int userId = Preferences.Get("UserId", 0);

            string name = NameEntry.Text?.Trim() ?? "";
            string email = EmailEntry.Text?.Trim() ?? "";
            string password = PasswordEntry.Text ?? "";

            if (userId == 0)
            {
                ShowCustomAlert("Error", "User not found. Please sign in again.");
                return;
            }

            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                ShowCustomAlert("Error", "Please fill in all fields.");
                return;
            }

            var currentUser = await _databaseService.GetUserByIdAsync(userId);

            if (currentUser == null)
            {
                ShowCustomAlert("Error", "User not found in database.");
                return;
            }

            var emailOwner = await _databaseService.GetUserByEmailAsync(email);

            if (emailOwner != null && emailOwner.Id != userId)
            {
                ShowCustomAlert("Error", "This email is already used by another account.");
                return;
            }

            currentUser.Name = name;
            currentUser.Email = email;
            currentUser.Password = password;

            await _databaseService.UpdateUserAsync(currentUser);

            Preferences.Set("UserId", currentUser.Id);
            Preferences.Set("UserName", currentUser.Name);
            Preferences.Set("UserEmail", currentUser.Email);
            Preferences.Set("UserPassword", currentUser.Password);

            LoadProfile();

            ShowCustomAlert("Success", "Profile updated successfully.");
        }
        catch (Exception ex)
        {
            ShowCustomAlert("Error", $"Something went wrong:\n{ex.Message}");
        }
    }

    private void OnCancelClicked(object sender, EventArgs e)
    {
        LoadProfile();
    }

    private async void OnBackClicked(object sender, EventArgs e)
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