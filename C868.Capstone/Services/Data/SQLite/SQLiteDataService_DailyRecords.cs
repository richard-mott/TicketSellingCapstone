using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using C868.Capstone.Core.Models.Activities;

namespace C868.Capstone.Services.Data.SQLite
{
    public partial class SQLiteDataService
    {
        public async Task<DailyRecord> GetDailyRecordAsync(int dailyRecordId)
        {
            return await dbContext.FindAsync<DailyRecord>(dailyRecordId);
        }
        
        public async Task<List<DailyRecord>> GetDailyRecordsAsync()
        {
            return await dbContext.Table<DailyRecord>()
                .ToListAsync();
        }

        public async Task<List<DailyRecord>> GetDailyRecordsAsync(DateTime date)
        {
            var startDate = date.Date;
            var endDate = date.AddDays(1).Date;

            return await dbContext.Table<DailyRecord>()
                .Where(dailyRecord => dailyRecord.OpenDate >= startDate &&
                                      dailyRecord.OpenDate < endDate)
                .ToListAsync();
        }

        public async Task<List<DailyRecord>> GetDailyRecordsAsync(DateTime startDate, DateTime endDate)
        {
            return await dbContext.Table<DailyRecord>()
                .Where(dailyRecord => dailyRecord.OpenDate >= startDate &&
                                      dailyRecord.OpenDate < endDate)
                .ToListAsync();
        }

        public async Task<bool> SaveDailyRecordAsync(DailyRecord dailyRecord)
        {
            var foundDailyRecord = await GetDailyRecordAsync(dailyRecord.DailyRecordId);

            return foundDailyRecord is null
                ? await dbContext.InsertAsync(dailyRecord) == 1
                : await dbContext.UpdateAsync(dailyRecord) == 1;
        }

        public async Task<bool> HasDailyRecordsAsync()
        {
            return await dbContext.Table<DailyRecord>().CountAsync() > 0;
        }
    }
}