namespace Team_Aura_Period_Tracker_;

public partial class FivePageStep : ContentPage
{
    public FivePageStep()
    {
        InitializeComponent();
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnNextClicked(object sender, EventArgs e)
    {
        if (!PrivacyCheckBox.IsChecked)
        {
            await DisplayAlert("Required", "Please agree to Aura's privacy practices first.", "OK");
            return;
        }

        await Shell.Current.GoToAsync(nameof(SixPageStep));
    }
}