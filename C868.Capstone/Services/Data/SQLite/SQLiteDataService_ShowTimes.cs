using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using C868.Capstone.Core.Models.Data;
using SQLiteNetExtensionsAsync.Extensions;

namespace C868.Capstone.Services.Data.SQLite
{
    public partial class SQLiteDataService
    {
        public async Task<ShowTime> GetShowTimeAsync(int showTimeId)
        {
            return await dbContext.FindWithChildrenAsync<ShowTime>(showTimeId, recursive:true);
        }

        public async Task<List<ShowTime>> GetShowTimesAsync()
        {
            return await dbContext.GetAllWithChildrenAsync<ShowTime>(recursive: true);
        }

        public async Task<List<ShowTime>> GetShowTimesAsync(DateTime date)
        {
            var start = date.Date;
            var end = date.AddDays(1).Date;

            return await dbContext.GetAllWithChildrenAsync<ShowTime>(
                showTime => showTime.StartTime >= start &&
                            showTime.StartTime < end,
                recursive: true);
        }

        public async Task<List<ShowTime>> GetShowTimesAsync(DateTime startDate, DateTime endDate)
        {
            return await dbContext.GetAllWithChildrenAsync<ShowTime>(
                showTime => showTime.StartTime >= startDate &&
                            showTime.StartTime < endDate,
                recursive: true);
        }

        public async Task<List<ShowTime>> GetShowTimesAsync(Movie movie)
        {
            return await dbContext.GetAllWithChildrenAsync<ShowTime>(
                showTime => showTime.MovieId == movie.MovieId,
                recursive: true);
        }

        public async Task<bool> SaveShowTimeAsync(ShowTime showTime)
        {
            var foundShowTime = await GetShowTimeAsync(showTime.ShowTimeId);

            return foundShowTime is null
                ? await dbContext.InsertAsync(showTime) == 1
                : await dbContext.UpdateAsync(showTime) == 1;
        }

        public async Task<bool> DeleteShowTimeAsync(ShowTime showTime)
        {
            return await dbContext.DeleteAsync(showTime) == 1;
        }

        public async Task<bool> HasShowTimesAsync()
        {
            return await dbContext.Table<ShowTime>().CountAsync() > 0;
        }
    }
}
