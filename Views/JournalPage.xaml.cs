using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Team_Aura_Period_Tracker_;

public partial class JournalPage : ContentPage
{
    private readonly DatabaseService _databaseService = new();
    private List<HealthJournal> _entries = new();
    private HealthJournal _journalToDelete;

    public JournalPage()
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
        JournalContainer.Children.Clear();
        int userId = Preferences.Get("UserId", 0);

        if (userId == 0) return;

        _entries = await _databaseService.GetHealthJournalsByUserAsync(userId);

        if (_entries == null || _entries.Count == 0)
        {
            JournalContainer.Children.Add(new Label { Text = "No entries yet", HorizontalTextAlignment = TextAlignment.Center, Margin = 20 });
            return;
        }

        foreach (var entry in _entries.OrderByDescending(e => e.Date))
        {
            JournalContainer.Children.Add(CreateJournalCard(entry));
        }
    }

    private Border CreateJournalCard(HealthJournal entry)
    {
        // Header with Edit/Delete
        var header = new Grid { ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto), new ColumnDefinition(GridLength.Auto) }, ColumnSpacing = 8 };
        var titleStack = new VerticalStackLayout
        {
            Children = {
            new Label { Text = entry.Title, FontSize = 18, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#333333") },
            new Label { Text = entry.Date, FontSize = 14, TextColor = Color.FromArgb("#8A8A8A"), Margin = new Thickness(0,0,0,10) }
        }
        };
        var editBtn = new ImageButton { Source = "edit_icon.png", BackgroundColor = Colors.Transparent, WidthRequest = 24, HeightRequest = 24 };
        var delBtn = new ImageButton { Source = "delete_icon.png", BackgroundColor = Colors.Transparent, WidthRequest = 24, HeightRequest = 24 };

        editBtn.Clicked += async (s, e) => await Navigation.PushAsync(new AddJournalEntryPage(entry.Id));
        delBtn.Clicked += (s, e) => { _journalToDelete = entry; ShowOverlay(); };

        Grid.SetColumn(titleStack, 0); Grid.SetColumn(editBtn, 1); Grid.SetColumn(delBtn, 2);
        header.Children.Add(titleStack); header.Children.Add(editBtn); header.Children.Add(delBtn);

        // Grid Design matching Screenshot 2026-05-14 005106.png
        var statsGrid = new Grid { ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Star) }, RowDefinitions = { new RowDefinition(GridLength.Auto), new RowDefinition(GridLength.Auto) }, RowSpacing = 15, ColumnSpacing = 10 };
        statsGrid.Add(MakeField("Mood", entry.Mood), 0, 0);
        statsGrid.Add(MakeField("Energy", entry.Energy), 1, 0);
        statsGrid.Add(MakeField("Sleep", entry.Sleep), 0, 1);
        statsGrid.Add(MakeField("Exercise", entry.Exercise), 1, 1);

        return new Border
        {
            Padding = 24,
            BackgroundColor = Colors.White,
            StrokeShape = new RoundRectangle { CornerRadius = 24 },
            Margin = new Thickness(0, 0, 0, 10),
            Content = new VerticalStackLayout
            {
                Spacing = 15,
                Children = {
                header, statsGrid,
                MakeField("Medication", string.IsNullOrWhiteSpace(entry.Medication) ? "None" : entry.Medication),
                new BoxView { HeightRequest = 0.5, Color = Color.FromArgb("#DDD6D8") },
                MakeField("Notes", entry.Notes)
            }
            }
        };
    }

    private VerticalStackLayout MakeField(string label, string value)
    {
        return new VerticalStackLayout
        {
            Spacing = 2,
            Children = {
            new Label { Text = label, FontSize = 13, TextColor = Color.FromArgb("#999999") },
            new Label { Text = string.IsNullOrWhiteSpace(value) ? "—" : value, FontSize = 15, TextColor = Color.FromArgb("#555555") }
        }
        };
    }

    private async void OnConfirmDeleteClicked(object sender, EventArgs e)
    {
        HideOverlay();
        if (_journalToDelete != null)
        {
            await _databaseService.DeleteHealthJournalAsync(_journalToDelete);
            await LoadEntriesAsync();
        }
    }

    private void OnCancelAlertClicked(object sender, EventArgs e) => HideOverlay();
    private void ShowOverlay() { AlertOverlay.IsVisible = true; AlertOverlay.Opacity = 0; AlertOverlay.FadeTo(1, 150); }
    private void HideOverlay() { AlertOverlay.FadeTo(0, 150); AlertOverlay.IsVisible = false; }

    // Navigation
    private async void OnBackClicked(object sender, EventArgs e) => await Navigation.PopAsync();
    private async void OnAddTapped(object sender, TappedEventArgs e) => await Navigation.PushAsync(new AddJournalEntryPage());
    private async void OnHomeTapped(object sender, TappedEventArgs e) => await Navigation.PushAsync(new HomePage());
    private void OnDailyTapped(object sender, TappedEventArgs e) => Navigation.PushAsync(new DailyLogPage());
    private async void OnInsightTapped(object sender, TappedEventArgs e) => await Navigation.PushAsync(new InsightPage());
    private async void OnLearnTapped(object sender, TappedEventArgs e) => await Navigation.PushAsync(new LearnPage());
    private async void OnSettingsTapped(object sender, TappedEventArgs e) => await Navigation.PushAsync(new SettingsPage());
}