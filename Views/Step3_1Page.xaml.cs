using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;
using System;

namespace Team_Aura_Period_Tracker_
{
    public partial class Step3_1Page : ContentPage
    {
        private int _currentMonth;
        private int _currentYear;
        private DateTime? _selectedStartDate;

        public Step3_1Page()
        {
            InitializeComponent();

            _currentMonth = DateTime.Now.Month;
            _currentYear = DateTime.Now.Year;

            LoadMonthYearPickers();
            UpdateCalendar();
        }

        private void LoadMonthYearPickers()
        {
            MonthPicker.Items.Clear();
            YearPicker.Items.Clear();

            string[] monthNames =
            {
                "Jan", "Feb", "Mar", "Apr", "May", "Jun",
                "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
            };

            foreach (string month in monthNames)
            {
                MonthPicker.Items.Add(month);
            }

            for (int year = DateTime.Now.Year - 50; year <= DateTime.Now.Year + 10; year++)
            {
                YearPicker.Items.Add(year.ToString());
            }

            MonthPicker.SelectedIndex = _currentMonth - 1;

            int yearIndex = YearPicker.Items.IndexOf(_currentYear.ToString());
            if (yearIndex >= 0)
                YearPicker.SelectedIndex = yearIndex;
        }

        private void UpdateCalendar()
        {
            CalendarGrid.Children.Clear();

            DateTime firstDayOfMonth = new DateTime(_currentYear, _currentMonth, 1);
            int daysInMonth = DateTime.DaysInMonth(_currentYear, _currentMonth);
            int startDay = (int)firstDayOfMonth.DayOfWeek;

            DateTime previousMonth = firstDayOfMonth.AddMonths(-1);
            int daysInPreviousMonth = DateTime.DaysInMonth(previousMonth.Year, previousMonth.Month);

            int row = 0;
            int col = 0;

            // Previous month trailing dates
            for (int i = 0; i < startDay; i++)
            {
                int day = daysInPreviousMonth - startDay + i + 1;
                DateTime date = new DateTime(previousMonth.Year, previousMonth.Month, day);
                AddDateCell(date, row, col, true);
                col++;
            }

            // Current month dates
            for (int day = 1; day <= daysInMonth; day++)
            {
                DateTime date = new DateTime(_currentYear, _currentMonth, day);
                AddDateCell(date, row, col, false);

                col++;
                if (col > 6)
                {
                    col = 0;
                    row++;
                }
            }

            // Next month leading dates
            DateTime nextMonth = firstDayOfMonth.AddMonths(1);
            int nextMonthDay = 1;

            while (row < 6)
            {
                DateTime date = new DateTime(nextMonth.Year, nextMonth.Month, nextMonthDay);
                AddDateCell(date, row, col, true);

                nextMonthDay++;
                col++;

                if (col > 6)
                {
                    col = 0;
                    row++;
                }
            }
        }

        private void AddDateCell(DateTime date, int row, int col, bool isOtherMonth)
        {
            bool isInSelectedPeriod = false;
            bool isPeriodStart = false;
            bool isPeriodEnd = false;

            if (_selectedStartDate.HasValue)
            {
                DateTime start = _selectedStartDate.Value.Date;
                DateTime end = start.AddDays(4);

                isInSelectedPeriod = date.Date >= start && date.Date <= end;
                isPeriodStart = date.Date == start;
                isPeriodEnd = date.Date == end;
            }

            Color backgroundColor = Colors.Transparent;
            Color textColor = isOtherMonth ? Color.FromArgb("#B9B9B9") : Color.FromArgb("#2B2B2B");

            if (isInSelectedPeriod)
            {
                if (isPeriodStart || isPeriodEnd)
                {
                    backgroundColor = Color.FromArgb("#2D2D2D");
                    textColor = Colors.White;
                }
                else
                {
                    backgroundColor = Color.FromArgb("#F1F1F1");
                    textColor = Color.FromArgb("#2B2B2B");
                }
            }

            var label = new Label
            {
                Text = date.Day.ToString(),
                FontSize = 16,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                TextColor = textColor,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            var border = new Border
            {
                HeightRequest = 40,
                WidthRequest = 40,
                StrokeThickness = 0,
                BackgroundColor = backgroundColor,
                StrokeShape = new RoundRectangle { CornerRadius = 12 },
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Content = label
            };

            var tap = new TapGestureRecognizer();
            tap.Tapped += (s, e) =>
            {
                _selectedStartDate = date;
                UpdateCalendar();
            };
            border.GestureRecognizers.Add(tap);

            CalendarGrid.Add(border, col, row);
        }

        private void OnMonthOrYearChanged(object sender, EventArgs e)
        {
            if (MonthPicker.SelectedIndex < 0 || YearPicker.SelectedIndex < 0)
                return;

            _currentMonth = MonthPicker.SelectedIndex + 1;
            _currentYear = int.Parse(YearPicker.Items[YearPicker.SelectedIndex]);

            UpdateCalendar();
        }

        private void OnPreviousMonthClicked(object sender, EventArgs e)
        {
            _currentMonth--;

            if (_currentMonth < 1)
            {
                _currentMonth = 12;
                _currentYear--;
            }

            MonthPicker.SelectedIndex = _currentMonth - 1;

            int yearIndex = YearPicker.Items.IndexOf(_currentYear.ToString());
            if (yearIndex >= 0)
                YearPicker.SelectedIndex = yearIndex;

            UpdateCalendar();
        }

        private void OnNextMonthClicked(object sender, EventArgs e)
        {
            _currentMonth++;

            if (_currentMonth > 12)
            {
                _currentMonth = 1;
                _currentYear++;
            }

            MonthPicker.SelectedIndex = _currentMonth - 1;

            int yearIndex = YearPicker.Items.IndexOf(_currentYear.ToString());
            if (yearIndex >= 0)
                YearPicker.SelectedIndex = yearIndex;

            UpdateCalendar();
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void OnNextClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(Step3Page));
        }
    }
}