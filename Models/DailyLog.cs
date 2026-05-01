using SQLite;

namespace Team_Aura_Period_Tracker_;

public class DailyLog
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Date { get; set; } = string.Empty;

    public string Flow { get; set; } = string.Empty;

    public string Mood { get; set; } = string.Empty;

    public double EnergyLevel { get; set; }

    public string Symptoms { get; set; } = string.Empty;

    public string Notes { get; set; } = string.Empty;
}