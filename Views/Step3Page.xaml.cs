using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Storage;

namespace Team_Aura_Period_Tracker_;

public partial class Step3Page : ContentPage
{
    private readonly DatabaseService _databaseService;
    private string selectedCycleType = "";

    public Step3Page()
    {
        InitializeComponent();
        _databaseService = new DatabaseService();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        string lastDateOfPeriod = Preferences.Get("LastDateOfPeriod", "");

        LastPeriodDateButton.Text = !string.IsNullOrWhiteSpace(lastDateOfPeriod)
            ? lastDateOfPeriod
            : "Select date";

        int savedCycleLength = Preferences.Get("CycleLengthDays", 0);
        int savedPeriodDays = Preferences.Get("PeriodDays", 0);
        string savedCycleType = Preferences.Get("CycleType", "");

        if (savedCycleLength > 0)
            CycleLengthEntry.Text = savedCycleLength.ToString();

        if (savedPeriodDays > 0)
            PeriodDaysEntry.Text = savedPeriodDays.ToString();

        if (!string.IsNullOrWhiteSpace(savedCycleType))
        {
            selectedCycleType = savedCycleType;

            SetCycleButtonUnselected(RegularButton);
            SetCycleButtonUnselected(IrregularButton);

            if (savedCycleType == RegularButton.Text)
                SetCycleButtonSelected(RegularButton);
            else if (savedCycleType == IrregularButton.Text)
                SetCycleButtonSelected(IrregularButton);
        }
    }

    private async void OnOpenDatePickerPageClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(Step3_1Page));
    }

    private void OnCycleTypeClicked(object sender, EventArgs e)
    {
        if (sender is not Button button)
            return;

        selectedCycleType = button.Text;

        SetCycleButtonUnselected(RegularButton);
        SetCycleButtonUnselected(IrregularButton);
        SetCycleButtonSelected(button);
    }

    private void SetCycleButtonSelected(Button button)
    {
        button.BackgroundColor = Color.FromArgb("#FEE2E2");
        button.BorderColor = Color.FromArgb("#E56B7D");
        button.TextColor = Color.FromArgb("#E56B7D");
    }

    private void SetCycleButtonUnselected(Button button)
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
        int userId = Preferences.Get("UserId", 0);
        string username = Preferences.Get("UserName", "");
        string lastDateOfPeriod = Preferences.Get("LastDateOfPeriod", "");

        if (userId == 0 || string.IsNullOrWhiteSpace(username))
        {
            ShowCustomAlert("Error", "No signed up user found.");
            return;
        }

        if (string.IsNullOrWhiteSpace(lastDateOfPeriod))
        {
            ShowCustomAlert("Required", "Please select your last period date.");
            return;
        }

        if (string.IsNullOrWhiteSpace(CycleLengthEntry.Text) ||
            !int.TryParse(CycleLengthEntry.Text.Trim(), out int cycleLengthDays))
        {
            ShowCustomAlert("Required", "Please enter a valid cycle length.");
            return;
        }

        if (string.IsNullOrWhiteSpace(PeriodDaysEntry.Text) ||
            !int.TryParse(PeriodDaysEntry.Text.Trim(), out int periodDays))
        {
            ShowCustomAlert("Required", "Please enter valid period days.");
            return;
        }

        if (cycleLengthDays <= 0 || periodDays <= 0)
        {
            ShowCustomAlert("Invalid", "Cycle length and period days must be greater than 0.");
            return;
        }

        if (cycleLengthDays < 21 || cycleLengthDays > 45)
        {
            ShowCustomAlert("Invalid", "Cycle length should usually be between 21 and 45 days.");
            return;
        }

        if (periodDays < 1 || periodDays > 10)
        {
            ShowCustomAlert("Invalid", "Period days should usually be between 1 and 10 days.");
            return;
        }

        if (periodDays > cycleLengthDays)
        {
            ShowCustomAlert("Invalid", "Period days cannot be greater than cycle length.");
            return;
        }

        if (string.IsNullOrWhiteSpace(selectedCycleType))
        {
            ShowCustomAlert("Required", "Please select your cycle type.");
            return;
        }

        await _databaseService.SaveUserCycleInfoAsync(
            userId,
            username,
            lastDateOfPeriod,
            cycleLengthDays,
            periodDays,
            selectedCycleType
        );

        Preferences.Set("CycleLengthDays", cycleLengthDays);
        Preferences.Set("PeriodDays", periodDays);
        Preferences.Set("CycleType", selectedCycleType);

        await Shell.Current.GoToAsync(nameof(Step4Page));
    }
}