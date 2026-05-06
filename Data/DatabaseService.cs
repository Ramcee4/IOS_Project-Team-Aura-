using SQLite;
using Microsoft.Maui.Storage;

namespace Team_Aura_Period_Tracker_;

public class DatabaseService
{
    private readonly SQLiteAsyncConnection _database;

    public DatabaseService()
    {
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "team_aura.db");
        _database = new SQLiteAsyncConnection(dbPath);

        _database.CreateTableAsync<User>().Wait();
        _database.CreateTableAsync<UserPreferences>().Wait();
        _database.CreateTableAsync<UserCycleInfo>().Wait();
        _database.CreateTableAsync<DailyLog>().Wait();
        _database.CreateTableAsync<HealthJournal>().Wait();
        _database.CreateTableAsync<PasswordChangeHistory>().Wait();
    }

    public Task<int> AddUserAsync(User user)
    {
        return _database.InsertAsync(user);
    }

    public Task<User?> GetUserByEmailAsync(string email)
    {
        return _database.Table<User>()
            .Where(u => u.Email == email)
            .FirstOrDefaultAsync();
    }

    public Task<User?> GetUserByIdAsync(int userId)
    {
        return _database.Table<User>()
            .Where(u => u.Id == userId)
            .FirstOrDefaultAsync();
    }

    public Task<int> UpdateUserAsync(User user)
    {
        return _database.UpdateAsync(user);
    }

    public async Task<bool> ChangePasswordByEmailAsync(
        string email,
        string newPassword,
        string confirmPassword)
    {
        var user = await GetUserByEmailAsync(email);

        if (user == null)
            return false;

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

    public Task<List<PasswordChangeHistory>> GetPasswordChangeHistoryAsync(int userId)
    {
        return _database.Table<PasswordChangeHistory>()
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.ChangedAt)
            .ToListAsync();
    }

    public async Task SaveUserPreferencesAsync(int userId, string username, string selectedOptions)
    {
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

    public Task<UserPreferences?> GetUserPreferencesAsync(int userId)
    {
        return _database.Table<UserPreferences>()
            .Where(p => p.UserId == userId)
            .FirstOrDefaultAsync();
    }

    public async Task SaveUserCycleInfoAsync(
        int userId,
        string username,
        string lastDateOfPeriod,
        int cycleLengthDays,
        int periodDays,
        string cycleType)
    {
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

    public Task<UserCycleInfo?> GetUserCycleInfoAsync(int userId)
    {
        return _database.Table<UserCycleInfo>()
            .Where(c => c.UserId == userId)
            .FirstOrDefaultAsync();
    }

    public async Task SaveUserAgeRangeAsync(int userId, string username, string ageRange)
    {
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

    public async Task SaveDailyLogAsync(DailyLog log)
    {
        await _database.InsertAsync(log);
    }

    public Task<List<DailyLog>> GetDailyLogsByUserAsync(int userId)
    {
        return _database.Table<DailyLog>()
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.Date)
            .ToListAsync();
    }

    public Task<DailyLog?> GetDailyLogByDateAsync(int userId, string date)
    {
        return _database.Table<DailyLog>()
            .Where(l => l.UserId == userId && l.Date == date)
            .FirstOrDefaultAsync();
    }

    public async Task SaveOrUpdateDailyLogAsync(DailyLog log)
    {
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

    public async Task SaveHealthJournalAsync(HealthJournal journal)
    {
        await _database.InsertAsync(journal);
    }

    public Task<List<HealthJournal>> GetHealthJournalsByUserAsync(int userId)
    {
        return _database.Table<HealthJournal>()
            .Where(j => j.UserId == userId)
            .OrderByDescending(j => j.Date)
            .ToListAsync();
    }

    public Task<HealthJournal?> GetHealthJournalByIdAsync(int id)
    {
        return _database.Table<HealthJournal>()
            .Where(j => j.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task UpdateHealthJournalAsync(HealthJournal journal)
    {
        await _database.UpdateAsync(journal);
    }

    public async Task DeleteHealthJournalAsync(HealthJournal journal)
    {
        await _database.DeleteAsync(journal);
    }

    public async Task DeleteDailyLogAsync(DailyLog log)
    {
        await _database.DeleteAsync(log);
    }

    public async Task UpdateLastPeriodDateAsync(int userId, string newLastPeriodDate)
    {
        var existingCycleInfo = await _database.Table<UserCycleInfo>()
            .Where(c => c.UserId == userId)
            .FirstOrDefaultAsync();

        if (existingCycleInfo != null)
        {
            existingCycleInfo.LastDateOfPeriod = newLastPeriodDate;
            await _database.UpdateAsync(existingCycleInfo);
        }
    }
}