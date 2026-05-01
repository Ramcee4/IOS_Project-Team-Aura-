using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Team_Aura_Period_Tracker_;

public partial class JournalPage : ContentPage
{
    private readonly DatabaseService _databaseService = new();
    private List<HealthJournal> _entries = new();

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

        if (userId == 0)
        {
            JournalContainer.Children.Add(CreateEmptyState());
            return;
        }

        _entries = await _databaseService.GetHealthJournalsByUserAsync(userId);

        if (_entries.Count == 0)
        {
            JournalContainer.Children.Add(CreateEmptyState());
            return;
        }

        foreach (var entry in _entries)
        {
            JournalContainer.Children.Add(CreateJournalCard(entry));
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
                Children =
                {
                    new Label
                    {
                        Text = "No journal entries yet",
                        FontSize = 18,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Color.FromArgb("#333333"),
                        HorizontalTextAlignment = TextAlignment.Center
                    },
                    new Label
                    {
                        Text = "Tap + to add your first health journal entry.",
                        FontSize = 14,
                        TextColor = Color.FromArgb("#6F6A6C"),
                        HorizontalTextAlignment = TextAlignment.Center
                    }
                }
            }
        };
    }

    private Border CreateJournalCard(HealthJournal entry)
    {
        var header = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto),
                new ColumnDefinition(GridLength.Auto)
            },
            ColumnSpacing = 12
        };

        var titleStack = new VerticalStackLayout
        {
            Spacing = 2,
            Children =
            {
                new Label
                {
                    Text = entry.Title,
                    FontSize = 16,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.FromArgb("#333333")
                },
                new Label
                {
                    Text = entry.Date,
                    FontSize = 13,
                    TextColor = Color.FromArgb("#8A8A8A")
                }
            }
        };

        var editButton = new ImageButton
        {
            Source = "edit_icon.png",
            BackgroundColor = Colors.Transparent,
            WidthRequest = 26,
            HeightRequest = 26,
            Padding = 4
        };

        var deleteButton = new ImageButton
        {
            Source = "delete_icon.png",
            BackgroundColor = Colors.Transparent,
            WidthRequest = 26,
            HeightRequest = 26,
            Padding = 4
        };

        editButton.Clicked += async (s, e) =>
        {
            await Navigation.PushAsync(new AddJournalEntryPage(entry.Id));
        };

        deleteButton.Clicked += async (s, e) =>
        {
            await DeleteEntryAsync(entry);
        };

        Grid.SetColumn(titleStack, 0);
        Grid.SetColumn(editButton, 1);
        Grid.SetColumn(deleteButton, 2);

        header.Children.Add(titleStack);
        header.Children.Add(editButton);
        header.Children.Add(deleteButton);

        var infoGrid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star)
            },
            ColumnSpacing = 28
        };

        var leftColumn = new VerticalStackLayout
        {
            Spacing = 16,
            Children =
            {
                MakeField("Mood", string.IsNullOrWhiteSpace(entry.Mood) ? "—" : entry.Mood),
                MakeField("Sleep", string.IsNullOrWhiteSpace(entry.Sleep) ? "—" : entry.Sleep)
            }
        };

        var rightColumn = new VerticalStackLayout
        {
            Spacing = 16,
            Children =
            {
                MakeField("Energy", string.IsNullOrWhiteSpace(entry.Energy) ? "—" : entry.Energy),
                MakeField("Exercise", string.IsNullOrWhiteSpace(entry.Exercise) ? "—" : entry.Exercise)
            }
        };

        Grid.SetColumn(leftColumn, 0);
        Grid.SetColumn(rightColumn, 1);

        infoGrid.Children.Add(leftColumn);
        infoGrid.Children.Add(rightColumn);

        return new Border
        {
            Padding = new Thickness(20, 18),
            BackgroundColor = Color.FromArgb("#F8F8F8"),
            Stroke = Color.FromArgb("#DDD6D8"),
            StrokeThickness = 1,
            StrokeShape = new RoundRectangle { CornerRadius = 18 },
            Content = new VerticalStackLayout
            {
                Spacing = 14,
                Children =
                {
                    header,
                    infoGrid,
                    new BoxView
                    {
                        HeightRequest = 3,
                        Color = Color.FromArgb("#EBDCF5")
                    },
                    MakeField("Medication", string.IsNullOrWhiteSpace(entry.Medication) ? "—" : entry.Medication),
                    new BoxView
                    {
                        HeightRequest = 3,
                        Color = Color.FromArgb("#EBDCF5")
                    },
                    MakeField("Notes", string.IsNullOrWhiteSpace(entry.Notes) ? "—" : entry.Notes)
                }
            }
        };
    }

    private VerticalStackLayout MakeField(string label, string value)
    {
        return new VerticalStackLayout
        {
            Spacing = 0,
            Children =
            {
                new Label
                {
                    Text = label,
                    FontSize = 13,
                    TextColor = Color.FromArgb("#8A8A8A")
                },
                new Label
                {
                    Text = value,
                    FontSize = 14,
                    TextColor = Color.FromArgb("#555555"),
                    LineBreakMode = LineBreakMode.WordWrap
                }
            }
        };
    }

    private async Task DeleteEntryAsync(HealthJournal entry)
    {
        bool confirm = await DisplayAlert(
            "Delete",
            "Delete this journal entry?",
            "Yes",
            "No");

        if (!confirm)
            return;

        await _databaseService.DeleteHealthJournalAsync(entry);
        await LoadEntriesAsync();
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnAddTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new AddJournalEntryPage());
    }

    private async void OnHomeTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new HomePage());
    }

    private async void OnDailyTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new DailyLogPage());
    }

    private async void OnInsightTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new InsightPage());
    }

    private async void OnLearnTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new LearnPage());
    }

    private async void OnSettingsTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new SettingsPage());
    }
}