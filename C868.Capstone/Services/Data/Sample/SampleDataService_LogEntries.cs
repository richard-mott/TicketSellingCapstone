using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using C868.Capstone.Core.Models.Activities;

namespace C868.Capstone.Services.Data.Sample
{
    public partial class SampleDataService
    {
        private List<LogEntry> logEntries;

        public async Task<LogEntry> GetLogEntryAsync(int logEntryId)
        {
            return await Task.FromResult(
                logEntries.FirstOrDefault(
                    logEntry => logEntry.LogEntryId == logEntryId));
        }

        public async Task<List<LogEntry>> GetLogEntriesAsync()
        {
            return await Task.FromResult(new List<LogEntry>(logEntries));
        }

        public async Task<List<LogEntry>> GetLogEntriesAsync(DateTime date)
        {
            return await Task.FromResult(
                logEntries
                    .Where(logEntry => logEntry.Created.Date == date.Date)
                    .ToList());
        }

        public async Task<List<LogEntry>> GetLogEntriesAsync(DateTime startDate, DateTime endDate)
        {
            return await Task.FromResult(
                logEntries
                    .Where(logEntry => logEntry.Created.Date >= startDate &&
                                       logEntry.Created.Date < endDate)
                    .ToList());
        }

        public async Task<bool> SaveLogEntryAsync(LogEntry logEntry)
        {
            return await Task.FromResult(logEntry.LogEntryId == 0
                ? await InsertLogEntryAsync(logEntry)
                : await UpdateLogEntryAsync(logEntry));
        }

        public async Task<bool> HasLogEntriesAsync()
        {
            return await Task.FromResult(logEntries.Count > 0);
        }

        private async Task<bool> InsertLogEntryAsync(LogEntry newLogEntry)
        {
            return await Task.Run(() =>
            {
                var lastLogEntryIndex = logEntries
                    .Select(logEntry => logEntry.LogEntryId)
                    .DefaultIfEmpty()
                    .Max();

                newLogEntry.LogEntryId = lastLogEntryIndex + 1;
                logEntries.Add(newLogEntry);

                return true;
            });
        }

        private async Task<bool> UpdateLogEntryAsync(LogEntry newLogEntry)
        {
            return await Task.Run(() =>
            {
                var oldLogEntry = logEntries.FirstOrDefault(
                    logEntry => logEntry.LogEntryId == newLogEntry.LogEntryId);

                logEntries.Add(newLogEntry);

                return logEntries.Remove(oldLogEntry);
            });
        }
    }
}