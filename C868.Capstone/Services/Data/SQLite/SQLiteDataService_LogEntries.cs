using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using C868.Capstone.Core.Models.Activities;
using SQLiteNetExtensionsAsync.Extensions;

namespace C868.Capstone.Services.Data.SQLite
{
    public partial class SQLiteDataService
    {
        public async Task<LogEntry> GetLogEntryAsync(int logEntryId)
        {
            return await dbContext.FindWithChildrenAsync<LogEntry>(logEntryId, recursive: true);
        }

        public async Task<List<LogEntry>> GetLogEntriesAsync()
        {
            return await dbContext.GetAllWithChildrenAsync<LogEntry>(recursive: true);
        }

        public async Task<List<LogEntry>> GetLogEntriesAsync(DateTime date)
        {
            var startDate = date.Date;
            var endDate = date.AddDays(1).Date;

            return await dbContext.GetAllWithChildrenAsync<LogEntry>(
                logEntry => logEntry.Created >= startDate &&
                            logEntry.Created < endDate,
                recursive: true);
        }

        public async Task<List<LogEntry>> GetLogEntriesAsync(DateTime startDate, DateTime endDate)
        {
            return await dbContext.GetAllWithChildrenAsync<LogEntry>(
                logEntry => logEntry.Created >= startDate &&
                            logEntry.Created < endDate,
                recursive: true);
        }

        public async Task<bool> SaveLogEntryAsync(LogEntry logEntry)
        {
            var foundLogEntry = await GetLogEntryAsync(logEntry.LogEntryId);

            return foundLogEntry is null
                ? await dbContext.InsertAsync(logEntry) == 1
                : await dbContext.UpdateAsync(logEntry) == 1;
        }

        public async Task<bool> HasLogEntriesAsync()
        {
            return await dbContext.Table<LogEntry>().CountAsync() > 0;
        }
    }
}