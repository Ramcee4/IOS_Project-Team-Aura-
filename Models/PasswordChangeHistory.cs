using SQLite;

namespace Team_Aura_Period_Tracker_;

public class PasswordChangeHistory
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public string UserEmail { get; set; } = string.Empty;

    public string NewPassword { get; set; } = string.Empty;

    public string ConfirmPassword { get; set; } = string.Empty;

    public string ChangedAt { get; set; } = string.Empty;
}