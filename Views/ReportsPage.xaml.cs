using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace Team_Aura_Period_Tracker_
{
    public partial class ReportsPage : ContentPage
    {
        private readonly DatabaseService _databaseService = new();

        public ReportsPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadReportDataAsync();
        }

        private async Task LoadReportDataAsync()
        {
            int userId = Preferences.Get("UserId", 0);
            var cycleInfo = await _databaseService.GetUserCycleInfoAsync(userId);
            var dailyLogs = await _databaseService.GetDailyLogsByUserAsync(userId);

            // 1. I-reset ang UI sa sugod para limpyo
            ResetUI();

            // 2. I-check ang cycleInfo. Kung naa, i-display ang values.
            // Dili ni dapat mawala maski walay logs.
            if (cycleInfo != null)
            {
                AvgCycleLengthLabel.Text = cycleInfo.CycleLengthDays.ToString();
                AvgPeriodLabel.Text = cycleInfo.PeriodDays.ToString();
            }

            int cycleLength = cycleInfo?.CycleLengthDays ?? 28;

            // 3. Kung walay logs, undang na diri (pero ang cycle info labels na-set na sa babaw)
            if (dailyLogs == null || dailyLogs.Count == 0) return;

            // 4. I-display ang total logs count
            TotalLogsLabel.Text = dailyLogs.Count.ToString();

            // 5. I-calculate ang symptoms frequency
            var symptomCounts = dailyLogs
                .Where(l => !string.IsNullOrWhiteSpace(l.Symptoms))
                .SelectMany(l => l.Symptoms.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                           .Select(s => s.Trim().ToLower()))
                .GroupBy(s => s)
                .ToDictionary(g => g.Key, g => g.Count());

            // Update tanan symptom UI...
            UpdateSymptomUI("cramps", symptomCounts, cycleLength, CrampsProgress, CrampsPercent);
            UpdateSymptomUI("weak", symptomCounts, cycleLength, WeakProgress, WeakPercent);
            UpdateSymptomUI("dull", symptomCounts, cycleLength, DullProgress, DullPercent);
            UpdateSymptomUI("acne", symptomCounts, cycleLength, AcneProgress, AcnePercent);
            UpdateSymptomUI("moody", symptomCounts, cycleLength, MoodyProgress, MoodyPercent);
            UpdateSymptomUI("blue", symptomCounts, cycleLength, BlueProgress, BluePercent);
            UpdateSymptomUI("bloating", symptomCounts, cycleLength, BloatingProgress, BloatingPercent);
            UpdateSymptomUI("craving", symptomCounts, cycleLength, CravingProgress, CravingPercent);
            UpdateSymptomUI("heat", symptomCounts, cycleLength, HeatProgress, HeatPercent);
            UpdateSymptomUI("nausea", symptomCounts, cycleLength, NauseaProgress, NauseaPercent);
            UpdateSymptomUI("restless", symptomCounts, cycleLength, RestlessProgress, RestlessPercent);
            UpdateSymptomUI("oily", symptomCounts, cycleLength, OilyProgress, OilyPercent);
            UpdateSymptomUI("fatigue", symptomCounts, cycleLength, FatigueProgress, FatiguePercent);
            UpdateSymptomUI("soreness", symptomCounts, cycleLength, SorenessProgress, SorenessPercent);
            UpdateSymptomUI("rush", symptomCounts, cycleLength, RushProgress, RushPercent);
            UpdateSymptomUI("anxiety", symptomCounts, cycleLength, AnxietyProgress, AnxietyPercent);
            UpdateSymptomUI("irritate", symptomCounts, cycleLength, IrritateProgress, IrritatePercent);
            UpdateSymptomUI("sick", symptomCounts, cycleLength, SickProgress, SickPercent);
        }

        private void UpdateSymptomUI(
            string symptomKey,
            Dictionary<string, int> dict,
            int cycleLength,
            ProgressBar progress,
            Label percentLabel)
        {
            string key = symptomKey.ToLower();
            int count = dict.ContainsKey(key) ? dict[key] : 0;

            // Formula: (Pila ka adlaw naay symptom) divided by (Total days sa cycle)
            double percentage = (double)count / cycleLength;

            // Cap the progress at 1.0 (100%) just in case
            progress.Progress = Math.Min(1.0, percentage);
            percentLabel.Text = $"{(int)(Math.Min(1.0, percentage) * 100)}%";
        }

        private void ResetUI()
        {
            // Cycle Summary Reset
            TotalLogsLabel.Text = "0";
            AvgCycleLengthLabel.Text = "--";
            AvgPeriodLabel.Text = "--";

            // Symptoms Reset (Tanan 18 symptoms)
            CrampsProgress.Progress = 0; CrampsPercent.Text = "0%";
            WeakProgress.Progress = 0; WeakPercent.Text = "0%";
            DullProgress.Progress = 0; DullPercent.Text = "0%";
            AcneProgress.Progress = 0; AcnePercent.Text = "0%";
            MoodyProgress.Progress = 0; MoodyPercent.Text = "0%";
            BlueProgress.Progress = 0; BluePercent.Text = "0%";
            BloatingProgress.Progress = 0; BloatingPercent.Text = "0%";
            CravingProgress.Progress = 0; CravingPercent.Text = "0%";
            HeatProgress.Progress = 0; HeatPercent.Text = "0%";
            NauseaProgress.Progress = 0; NauseaPercent.Text = "0%";
            RestlessProgress.Progress = 0; RestlessPercent.Text = "0%";
            OilyProgress.Progress = 0; OilyPercent.Text = "0%";
            FatigueProgress.Progress = 0; FatiguePercent.Text = "0%";
            SorenessProgress.Progress = 0; SorenessPercent.Text = "0%";
            RushProgress.Progress = 0; RushPercent.Text = "0%";
            AnxietyProgress.Progress = 0; AnxietyPercent.Text = "0%";
            IrritateProgress.Progress = 0; IrritatePercent.Text = "0%";
            SickProgress.Progress = 0; SickPercent.Text = "0%";
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
