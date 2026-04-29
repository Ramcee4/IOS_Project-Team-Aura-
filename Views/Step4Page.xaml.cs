using Microsoft.Maui.Graphics;
using Microsoft.Maui.Storage;

namespace Team_Aura_Period_Tracker_;

public partial class Step4Page : ContentPage
{
    private readonly List<Button> _buttons;
    private readonly DatabaseService _databaseService;
    private Button? _selected;

    public Step4Page()
    {
        InitializeComponent();

        _databaseService = new DatabaseService();

        _buttons = new List<Button>
        {
            Age1,
            Age2,
            Age3,
            Age4
        };
    }

    private void OnAgeClicked(object sender, EventArgs e)
    {
        if (sender is not Button btn)
            return;

        foreach (var b in _buttons)
        {
            SetUnselectedStyle(b);
        }

        _selected = btn;
        SetSelectedStyle(btn);
    }

    private void SetSelectedStyle(Button button)
    {
        button.BackgroundColor = Color.FromArgb("#FEE2E2");
        button.BorderColor = Color.FromArgb("#E56B7D");
        button.TextColor = Color.FromArgb("#E56B7D");
    }

    private void SetUnselectedStyle(Button button)
    {
        button.BackgroundColor = Color.FromArgb("#F8F8F8");
        button.BorderColor = Color.FromArgb("#111111");
        button.TextColor = Color.FromArgb("#222222");
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

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void OnNextClicked(object sender, EventArgs e)
    {
        if (_selected == null)
        {
            ShowCustomAlert("Required", "Please select an age range.");
            return;
        }

        int userId = Preferences.Get("UserId", 0);
        string username = Preferences.Get("UserName", "");

        if (userId == 0 || string.IsNullOrWhiteSpace(username))
        {
            ShowCustomAlert("Error", "No signed up user found.");
            return;
        }

        string ageRange = _selected.Text;

        await _databaseService.SaveUserAgeRangeAsync(userId, username, ageRange);

        await Shell.Current.GoToAsync(nameof(FivePageStep));
    }
}