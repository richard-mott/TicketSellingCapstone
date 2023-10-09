using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using C868.Capstone.Core.Models.Activities;

namespace C868.Capstone.Services.Data.Sample
{
    public partial class SampleDataService
    {
        private List<DailyRecord> dailyRecords;

        public async Task<DailyRecord> GetDailyRecordAsync(int dailyRecordId)
        {
            return await Task.FromResult(
                dailyRecords.FirstOrDefault(
                    activity => activity.DailyRecordId == dailyRecordId));
        }

        public async Task<List<DailyRecord>> GetDailyRecordsAsync()
        {
            return await Task.FromResult(new List<DailyRecord>(dailyRecords));
        }

        public async Task<List<DailyRecord>> GetDailyRecordsAsync(DateTime date)
        {
            var startDate = date.Date;
            var endDate = date.AddDays(1).Date;

            return await Task.FromResult(
                dailyRecords
                    .Where(dailyRecord => dailyRecord.OpenDate >= startDate &&
                                          dailyRecord.OpenDate < endDate)
                    .ToList());
        }

        public async Task<List<DailyRecord>> GetDailyRecordsAsync(DateTime startDate, DateTime endDate)
        {
            return await Task.FromResult(
                dailyRecords
                    .Where(dailyRecord => dailyRecord.OpenDate >= startDate &&
                                          dailyRecord.OpenDate < endDate)
                    .ToList());
        }

        public async Task<bool> SaveDailyRecordAsync(DailyRecord dailyRecord)
        {
            return await Task.FromResult(
                dailyRecord.DailyRecordId == 0
                    ? await InsertDailyActivityAsync(dailyRecord)
                    : await UpdateDailyActivityAsync(dailyRecord));
        }

        public async Task<bool> HasDailyRecordsAsync()
        {
            return await Task.FromResult(dailyRecords.Count > 0);
        }

        private async Task<bool> InsertDailyActivityAsync(DailyRecord newDailyRecord)
        {
            return await Task.Run(() =>
            {
                var lastDailyRecordIndex = dailyRecords
                    .Select(dailyRecord => dailyRecord.DailyRecordId)
                    .DefaultIfEmpty()
                    .Max();

                newDailyRecord.DailyRecordId = lastDailyRecordIndex + 1;
                dailyRecords.Add(newDailyRecord);

                return true;
            });
        }

        private async Task<bool> UpdateDailyActivityAsync(DailyRecord newDailyRecord)
        {
            return await Task.Run(() =>
            {
                var oldDailyRecord = dailyRecords.FirstOrDefault(
                    dailyRecord => dailyRecord.DailyRecordId == newDailyRecord.DailyRecordId);

                dailyRecords.Add(newDailyRecord);

                return dailyRecords.Remove(oldDailyRecord);
            });
        }
    }
}