using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using C868.Capstone.Core.Models.Activities;

namespace C868.Capstone.Services.Data
{
    public partial interface IDataService
    {
        Task<LogEntry> GetLogEntryAsync(int logEntryId);
        Task<List<LogEntry>> GetLogEntriesAsync();
        Task<List<LogEntry>> GetLogEntriesAsync(DateTime date);
        Task<List<LogEntry>> GetLogEntriesAsync(DateTime startDate, DateTime endDate);
        Task<bool> SaveLogEntryAsync(LogEntry logEntry);
        Task<bool> HasLogEntriesAsync();
    }
}