using Microsoft.Maui.Storage;

namespace Team_Aura_Period_Tracker_;

public partial class Step3Page : ContentPage
{
    public Step3Page()
    {
        InitializeComponent();
        CycleTypePicker.SelectedIndex = 0;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        string savedDate = Preferences.Get("lastPeriodDate", "");
        LastPeriodDateButton.Text = string.IsNullOrWhiteSpace(savedDate) ? "Select date" : savedDate;
    }

    private async void OnOpenDatePickerPageClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Step3_1Page());
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnNextClicked(object sender, EventArgs e)
    {
        string savedDate = Preferences.Get("lastPeriodDate", "");

        if (string.IsNullOrWhiteSpace(savedDate))
        {
            await DisplayAlert("Required", "Please select your last period date.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(CycleLengthEntry.Text))
        {
            await DisplayAlert("Required", "Please enter your cycle length.", "OK");
            return;
        }

        await Shell.Current.GoToAsync(nameof(Step4Page));
    }
}