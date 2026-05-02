using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Team_Aura_Period_Tracker_;

public partial class DailyPulseHistoryPage : ContentPage
{
    private readonly DatabaseService _databaseService = new();
    private List<DailyLog> _entries = new();

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

        if (_entries.Count == 0)
        {
            DailyPulseContainer.Children.Add(CreateEmptyState());
            return;
        }

        foreach (var entry in _entries)
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
                Children =
                {
                    new Label
                    {
                        Text = "No daily pulse entries yet",
                        FontSize = 18,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Color.FromArgb("#333333"),
                        HorizontalTextAlignment = TextAlignment.Center
                    },
                    new Label
                    {
                        Text = "Tap + to add your first daily pulse entry.",
                        FontSize = 14,
                        TextColor = Color.FromArgb("#6F6A6C"),
                        HorizontalTextAlignment = TextAlignment.Center
                    }
                }
            }
        };
    }

    private Border CreateDailyPulseCard(DailyLog entry)
    {
        var header = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
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
                    Text = "Daily Pulse Entry",
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

        var deleteButton = new ImageButton
        {
            Source = "delete_icon.png",
            BackgroundColor = Colors.Transparent,
            WidthRequest = 26,
            HeightRequest = 26,
            Padding = 4
        };

        deleteButton.Clicked += async (s, e) =>
        {
            await DeleteEntryAsync(entry);
        };

        Grid.SetColumn(titleStack, 0);
        Grid.SetColumn(deleteButton, 1);

        header.Children.Add(titleStack);
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
                MakeField("Flow", string.IsNullOrWhiteSpace(entry.Flow) ? "—" : entry.Flow),
                MakeField("Mood", string.IsNullOrWhiteSpace(entry.Mood) ? "—" : entry.Mood)
            }
        };

        var rightColumn = new VerticalStackLayout
        {
            Spacing = 16,
            Children =
            {
                MakeField("Energy", $"{Math.Round(entry.EnergyLevel)}/10"),
                MakeField("Symptoms", string.IsNullOrWhiteSpace(entry.Symptoms) ? "—" : entry.Symptoms)
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

    private async Task DeleteEntryAsync(DailyLog entry)
    {
        bool confirm = await ShowCustomConfirmAlertAsync(
            "Delete",
            "Delete this daily pulse entry?",
            "Yes",
            "No");

        if (!confirm)
            return;

        await _databaseService.DeleteDailyLogAsync(entry);

        await ShowCustomAlertAsync(
            "Success",
            "Daily pulse entry deleted successfully.");

        await LoadEntriesAsync();
    }

    private async Task ShowCustomAlertAsync(string title, string message)
    {
        var overlay = new Grid
        {
            BackgroundColor = Color.FromRgba(0, 0, 0, 0.55),
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill
        };

        var okButton = new Button
        {
            Text = "OK",
            FontSize = 18,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.White,
            BackgroundColor = Color.FromArgb("#E85472"),
            CornerRadius = 14,
            HeightRequest = 56
        };

        var popup = new Border
        {
            BackgroundColor = Colors.White,
            StrokeThickness = 0,
            Padding = new Thickness(28, 26),
            WidthRequest = 320,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(24)
            },
            Content = new VerticalStackLayout
            {
                Spacing = 22,
                Children =
                {
                    new Label
                    {
                        Text = title,
                        FontSize = 28,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Color.FromArgb("#E85472"),
                        HorizontalTextAlignment = TextAlignment.Center
                    },
                    new Label
                    {
                        Text = message,
                        FontSize = 18,
                        TextColor = Color.FromArgb("#333333"),
                        HorizontalTextAlignment = TextAlignment.Center
                    },
                    okButton
                }
            }
        };

        overlay.Children.Add(popup);

        var tcs = new TaskCompletionSource();

        okButton.Clicked += async (s, e) =>
        {
            await overlay.FadeTo(0, 150);
            ((Grid)Content).Children.Remove(overlay);
            tcs.SetResult();
        };

        ((Grid)Content).Children.Add(overlay);

        overlay.Opacity = 0;
        await overlay.FadeTo(1, 150);

        await tcs.Task;
    }

    private async Task<bool> ShowCustomConfirmAlertAsync(
        string title,
        string message,
        string accept,
        string cancel)
    {
        var overlay = new Grid
        {
            BackgroundColor = Color.FromRgba(0, 0, 0, 0.55),
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill
        };

        var yesButton = new Button
        {
            Text = accept,
            FontSize = 18,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.White,
            BackgroundColor = Color.FromArgb("#E85472"),
            CornerRadius = 14,
            HeightRequest = 56
        };

        var noButton = new Button
        {
            Text = cancel,
            FontSize = 18,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#E85472"),
            BackgroundColor = Colors.White,
            BorderColor = Color.FromArgb("#E85472"),
            BorderWidth = 1,
            CornerRadius = 14,
            HeightRequest = 56
        };

        var popup = new Border
        {
            BackgroundColor = Colors.White,
            StrokeThickness = 0,
            Padding = new Thickness(28, 26),
            WidthRequest = 320,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(24)
            },
            Content = new VerticalStackLayout
            {
                Spacing = 20,
                Children =
                {
                    new Label
                    {
                        Text = title,
                        FontSize = 28,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Color.FromArgb("#E85472"),
                        HorizontalTextAlignment = TextAlignment.Center
                    },
                    new Label
                    {
                        Text = message,
                        FontSize = 18,
                        TextColor = Color.FromArgb("#333333"),
                        HorizontalTextAlignment = TextAlignment.Center
                    },
                    yesButton,
                    noButton
                }
            }
        };

        overlay.Children.Add(popup);

        var tcs = new TaskCompletionSource<bool>();

        yesButton.Clicked += async (s, e) =>
        {
            await overlay.FadeTo(0, 150);
            ((Grid)Content).Children.Remove(overlay);
            tcs.SetResult(true);
        };

        noButton.Clicked += async (s, e) =>
        {
            await overlay.FadeTo(0, 150);
            ((Grid)Content).Children.Remove(overlay);
            tcs.SetResult(false);
        };

        ((Grid)Content).Children.Add(overlay);

        overlay.Opacity = 0;
        await overlay.FadeTo(1, 150);

        return await tcs.Task;
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnAddTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new DailyLogPage());
    }

    private async void OnHomeTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new HomePage());
    }

    private void OnDailyTapped(object sender, TappedEventArgs e)
    {
        // Already on Daily Pulse history page
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