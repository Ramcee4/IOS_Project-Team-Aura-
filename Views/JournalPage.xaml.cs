using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Team_Aura_Period_Tracker_;

public partial class JournalPage : ContentPage
{
    private readonly List<JournalItem> _entries = new()
    {
        new JournalItem
        {
            Title = "Day 5 of Cycle",
            Date = "2026-03-06",
            Mood = "Good",
            Energy = "4/10",
            Sleep = "7 hrs",
            Exercise = "Rest Day",
            Medication = "None",
            Notes = "Feeling better today, cramps subsiding"
        },
        new JournalItem
        {
            Title = "Day 4 of Cycle",
            Date = "2026-03-05",
            Mood = "Neutral",
            Energy = "4/10",
            Sleep = "6 hrs",
            Exercise = "Rest Day",
            Medication = "Ibuprofen",
            Notes = "Heavy flow, took it easy today"
        }
    };

    public JournalPage()
    {
        InitializeComponent();
        LoadEntries();
    }

    private void LoadEntries()
    {
        JournalContainer.Children.Clear();

        foreach (var entry in _entries)
        {
            JournalContainer.Children.Add(CreateJournalCard(entry));
        }
    }

    private Border CreateJournalCard(JournalItem entry)
    {
        var header = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto),
                new ColumnDefinition(GridLength.Auto)
            },
            ColumnSpacing = 16
        };

        var titleStack = new VerticalStackLayout
        {
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
            WidthRequest = 20,
            HeightRequest = 20
        };

        var deleteButton = new ImageButton
        {
            Source = "delete_icon.png",
            BackgroundColor = Colors.Transparent,
            WidthRequest = 20,
            HeightRequest = 20
        };

        editButton.Clicked += (s, e) => EditEntry(entry);
        deleteButton.Clicked += async (s, e) => await DeleteEntry(entry);

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
                MakeField("Mood", entry.Mood),
                MakeField("Sleep", entry.Sleep)
            }
        };

        var rightColumn = new VerticalStackLayout
        {
            Spacing = 16,
            Children =
            {
                MakeField("Energy", entry.Energy),
                MakeField("Exercise", entry.Exercise)
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
                    MakeField("Medication", entry.Medication),
                    new BoxView
                    {
                        HeightRequest = 3,
                        Color = Color.FromArgb("#EBDCF5")
                    },
                    MakeField("Notes", entry.Notes)
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
                    TextColor = Color.FromArgb("#555555")
                }
            }
        };
    }

    private async void EditEntry(JournalItem entry)
    {
        string result = await DisplayPromptAsync(
            "Update Entry",
            "Update title:",
            initialValue: entry.Title);

        if (!string.IsNullOrWhiteSpace(result))
        {
            entry.Title = result.Trim();
            LoadEntries();
        }
    }

    private async Task DeleteEntry(JournalItem entry)
    {
        bool confirm = await DisplayAlert(
            "Delete",
            "Delete this journal entry?",
            "Yes",
            "No");

        if (confirm)
        {
            _entries.Remove(entry);
            LoadEntries();
        }
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

    private class JournalItem
    {
        public string Title { get; set; } = "";
        public string Date { get; set; } = "";
        public string Mood { get; set; } = "";
        public string Energy { get; set; } = "";
        public string Sleep { get; set; } = "";
        public string Exercise { get; set; } = "";
        public string Medication { get; set; } = "";
        public string Notes { get; set; } = "";
    }
}