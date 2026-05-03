using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;

namespace Team_Aura_Period_Tracker_;

public partial class LearnPage : ContentPage
{
    public LearnPage()
    {
        InitializeComponent();
        DisplayLessons(GetAllLessons());
    }

    private void ResetFilters()
    {
        AllButton.BackgroundColor = Color.FromArgb("#F8F8F8");
        CycleHealthButton.BackgroundColor = Color.FromArgb("#F8F8F8");
        ConditionsButton.BackgroundColor = Color.FromArgb("#F8F8F8");

        AllButton.TextColor = Color.FromArgb("#666666");
        CycleHealthButton.TextColor = Color.FromArgb("#666666");
        ConditionsButton.TextColor = Color.FromArgb("#666666");
    }

    private void SelectButton(Button button)
    {
        ResetFilters();
        button.BackgroundColor = Color.FromArgb("#F4CDD5");
        button.TextColor = Color.FromArgb("#333333");
    }

    private void OnFilterClicked(object sender, EventArgs e)
    {
        if (sender is not Button button)
            return;

        SelectButton(button);

        if (button == AllButton)
            DisplayLessons(GetAllLessons());
        else if (button == CycleHealthButton)
            DisplayLessons(GetCycleHealthLessons());
        else if (button == ConditionsButton)
            DisplayLessons(GetConditionLessons());
    }

    private void DisplayLessons((string Category, string Title, string Description, string Content)[] lessons)
    {
        LessonsContainer.Children.Clear();

        foreach (var lesson in lessons)
        {
            LessonsContainer.Children.Add(CreateLessonCard(lesson));
        }
    }

    private Border CreateLessonCard((string Category, string Title, string Description, string Content) lesson)
    {
        var categoryBorder = new Border
        {
            Padding = new Thickness(14, 4),
            BackgroundColor = Color.FromArgb("#F8F8F8"),
            Stroke = Color.FromArgb("#222222"),
            StrokeThickness = 1,
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(14)
            },
            Content = new Label
            {
                Text = lesson.Category,
                FontSize = 13,
                TextColor = Color.FromArgb("#E85B73"),
                HorizontalTextAlignment = TextAlignment.Center
            }
        };

        var arrowLabel = new Label
        {
            Text = "›",
            FontSize = 30,
            TextColor = Color.FromArgb("#222222"),
            VerticalTextAlignment = TextAlignment.Center,
            HorizontalTextAlignment = TextAlignment.End
        };

