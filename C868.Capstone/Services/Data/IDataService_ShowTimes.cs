using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using C868.Capstone.Core.Models.Data;

namespace C868.Capstone.Services.Data
{
    public partial interface IDataService
    {
        Task<ShowTime> GetShowTimeAsync(int showTimeId);
        Task<List<ShowTime>> GetShowTimesAsync();
        Task<List<ShowTime>> GetShowTimesAsync(DateTime date);
        Task<List<ShowTime>> GetShowTimesAsync(DateTime startDate, DateTime endDate);
        Task<List<ShowTime>> GetShowTimesAsync(Movie movie);
        Task<bool> SaveShowTimeAsync(ShowTime showTime);
        Task<bool> DeleteShowTimeAsync(ShowTime showTime);
        Task<bool> HasShowTimesAsync();
    }
}
