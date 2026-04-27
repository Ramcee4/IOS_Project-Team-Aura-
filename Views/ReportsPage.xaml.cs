using System;
using Microsoft.Maui.Controls;

namespace Team_Aura_Period_Tracker_;

public partial class ReportsPage : ContentPage
{
    public ReportsPage()
    {
        InitializeComponent();
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}