        var topGrid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Auto }
            },
            ColumnSpacing = 8
        };

        topGrid.Add(categoryBorder, 0, 0);
        topGrid.Add(arrowLabel, 2, 0);

        var card = new Border
        {
            Padding = new Thickness(16, 14, 16, 18),
            BackgroundColor = Color.FromArgb("#F8F8F8"),
            Stroke = Color.FromArgb("#DDD6D8"),
            StrokeThickness = 1,
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(16)
            },
            Content = new VerticalStackLayout
            {
                Spacing = 22,
                Children =
                {
                    topGrid,
                    new Label
                    {
                        Text = lesson.Title,
                        FontSize = 20,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Color.FromArgb("#666666")
                    },
                    new Label
                    {
                        Text = lesson.Description,
                        FontSize = 14,
                        TextColor = Color.FromArgb("#6F6A6C"),
                        LineHeight = 1.35
                    }
                }
            }
        };

        card.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(async () =>
            {
                await ShowScrollableModalAsync(lesson.Title, lesson.Content);
            })
        });

        return card;
    }

    private async Task ShowScrollableModalAsync(string title, string content)
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
            FontSize = 16,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.White,
            BackgroundColor = Color.FromArgb("#E85B73"),
            CornerRadius = 14,
            HeightRequest = 50
        };

        var popup = new Border
        {
            BackgroundColor = Colors.White,
            StrokeThickness = 0,
            Padding = new Thickness(22, 24),
            WidthRequest = 330,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(24)
            },
            Content = new VerticalStackLayout
            {
                Spacing = 18,
                Children =
                {
                    new Label
                    {
                        Text = title,
                        FontSize = 23,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Color.FromArgb("#E85B73"),
                        HorizontalTextAlignment = TextAlignment.Center
                    },
                    new ScrollView
                    {
                        HeightRequest = 330,
                        Content = new Label
                        {
                            Text = content,
                            FontSize = 15,
                            TextColor = Color.FromArgb("#333333"),
                            LineHeight = 1.35
                        }
                    },
                    okButton
                }
            }
        };

        okButton.Clicked += async (s, e) =>
        {
            await overlay.FadeTo(0, 150);
            RootGrid.Children.Remove(overlay);
        };

        overlay.Children.Add(popup);
        RootGrid.Children.Add(overlay);

        overlay.Opacity = 0;
        await overlay.FadeTo(1, 150);
    }

    private (string Category, string Title, string Description, string Content)[] GetAllLessons()
    {
        var cycleLessons = GetCycleHealthLessons();
        var conditionLessons = GetConditionLessons();

        var allLessons = new (string Category, string Title, string Description, string Content)[cycleLessons.Length + conditionLessons.Length];

        cycleLessons.CopyTo(allLessons, 0);
        conditionLessons.CopyTo(allLessons, cycleLessons.Length);

        return allLessons;
    }

    private (string Category, string Title, string Description, string Content)[] GetCycleHealthLessons()
    {
        return new[]
        {
            (
                "Cycle Health",
                "Understanding Your Menstrual Cycle",
                "Learn about the four phases of your cycle and how hormones affect your body and mood.",
                "The menstrual cycle is the monthly process your body goes through to prepare for a possible pregnancy.\n\n" +
                "The four main phases are menstrual phase, follicular phase, ovulation, and luteal phase.\n\n" +
                "Tracking your cycle helps you predict your next period, understand symptoms, and notice changes in your body."
            ),
            (
                "Cycle Health",
                "Menstrual Phase",
                "Understand what happens during your period and why symptoms may appear.",
                "The menstrual phase is when bleeding happens. The uterus sheds its lining, which leaves the body as menstrual blood.\n\n" +
                "You may feel cramps, tiredness, back pain, bloating, or mood changes during this phase."
            ),
            (
                "Cycle Health",
                "Follicular Phase",
                "Learn how your body prepares an egg and how estrogen rises.",
                "The follicular phase begins around the same time as your period and continues until ovulation.\n\n" +
                "Estrogen rises, and your body prepares an egg for release. Some people feel more energetic during this phase."
            ),
            (
                "Cycle Health",
                "Ovulation",
                "Learn when ovulation happens and why it is considered the fertile window.",
                "Ovulation happens when an egg is released from the ovary.\n\n" +
                "This is often the most fertile time of the cycle. Some people notice clearer discharge, mild pelvic pain, or higher energy."
            ),
            (
                "Cycle Health",
                "Luteal Phase",
                "Learn why PMS symptoms may happen before your next period.",
                "The luteal phase happens after ovulation.\n\n" +
                "Progesterone rises. PMS symptoms may happen, such as bloating, cravings, acne, breast tenderness, fatigue, or mood swings."
            )
        };
    }

    private (string Category, string Title, string Description, string Content)[] GetConditionLessons()
    {
        return new[]
        {
            (
                "Conditions",
                "Managing PCOS Naturally",
                "Evidence-based strategies for managing PCOS through lifestyle and nutrition.",
                "PCOS, or Polycystic Ovary Syndrome, is a hormone-related condition that may cause irregular periods, acne, weight changes, and ovulation problems.\n\n" +
                "A tracker can help record period dates, symptoms, mood, and cycle changes."
            ),
            (
                "Conditions",
                "PMS",
                "Learn about common premenstrual symptoms and how tracking can help.",
                "PMS means premenstrual syndrome. It can happen before your period starts.\n\n" +
                "Common symptoms include mood changes, cramps, cravings, acne, bloating, headaches, and tiredness."
            ),
            (
                "Conditions",
                "Irregular Periods",
                "Understand what irregular periods are and when patterns matter.",
                "Irregular periods are cycles that are hard to predict, too short, too long, missed, or very different from your usual pattern.\n\n" +
                "Tracking helps you see if irregularity happens often."
            ),
            (
                "Conditions",
                "Heavy Bleeding",
                "Learn how to track heavy flow and recognize unusual bleeding patterns.",
                "Heavy bleeding means your flow is much heavier than usual or affects your daily activities.\n\n" +
                "Tracking flow can help you explain your experience more clearly to a healthcare provider."
            ),
            (
                "Conditions",
                "When to Seek Help",
                "Know which symptoms may need support from a healthcare provider.",
                "Consider talking to a healthcare provider if you have very severe cramps, very heavy bleeding, missed periods, bleeding between periods, or sudden changes in your cycle.\n\n" +
                "A period tracker can provide helpful records."
            )
        };
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
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

    private void OnLearnTapped(object sender, TappedEventArgs e)
    {
        // Already on Learn page
    }

    private async void OnSettingsTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new SettingsPage());
    }
}