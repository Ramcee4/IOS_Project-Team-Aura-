using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Storage;

namespace Team_Aura_Period_Tracker_;

public partial class Step3_1Page : ContentPage
{
    private DateTime _displayedMonth;
    private DateTime? _selectedDate;

    public Step3_1Page()
    {
        InitializeComponent();

        _displayedMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        _selectedDate = DateTime.Today;

        LoadPickers();
        RenderCalendar();
    }

    private void LoadPickers()
    {
        MonthPicker.ItemsSource = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames
            .Where(m => !string.IsNullOrWhiteSpace(m))
            .ToList();

        var years = Enumerable.Range(DateTime.Today.Year - 10, 21)
            .Select(y => y.ToString())
            .ToList();

        YearPicker.ItemsSource = years;

        MonthPicker.SelectedIndex = _displayedMonth.Month - 1;
        YearPicker.SelectedItem = _displayedMonth.Year.ToString();
    }

    private void OnMonthOrYearChanged(object sender, EventArgs e)
    {
        if (MonthPicker.SelectedIndex < 0 || YearPicker.SelectedItem is null)
            return;

        int month = MonthPicker.SelectedIndex + 1;
        int year = int.Parse(YearPicker.SelectedItem.ToString()!);

        _displayedMonth = new DateTime(year, month, 1);
        RenderCalendar();
    }

    private void OnPreviousMonthClicked(object sender, EventArgs e)
    {
        _displayedMonth = _displayedMonth.AddMonths(-1);
        SyncPickers();
        RenderCalendar();
    }

    private void OnNextMonthClicked(object sender, EventArgs e)
    {
        _displayedMonth = _displayedMonth.AddMonths(1);
        SyncPickers();
        RenderCalendar();
    }

    private void SyncPickers()
    {
        MonthPicker.SelectedIndex = _displayedMonth.Month - 1;
        YearPicker.SelectedItem = _displayedMonth.Year.ToString();
    }

    private void RenderCalendar()
    {
        CalendarGrid.Children.Clear();

        var firstDay = new DateTime(_displayedMonth.Year, _displayedMonth.Month, 1);
        int daysInMonth = DateTime.DaysInMonth(_displayedMonth.Year, _displayedMonth.Month);
        int startColumn = (int)firstDay.DayOfWeek;

        int currentRow = 0;
        int currentColumn = startColumn;

        for (int day = 1; day <= daysInMonth; day++)
        {
            var date = new DateTime(_displayedMonth.Year, _displayedMonth.Month, day);
            var button = CreateDayButton(date);
            CalendarGrid.Add(button, currentColumn, currentRow);

            currentColumn++;
            if (currentColumn > 6)
            {
                currentColumn = 0;
                currentRow++;
            }
        }
    }

    private Button CreateDayButton(DateTime date)
    {
        bool isSelected = _selectedDate.HasValue && _selectedDate.Value.Date == date.Date;

        var button = new Button
        {
            Text = date.Day.ToString(),
            FontSize = 16,
            CornerRadius = 10,
            HeightRequest = 40,
            WidthRequest = 40,
            Padding = 0,
            BackgroundColor = isSelected ? Color.FromArgb("#333333") : Color.FromArgb("#F8F8F8"),
            TextColor = isSelected ? Colors.White : Color.FromArgb("#333333"),
            BorderColor = Colors.Transparent,
            BorderWidth = 0,
            CommandParameter = date
        };

        button.Clicked += OnDayClicked;
        return button;
    }

    private void OnDayClicked(object? sender, EventArgs e)
    {
        if (sender is not Button button || button.CommandParameter is not DateTime date)
            return;

        _selectedDate = date;
        RenderCalendar();
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnNextClicked(object sender, EventArgs e)
    {
        if (_selectedDate == null)
        {
            await DisplayAlert("Required", "Please select a date.", "OK");
            return;
        }

        Preferences.Set("lastPeriodDate", _selectedDate.Value.ToString("yyyy-MM-dd"));
        await Navigation.PopAsync();
    }
}