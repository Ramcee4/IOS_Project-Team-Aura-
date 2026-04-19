using Microsoft.Maui.Graphics;

namespace Team_Aura_Period_Tracker_;

public partial class Step2Page : ContentPage
{
    private readonly HashSet<Button> _selectedButtons = new();

    public Step2Page()
    {
        InitializeComponent();
    }

    private void OnOptionClicked(object sender, EventArgs e)
    {
        if (sender is not Button button)
            return;

        if (_selectedButtons.Contains(button))
        {
            _selectedButtons.Remove(button);
            SetUnselectedStyle(button);
        }
        else
        {
            _selectedButtons.Add(button);
            SetSelectedStyle(button);
        }
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

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnNextClicked(object sender, EventArgs e)
    {
        if (_selectedButtons.Count == 0)
        {
            await DisplayAlert("Required", "Please select at least one option.", "OK");
            return;
        }

        await Shell.Current.GoToAsync(nameof(Step3Page));
    }
}