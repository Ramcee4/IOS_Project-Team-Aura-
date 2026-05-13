using SQLite;
using Microsoft.Maui.Storage;
using System.Text.Json;
using System.Diagnostics;

namespace Team_Aura_Period_Tracker_;

public class DatabaseService
{
    private SQLiteAsyncConnection _database;

    public DatabaseService()
    {
        // I-set up lang ang connection diri, ayaw paghimo og tables diri
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "team_aura.db");
        _database = new SQLiteAsyncConnection(dbPath);
    }

    // Kini nga method maoy mo-siguro nga initialized ang tables sa dili pa gamiton
    private async Task Init()
    {
        try
        {
            // Kini mo-create ra sa tables kon wala pa sila mag-exist
            await _database.CreateTableAsync<User>();
            await _database.CreateTableAsync<UserPreferences>();
            await _database.CreateTableAsync<UserCycleInfo>();
            await _database.CreateTableAsync<DailyLog>();
            await _database.CreateTableAsync<HealthJournal>();
            await _database.CreateTableAsync<PasswordChangeHistory>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Database Init Error: {ex.Message}");
        }
    }

    // --- USER METHODS ---

    public async Task<int> AddUserAsync(User user)
    {
        await Init();
        return await _database.InsertAsync(user);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        await Init();
        return await _database.Table<User>()
            .Where(u => u.Email == email)
            .FirstOrDefaultAsync();
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        await Init();
        return await _database.Table<User>()
            .Where(u => u.Id == userId)
            .FirstOrDefaultAsync();
    }

    public async Task<int> UpdateUserAsync(User user)
    {
        await Init();
        return await _database.UpdateAsync(user);
    }

    public async Task<bool> ChangePasswordByEmailAsync(string email, string newPassword, string confirmPassword)
    {
        await Init();
        var user = await GetUserByEmailAsync(email);

        if (user == null) return false;

        user.Password = newPassword;
        await _database.UpdateAsync(user);

        var passwordHistory = new PasswordChangeHistory
        {
            UserId = user.Id,
            Username = user.Name,
            UserEmail = user.Email,
            NewPassword = newPassword,
            ConfirmPassword = confirmPassword,
            ChangedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };

        await _database.InsertAsync(passwordHistory);

        Preferences.Set("UserId", user.Id);
        Preferences.Set("UserName", user.Name);
        Preferences.Set("UserEmail", user.Email);
        Preferences.Set("UserPassword", newPassword);

        return true;
    }

    public async Task<List<PasswordChangeHistory>> GetPasswordChangeHistoryAsync(int userId)
    {
        await Init();
        return await _database.Table<PasswordChangeHistory>()
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.ChangedAt)
            .ToListAsync();
    }

    // --- PREFERENCES & CYCLE INFO ---

    public async Task SaveUserPreferencesAsync(int userId, string username, string selectedOptions)
    {
        await Init();
        var existingPreference = await _database.Table<UserPreferences>()
            .Where(p => p.UserId == userId)
            .FirstOrDefaultAsync();

        if (existingPreference != null)
        {
            existingPreference.Username = username;
            existingPreference.WhatBringsYouHere = selectedOptions;
            await _database.UpdateAsync(existingPreference);
        }
        else
        {
            var preference = new UserPreferences
            {
                UserId = userId,
                Username = username,
                WhatBringsYouHere = selectedOptions
            };
            await _database.InsertAsync(preference);
        }
    }

    public async Task<UserPreferences?> GetUserPreferencesAsync(int userId)
    {
        await Init();
        return await _database.Table<UserPreferences>()
            .Where(p => p.UserId == userId)
            .FirstOrDefaultAsync();
    }

    public async Task SaveUserCycleInfoAsync(int userId, string username, string lastDateOfPeriod, int cycleLengthDays, int periodDays, string cycleType)
    {
        await Init();
        var existingCycleInfo = await _database.Table<UserCycleInfo>()
            .Where(c => c.UserId == userId)
            .FirstOrDefaultAsync();

        if (existingCycleInfo != null)
        {
            existingCycleInfo.Username = username;
            existingCycleInfo.LastDateOfPeriod = lastDateOfPeriod;
            existingCycleInfo.CycleLengthDays = cycleLengthDays;
            existingCycleInfo.PeriodDays = periodDays;
            existingCycleInfo.CycleType = cycleType;
            await _database.UpdateAsync(existingCycleInfo);
        }
        else
        {
            var cycleInfo = new UserCycleInfo
            {
                UserId = userId,
                Username = username,
                LastDateOfPeriod = lastDateOfPeriod,
                CycleLengthDays = cycleLengthDays,
                PeriodDays = periodDays,
                CycleType = cycleType
            };
            await _database.InsertAsync(cycleInfo);
        }
    }

    public async Task<UserCycleInfo?> GetUserCycleInfoAsync(int userId)
    {
        await Init();
        return await _database.Table<UserCycleInfo>()
            .Where(c => c.UserId == userId)
            .FirstOrDefaultAsync();
    }

    public async Task SaveUserAgeRangeAsync(int userId, string username, string ageRange)
    {
        await Init();
        var existingCycleInfo = await _database.Table<UserCycleInfo>()
            .Where(c => c.UserId == userId)
            .FirstOrDefaultAsync();

        if (existingCycleInfo != null)
        {
            existingCycleInfo.Username = username;
            existingCycleInfo.AgeRange = ageRange;
            await _database.UpdateAsync(existingCycleInfo);
        }
        else
        {
            var cycleInfo = new UserCycleInfo
            {
                UserId = userId,
                Username = username,
                AgeRange = ageRange
            };
            await _database.InsertAsync(cycleInfo);
        }
    }

    // --- DAILY LOG METHODS ---

    public async Task SaveDailyLogAsync(DailyLog log)
    {
        await Init();
        await _database.InsertAsync(log);
    }

    public async Task<List<DailyLog>> GetDailyLogsByUserAsync(int userId)
    {
        await Init();
        return await _database.Table<DailyLog>()
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.Date)
            .ToListAsync();
    }

    public async Task<DailyLog?> GetDailyLogByDateAsync(int userId, string date)
    {
        await Init();
        return await _database.Table<DailyLog>()
            .Where(l => l.UserId == userId && l.Date == date)
            .FirstOrDefaultAsync();
    }

    public async Task SaveOrUpdateDailyLogAsync(DailyLog log)
    {
        await Init();
        var existingLog = await _database.Table<DailyLog>()
            .Where(l => l.UserId == log.UserId && l.Date == log.Date)
            .FirstOrDefaultAsync();

        if (existingLog != null)
        {
            existingLog.Username = log.Username;
            existingLog.Flow = log.Flow;
            existingLog.Mood = log.Mood;
            existingLog.EnergyLevel = log.EnergyLevel;
            existingLog.Symptoms = log.Symptoms;
            existingLog.Notes = log.Notes;
            await _database.UpdateAsync(existingLog);
        }
        else
        {
            await _database.InsertAsync(log);
        }
    }

    public async Task DeleteDailyLogAsync(DailyLog log)
    {
        await Init();
        await _database.DeleteAsync(log);
    }

    // --- HEALTH JOURNAL METHODS ---

    public async Task SaveHealthJournalAsync(HealthJournal journal)
    {
        await Init();
        await _database.InsertAsync(journal);
    }

    public async Task<List<HealthJournal>> GetHealthJournalsByUserAsync(int userId)
    {
        await Init();
        return await _database.Table<HealthJournal>()
            .Where(j => j.UserId == userId)
            .OrderByDescending(j => j.Date)
            .ToListAsync();
    }

    public async Task<HealthJournal?> GetHealthJournalByIdAsync(int id)
    {
        await Init();
        return await _database.Table<HealthJournal>()
            .Where(j => j.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task UpdateHealthJournalAsync(HealthJournal journal)
    {
        await Init();
        await _database.UpdateAsync(journal);
    }

    public async Task DeleteHealthJournalAsync(HealthJournal journal)
    {
        await Init();
        await _database.DeleteAsync(journal);
    }

    // --- UTILITY METHODS ---

    public async Task UpdateLastPeriodDateAsync(int userId, string newLastPeriodDate)
    {
        await Init();
        var existingCycleInfo = await _database.Table<UserCycleInfo>()
            .Where(c => c.UserId == userId)
            .FirstOrDefaultAsync();

        if (existingCycleInfo != null)
        {
            existingCycleInfo.LastDateOfPeriod = newLastPeriodDate;
            await _database.UpdateAsync(existingCycleInfo);
        }
    }

    public async Task<string> ExportAllDataAsJsonAsync(int userId)
    {
        await Init();
        var user = await GetUserByIdAsync(userId);
        var preferences = await GetUserPreferencesAsync(userId);
        var cycleInfo = await GetUserCycleInfoAsync(userId);
        var dailyLogs = await GetDailyLogsByUserAsync(userId);
        var healthJournals = await GetHealthJournalsByUserAsync(userId);
        var passwordHistory = await GetPasswordChangeHistoryAsync(userId);

        var exportData = new
        {
            User = user,
            Preferences = preferences,
            CycleInfo = cycleInfo,
            DailyLogs = dailyLogs,
            HealthJournals = healthJournals,
            PasswordChangeHistory = passwordHistory,
            ExportedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };

        string json = JsonSerializer.Serialize(exportData, new JsonSerializerOptions { WriteIndented = true });
        string fileName = $"team_aura_export_{DateTime.Now:yyyyMMdd_HHmmss}.json";
        string filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

        await File.WriteAllTextAsync(filePath, json);
        return filePath;
    }

    public async Task DeleteAllUserDataAsync(int userId)
    {
        await Init();
        var preferences = await GetUserPreferencesAsync(userId);
        if (preferences != null) await _database.DeleteAsync(preferences);

        var cycleInfo = await GetUserCycleInfoAsync(userId);
        if (cycleInfo != null) await _database.DeleteAsync(cycleInfo);

        var dailyLogs = await GetDailyLogsByUserAsync(userId);
        foreach (var log in dailyLogs) await _database.DeleteAsync(log);

        var journals = await GetHealthJournalsByUserAsync(userId);
        foreach (var journal in journals) await _database.DeleteAsync(journal);

        var passwordHistory = await GetPasswordChangeHistoryAsync(userId);
        foreach (var history in passwordHistory) await _database.DeleteAsync(history);

        var user = await GetUserByIdAsync(userId);
        if (user != null) await _database.DeleteAsync(user);

        Preferences.Clear();
    }
}