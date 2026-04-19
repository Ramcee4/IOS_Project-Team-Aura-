using Microsoft.Maui.Storage;

namespace Team_Aura_Period_Tracker_;

public partial class Up : ContentPage
{
    private bool _isPasswordVisible = false;
    private bool _isConfirmPasswordVisible = false;

    public Up()
    {
        InitializeComponent();
    }

    private void OnPasswordEyeClicked(object sender, EventArgs e)
    {
        _isPasswordVisible = !_isPasswordVisible;

        PasswordEntry.IsPassword = !_isPasswordVisible;
        PasswordEye.Source = _isPasswordVisible ? "eye_open.png" : "eye_closed.png";
    }

    private void OnConfirmPasswordEyeClicked(object sender, EventArgs e)
    {
        _isConfirmPasswordVisible = !_isConfirmPasswordVisible;

        ConfirmPasswordEntry.IsPassword = !_isConfirmPasswordVisible;
        ConfirmPasswordEye.Source = _isConfirmPasswordVisible ? "eye_open.png" : "eye_closed.png";
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
            await DisplayAlert("Missing Information", "Please fill in all fields.", "OK");
            return;
        }

        if (password != confirmPassword)
        {
            await DisplayAlert("Password Error", "Passwords do not match.", "OK");
            return;
        }

        Preferences.Set("userName", name);

        await Shell.Current.GoToAsync(nameof(Step1Page));
    }

    private async void OnSignInClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}