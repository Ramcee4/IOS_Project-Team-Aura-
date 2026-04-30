using SQLite;

namespace Team_Aura_Period_Tracker_;

public class UserCycleInfo
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public string LastDateOfPeriod { get; set; } = string.Empty;

    public int CycleLengthDays { get; set; }

    public int PeriodDays { get; set; }

    public string CycleType { get; set; } = string.Empty;

    public string AgeRange { get; set; } = string.Empty;
}