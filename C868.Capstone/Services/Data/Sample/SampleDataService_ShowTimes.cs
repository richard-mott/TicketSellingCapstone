using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using C868.Capstone.Core.Models.Data;

namespace C868.Capstone.Services.Data.Sample
{
    public partial class SampleDataService
    {
        private List<ShowTime> showTimes;

        public async Task<ShowTime> GetShowTimeAsync(int showTimeId)
        {
            return await Task.FromResult(
                showTimes.FirstOrDefault(
                    showTime => showTime.ShowTimeId == showTimeId));
        }

        public async Task<List<ShowTime>> GetShowTimesAsync()
        {
            return await Task.FromResult(new List<ShowTime>(showTimes));
        }

        public async Task<List<ShowTime>> GetShowTimesAsync(DateTime date)
        {
            return await Task.FromResult(
                showTimes
                    .Where(showTime => showTime.StartTime.Date == date.Date)
                    .ToList());
        }

        public async Task<List<ShowTime>> GetShowTimesAsync(DateTime startDate, DateTime endDate)
        {
            return await Task.FromResult(
                showTimes
                    .Where(showTime => showTime.StartTime.Date >= startDate &&
                                       showTime.StartTime.Date < endDate)
                    .ToList());
        }

        public async Task<List<ShowTime>> GetShowTimesAsync(Movie movie)
        {
            return await Task.FromResult(
                showTimes
                    .Where(showTime => showTime.MovieId == movie.MovieId)
                    .ToList());
        }

        public async Task<bool> SaveShowTimeAsync(ShowTime showTime)
        {
            return await Task.FromResult(
                showTime.ShowTimeId == 0
                    ? await InsertShowTimeAsync(showTime)
                    : await UpdateShowTimeAsync(showTime));
        }

        public async Task<bool> DeleteShowTimeAsync(ShowTime showTime)
        {
            var foundShowTime = showTimes.FirstOrDefault(
                found => found.ShowTimeId == showTime.ShowTimeId);

            return await Task.FromResult(showTimes.Remove(foundShowTime));
        }

        public async Task<bool> HasShowTimesAsync()
        {
            return await Task.FromResult(showTimes.Count > 0);
        }

        private async Task<bool> InsertShowTimeAsync(ShowTime newShowTime)
        {
            return await Task.Run(() =>
            {
                var lastShowingId = showTimes
                    .Select(s => s.ShowTimeId)
                    .DefaultIfEmpty()
                    .Max();

                newShowTime.ShowTimeId = lastShowingId + 1;
                showTimes.Add(newShowTime);
                return true;
            });
        }

        private async Task<bool> UpdateShowTimeAsync(ShowTime newShowTime)
        {
            return await Task.Run(() =>
            {
                var oldShowTime = showTimes
                    .FirstOrDefault(s => s.ShowTimeId == newShowTime.ShowTimeId);

                showTimes.Add(newShowTime);

                return showTimes.Remove(oldShowTime);
            });
        }
    }
}
