using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Team_Aura_Period_Tracker_;

public partial class DailyPulseHistoryPage : ContentPage
{
    private readonly DatabaseService _databaseService = new();
    private List<DailyLog> _entries = new();
    private DailyLog _logToDelete;

    public DailyPulseHistoryPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadEntriesAsync();
    }

    private async Task LoadEntriesAsync()
    {
        DailyPulseContainer.Children.Clear();
        int userId = Preferences.Get("UserId", 0);

        if (userId == 0)
        {
            DailyPulseContainer.Children.Add(CreateEmptyState());
            return;
        }

        _entries = await _databaseService.GetDailyLogsByUserAsync(userId);

        if (_entries == null || _entries.Count == 0)
        {
            DailyPulseContainer.Children.Add(CreateEmptyState());
            return;
        }

        foreach (var entry in _entries.OrderByDescending(e => e.Date))
        {
            DailyPulseContainer.Children.Add(CreateDailyPulseCard(entry));
        }
    }

    private View CreateEmptyState()
    {
        return new Border
        {
            Padding = new Thickness(22, 26),
            BackgroundColor = Color.FromArgb("#F8F8F8"),
            Stroke = Color.FromArgb("#DDD6D8"),
            StrokeThickness = 1,
            StrokeShape = new RoundRectangle { CornerRadius = 18 },
            Content = new VerticalStackLayout
            {
                Spacing = 8,
                Children = {
                    new Label { Text = "No daily pulse entries yet", FontSize = 18, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#333333"), HorizontalTextAlignment = TextAlignment.Center },
                    new Label { Text = "Tap + to add your first daily pulse entry.", FontSize = 14, TextColor = Color.FromArgb("#6F6A6C"), HorizontalTextAlignment = TextAlignment.Center }
                }
            }
        };
    }

    private Border CreateDailyPulseCard(DailyLog entry)
    {
        var header = new Grid { ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) }, ColumnSpacing = 12 };
        var titleStack = new VerticalStackLayout
        {
            Spacing = 2,
            Children = {
            new Label { Text = "Daily Pulse Entry", FontSize = 16, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#333333") },
            new Label { Text = entry.Date, FontSize = 13, TextColor = Color.FromArgb("#8A8A8A") }
        }
        };

        var deleteButton = new ImageButton { Source = "delete_icon.png", BackgroundColor = Colors.Transparent, WidthRequest = 26, HeightRequest = 26, Padding = 4 };
        deleteButton.Clicked += (s, e) => { _logToDelete = entry; ShowDeleteConfirmation(); };

        Grid.SetColumn(titleStack, 0); Grid.SetColumn(deleteButton, 1);
        header.Children.Add(titleStack); header.Children.Add(deleteButton);

        var infoGrid = new Grid { ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Star) }, ColumnSpacing = 28 };
        var leftColumn = new VerticalStackLayout { Spacing = 16, Children = { MakeField("Flow", entry.Flow), MakeField("Mood", entry.Mood) } };
        var rightColumn = new VerticalStackLayout { Spacing = 16, Children = { MakeField("Energy", $"{Math.Round(entry.EnergyLevel)}/10"), MakeField("Symptoms", entry.Symptoms) } };

        Grid.SetColumn(leftColumn, 0); Grid.SetColumn(rightColumn, 1);
        infoGrid.Children.Add(leftColumn); infoGrid.Children.Add(rightColumn);

        return new Border
        {
            Padding = new Thickness(20, 18),
            BackgroundColor = Color.FromArgb("#F8F8F8"),
            Stroke = Color.FromArgb("#DDD6D8"),
            StrokeThickness = 1,
            StrokeShape = new RoundRectangle { CornerRadius = 18 },
            Content = new VerticalStackLayout { Spacing = 14, Children = { header, infoGrid, new BoxView { HeightRequest = 1, Color = Color.FromArgb("#EBDCF5") }, MakeField("Notes", entry.Notes) } }
        };
    }

    private VerticalStackLayout MakeField(string label, string value)
    {
        return new VerticalStackLayout
        {
            Children = {
            new Label { Text = label, FontSize = 12, TextColor = Color.FromArgb("#8A8A8A") },
            new Label { Text = string.IsNullOrWhiteSpace(value) ? "—" : value, FontSize = 14, TextColor = Color.FromArgb("#555555"), LineBreakMode = LineBreakMode.WordWrap }
        }
        };
    }

    private void ShowDeleteConfirmation()
    {
        AlertTitle.Text = "Delete";
        AlertMessage.Text = "Delete this daily pulse entry?";
        OkButton.IsVisible = false;
        ConfirmButtons.IsVisible = true;
        ShowOverlay();
    }

    private async void OnConfirmDeleteClicked(object sender, EventArgs e)
    {
        if (_logToDelete != null)
        {
            await HideOverlay();
            await _databaseService.DeleteDailyLogAsync(_logToDelete);
            await LoadEntriesAsync();
            _logToDelete = null;
        }
    }

    private async Task HideOverlay()
    {
        await AlertOverlay.FadeTo(0, 150);
        AlertOverlay.IsVisible = false;
    }

    private async void OnCancelAlertClicked(object sender, EventArgs e) => await HideOverlay();
    private async void OnOkClicked(object sender, EventArgs e) => await HideOverlay();

    private async void ShowOverlay()
    {
        AlertOverlay.IsVisible = true;
        AlertOverlay.Opacity = 0;
        await AlertOverlay.FadeTo(1, 150);
    }

    private void OnDailyTapped(object sender, TappedEventArgs e)
    {
        // Tungod kay naa na ka sa DailyPulseHistoryPage, 
        // wala na kinahanglan mobalhin og page.
    }

    private async void OnBackClicked(object sender, EventArgs e) => await Navigation.PopAsync();
    private async void OnAddTapped(object sender, TappedEventArgs e) => await Navigation.PushAsync(new DailyLogPage());
    private async void OnHomeTapped(object sender, TappedEventArgs e) => await Navigation.PushAsync(new HomePage());
    private async void OnInsightTapped(object sender, TappedEventArgs e) => await Navigation.PushAsync(new InsightPage());
    private async void OnLearnTapped(object sender, TappedEventArgs e) => await Navigation.PushAsync(new LearnPage());
    private async void OnSettingsTapped(object sender, TappedEventArgs e) => await Navigation.PushAsync(new SettingsPage());
}