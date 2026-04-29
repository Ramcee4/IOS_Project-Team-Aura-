namespace Team_Aura_Period_Tracker_;

public partial class FivePageStep : ContentPage
{
    public FivePageStep()
    {
        InitializeComponent();
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
        if (!PrivacyCheckBox.IsChecked)
        {
            ShowCustomAlert("Required", "Please agree to Aura's privacy practices first.");
            return;
        }

        await Shell.Current.GoToAsync(nameof(SixPageStep));
    }
}