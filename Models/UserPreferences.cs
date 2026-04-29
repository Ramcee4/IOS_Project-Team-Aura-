using SQLite;

namespace Team_Aura_Period_Tracker_;

public class UserPreferences
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public string WhatBringsYouHere { get; set; } = string.Empty;
}