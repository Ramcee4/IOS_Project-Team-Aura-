using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;

namespace Team_Aura_Period_Tracker_;

public partial class Step3_1Page : ContentPage
{
    private int _currentMonth = DateTime.Now.Month;
    private int _currentYear = DateTime.Now.Year;
    private DateTime? _selectedDate;
    private readonly string[] _monthNames = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

    public Step3_1Page()
    {
        InitializeComponent();
        UpdateCalendarHeader();
        UpdateCalendar();
    }

    private void UpdateCalendarHeader()
    {
        MonthLabel.Text = _monthNames[_currentMonth - 1];
        YearLabel.Text = _currentYear.ToString();
    }

    private void UpdateCalendar()
    {
        CalendarGrid.Children.Clear();
        DateTime firstDay = new DateTime(_currentYear, _currentMonth, 1);
        int daysInMonth = DateTime.DaysInMonth(_currentYear, _currentMonth);
        int startDay = (int)firstDay.DayOfWeek;

        DateTime prevMonth = firstDay.AddMonths(-1);
        int daysInPrev = DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month);

        int row = 0; int col = 0;
        for (int i = 0; i < startDay; i++)
        {
            AddDateCell(new DateTime(prevMonth.Year, prevMonth.Month, daysInPrev - startDay + i + 1), row, col, true);
            col++;
        }

        for (int day = 1; day <= daysInMonth; day++)
        {
            AddDateCell(new DateTime(_currentYear, _currentMonth, day), row, col, false);
            col++;
            if (col > 6) { col = 0; row++; }
        }

        int nextDay = 1;
        while (row < 6)
        {
            AddDateCell(new DateTime(firstDay.AddMonths(1).Year, firstDay.AddMonths(1).Month, nextDay++), row, col, true);
            col++; if (col > 6) { col = 0; row++; }
        }
    }

    private void AddDateCell(DateTime date, int row, int col, bool isOtherMonth)
    {
        bool isSelected = _selectedDate.HasValue && date.Date == _selectedDate.Value.Date;
        var border = new Border
        {
            HeightRequest = 40,
            WidthRequest = 40,
            StrokeThickness = 0,
            BackgroundColor = isSelected ? Color.FromArgb("#E85B73") : Colors.Transparent,
            StrokeShape = new RoundRectangle { CornerRadius = 20 },
            Content = new Label
            {
                Text = date.Day.ToString(),
                FontSize = 15,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                TextColor = isSelected ? Colors.White : (isOtherMonth ? Color.FromArgb("#B9B9B9") : Color.FromArgb("#2B2B2B"))
            }
        };
        var tap = new TapGestureRecognizer();
        tap.Tapped += (s, e) => { _selectedDate = date; UpdateCalendar(); };
        border.GestureRecognizers.Add(tap);
        CalendarGrid.Add(border, col, row);
    }

    // --- CUSTOM SELECTION OVERLAY LOGIC ---
    private void OnMonthLabelTapped(object sender, EventArgs e)
    {
        SelectionTitle.Text = "Select Month";
        SelectionContainer.Children.Clear();
        for (int i = 0; i < _monthNames.Length; i++)
        {
            int index = i + 1;
            SelectionContainer.Children.Add(CreateSelectionItem(_monthNames[i], () => {
                _currentMonth = index;
                UpdateCalendarHeader();
                UpdateCalendar();
                HideSelection();
            }));
        }
        ShowSelection();
    }

    private void OnYearLabelTapped(object sender, EventArgs e)
    {
        SelectionTitle.Text = "Select Year";
        SelectionContainer.Children.Clear();
        for (int y = DateTime.Now.Year - 10; y <= DateTime.Now.Year + 2; y++)
        {
            int yearVal = y;
            SelectionContainer.Children.Add(CreateSelectionItem(y.ToString(), () => {
                _currentYear = yearVal;
                UpdateCalendarHeader();
                UpdateCalendar();
                HideSelection();
            }));
        }
        ShowSelection();
    }

    private Button CreateSelectionItem(string text, Action onSelect)
    {
        var btn = new Button { Text = text, BackgroundColor = Colors.Transparent, TextColor = Color.FromArgb("#444444"), FontSize = 17, HeightRequest = 50 };
        btn.Clicked += (s, e) => onSelect();
        return btn;
    }

    private async void ShowSelection() { SelectionOverlay.IsVisible = true; SelectionOverlay.Opacity = 0; await SelectionOverlay.FadeTo(1, 150); }
    private async void HideSelection() { await SelectionOverlay.FadeTo(0, 150); SelectionOverlay.IsVisible = false; }
    private void OnCancelSelectionClicked(object sender, EventArgs e) => HideSelection();

    // --- ALERTS & NAVIGATION ---
    private void OnPreviousMonthClicked(object sender, EventArgs e) { _currentMonth--; if (_currentMonth < 1) { _currentMonth = 12; _currentYear--; } UpdateCalendarHeader(); UpdateCalendar(); }
    private void OnNextMonthClicked(object sender, EventArgs e) { _currentMonth++; if (_currentMonth > 12) { _currentMonth = 1; _currentYear++; } UpdateCalendarHeader(); UpdateCalendar(); }

    private async void ShowCustomAlert(string title, string message) { CustomAlertTitle.Text = title; CustomAlertMessage.Text = message; CustomAlertOverlay.IsVisible = true; CustomAlertOverlay.Opacity = 0; await CustomAlertOverlay.FadeTo(1, 150); }
    private async void OnCustomAlertOkClicked(object sender, EventArgs e) { await CustomAlertOverlay.FadeTo(0, 150); CustomAlertOverlay.IsVisible = false; }
    private async void OnBackClicked(object sender, EventArgs e) => await Shell.Current.GoToAsync("..");
    private async void OnNextClicked(object sender, EventArgs e)
    {
        if (!_selectedDate.HasValue) { ShowCustomAlert("Required", "Please select your last period date."); return; }
        Preferences.Set("LastDateOfPeriod", _selectedDate.Value.ToString("yyyy-MM-dd"));
        await Shell.Current.GoToAsync("..");
    }
}