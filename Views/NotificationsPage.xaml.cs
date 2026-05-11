using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace Team_Aura_Period_Tracker_;

public partial class NotificationsPage : ContentPage
{
    private readonly DatabaseService _databaseService = new();

    public NotificationsPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Kada abli, i-refresh ang alerts base sa settings ug data
        await LoadDynamicNotificationsAsync();
    }

    private async Task LoadDynamicNotificationsAsync()
    {
        NotificationContainer.Children.Clear();

        int userId = Preferences.Get("UserId", 0);
        var cycle = await _databaseService.GetUserCycleInfoAsync(userId);
        var logs = await _databaseService.GetDailyLogsByUserAsync(userId);

        if (cycle == null)
        {
            NoNotifLabel.IsVisible = true;
            return;
        }

        bool hasAlerts = false;

        // 1. DAILY LOG CHECK (Sync with DailyReminder Setting)
        bool isDailyEnabled = Preferences.Get("DailyReminder", true);
        string todayStr = DateTime.Now.ToString("yyyy-MM-dd");
        if (isDailyEnabled && !logs.Any(l => l.Date == todayStr))
        {
            AddNotificationItem("📝", "Daily Logging", "Don't forget to log your symptoms today.", "Just now");
            hasAlerts = true;
        }

        // 2. CYCLE EVENTS (Sync with Cycle Settings)
        if (DateTime.TryParse(cycle.LastDateOfPeriod, out DateTime lastPeriod))
        {
            int cycleLength = cycle.CycleLengthDays;
            int daysSince = (DateTime.Today - lastPeriod.Date).Days;
            while (daysSince < 0) daysSince += cycleLength;
            int currentDay = (daysSince % cycleLength) + 1;

            // OVULATION ALERT
            bool isOvulationEnabled = Preferences.Get("OvulationReminder", true);
            int ovulationDay = cycleLength - 14;
            if (isOvulationEnabled && currentDay == ovulationDay)
            {
                AddNotificationItem("✨", "Ovulation Day", "You are at your peak ovulation today.", "Today");
                hasAlerts = true;
            }

            // FERTILE WINDOW ALERT
            bool isFertileEnabled = Preferences.Get("FertileReminder", false);
            if (isFertileEnabled && currentDay >= (ovulationDay - 5) && currentDay <= (ovulationDay + 1))
            {
                AddNotificationItem("💚", "Fertile Window", "Your fertile window is active. Energy might be high!", "Current Phase");
                hasAlerts = true;
            }

            // PERIOD PREDICTION ALERT
            bool isPeriodEnabled = Preferences.Get("PeriodReminder", true);
            int daysUntilNext = cycleLength - currentDay + 1;
            if (isPeriodEnabled && daysUntilNext <= 3)
            {
                AddNotificationItem("🌸", "Period Alert", $"Your period is expected in {daysUntilNext} days.", "Coming soon");
                hasAlerts = true;
            }
        }

        // 3. MEDICATION REMINDER (Sync with Setting)
        if (Preferences.Get("MedicationReminder", true))
        {
            AddNotificationItem("💊", "Medication", "Time to take your meds. Stay consistent!", "Routine");
            hasAlerts = true;
        }

        // 4. EXERCISE REMINDER (Sync with Setting)
        if (Preferences.Get("ExerciseReminder", false))
        {
            AddNotificationItem("🏃‍♀️", "Exercise Alert", "Keep your body moving today! Even a short walk helps.", "Daily Goal");
            hasAlerts = true;
        }

        NoNotifLabel.IsVisible = !hasAlerts;
    }

    private void AddNotificationItem(string emoji, string title, string message, string time)
    {
        var card = new Border
        {
            Stroke = Color.FromArgb("#DDD6D8"),
            StrokeThickness = 1,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 12 },
            Padding = 15,
            BackgroundColor = Colors.White,
            Content = new HorizontalStackLayout
            {
                Spacing = 12,
                Children = {
                    new Label { Text = emoji, FontSize = 22, VerticalOptions = LayoutOptions.Center },
                    new VerticalStackLayout {
                        Spacing = 2,
                        Children = {
                            new Label { Text = title, FontAttributes = FontAttributes.Bold, FontSize = 15, TextColor = Color.FromArgb("#333333") },
                            new Label { Text = message, FontSize = 13, TextColor = Color.FromArgb("#6F6A6C") },
                            new Label { Text = time, FontSize = 11, TextColor = Color.FromArgb("#9A9A9A") }
                        }
                    }
                }
            }
        };

        NotificationContainer.Children.Add(card);
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnClearAllClicked(object sender, EventArgs e)
    {
        NotificationContainer.Children.Clear();
        NoNotifLabel.IsVisible = true;
        Preferences.Set("NotificationBellClicked", true);
    }
}