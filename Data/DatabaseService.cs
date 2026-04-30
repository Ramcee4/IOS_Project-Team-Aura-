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
}