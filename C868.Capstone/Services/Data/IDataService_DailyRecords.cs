using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using C868.Capstone.Core.Models.Activities;

namespace C868.Capstone.Services.Data
{
    public partial interface IDataService
    {
        Task<DailyRecord> GetDailyRecordAsync(int dailyRecordId);
        Task<List<DailyRecord>> GetDailyRecordsAsync();
        Task<List<DailyRecord>> GetDailyRecordsAsync(DateTime date);
        Task<List<DailyRecord>> GetDailyRecordsAsync(DateTime startDate, DateTime endDate);
        Task<bool> SaveDailyRecordAsync(DailyRecord dailyRecord);
        Task<bool> HasDailyRecordsAsync();
    }
}