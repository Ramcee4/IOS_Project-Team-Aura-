using SQLite;

namespace Team_Aura_Period_Tracker_;

public class HealthJournal
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Date { get; set; } = string.Empty;

    public string Mood { get; set; } = string.Empty;

    public string Energy { get; set; } = string.Empty;

    public string Sleep { get; set; } = string.Empty;

    public string Exercise { get; set; } = string.Empty;

    public string Medication { get; set; } = string.Empty;

    public string Notes { get; set; } = string.Empty;
}