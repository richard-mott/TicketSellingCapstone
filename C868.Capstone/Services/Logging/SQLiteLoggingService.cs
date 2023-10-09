using System;
using System.Threading.Tasks;
using C868.Capstone.Core;
using C868.Capstone.Core.Models.Activities;
using C868.Capstone.Core.Models.Data;
using C868.Capstone.Services.Data;

namespace C868.Capstone.Services.Logging
{
    public class SQLiteLoggingService : ILoggingService
    {
        private readonly IDataService dataService;

        public User User { get; set; }

        public SQLiteLoggingService(IDataService dataService)
        {
            this.dataService = dataService;
        }

        public void LogInfo(string message)
        {
            var logEntry = new LogEntry
            {
                Type = LogMessageType.Info,
                Message = message,
                Created = DateTime.Now,
                UserId = User?.UserId ?? 0,
                User = User
            };

            Task.Run(async () => await dataService.SaveLogEntryAsync(logEntry));
        }

        public void LogWarning(string message)
        {
            var logEntry = new LogEntry
            {
                Type = LogMessageType.Warning,
                Message = message,
                Created = DateTime.Now,
                UserId = User?.UserId ?? 0,
                User = User
            };

            Task.Run(async () => await dataService.SaveLogEntryAsync(logEntry));
        }

        public void LogError(string message)
        {
            var logEntry = new LogEntry
            {
                Type = LogMessageType.Error,
                Message = message,
                Created = DateTime.Now,
                UserId = User?.UserId ?? 0,
                User = User
            };

            Task.Run(async () => await dataService.SaveLogEntryAsync(logEntry));
        }
    }
}