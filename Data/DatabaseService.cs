using SQLite;

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
}