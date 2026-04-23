using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;
using System;

namespace Team_Aura_Period_Tracker_;

public partial class LearnPage : ContentPage
{
    public LearnPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadEntries();
    }

    private void LoadEntries()
    {
        EntriesContainer.Children.Clear();

        foreach (var entry in JournalEntryStore.Entries)
        {
            var leftColumn = new VerticalStackLayout
            {
                Spacing = 16,
                Children =
                {
                    MakeField("Mood", entry.Mood),
                    MakeField("Sleep", entry.Sleep),
                    MakeField("Medication", entry.Medication)
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

            Grid.SetColumn(rightColumn, 1);

            var infoGrid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Star)
                },
                ColumnSpacing = 28
            };

            infoGrid.Children.Add(leftColumn);
            infoGrid.Children.Add(rightColumn);

            var card = new Border
            {
                Padding = new Thickness(20, 18, 20, 18),
                BackgroundColor = Color.FromArgb("#F8F8F8"),
                Stroke = Color.FromArgb("#DDD6D8"),
                StrokeThickness = 1,
                StrokeShape = new RoundRectangle { CornerRadius = 18 },
                Content = new VerticalStackLayout
                {
                    Spacing = 16,
                    Children =
                    {
                        new VerticalStackLayout
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
                                    Text = entry.DateText,
                                    FontSize = 13,
                                    TextColor = Color.FromArgb("#8A8A8A")
                                }
                            }
                        },
                        infoGrid,
                        new VerticalStackLayout
                        {
                            Spacing = 2,
                            Children =
                            {
                                new Label
                                {
                                    Text = "Notes",
                                    FontSize = 12,
                                    TextColor = Color.FromArgb("#8A8A8A")
                                },
                                new Label
                                {
                                    Text = entry.Notes,
                                    FontSize = 14,
                                    TextColor = Color.FromArgb("#555555")
                                }
                            }
                        }
                    }
                }
            };

            EntriesContainer.Children.Add(card);
        }
    }

    private VerticalStackLayout MakeField(string title, string value)
    {
        return new VerticalStackLayout
        {
            Spacing = 2,
            Children =
            {
                new Label
                {
                    Text = title,
                    FontSize = 12,
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
        await DisplayAlert("Learn", "You are already on Learn.", "OK");
    }

    private async void OnSettingsTapped(object sender, TappedEventArgs e)
    {
        await DisplayAlert("Settings", "Open settings page.", "OK");
    }
}