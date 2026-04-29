using SQLite;

namespace Team_Aura_Period_Tracker_;

public class User
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    [Unique]
    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}