using System.Collections.Generic;
using System.Threading.Tasks;
using C868.Capstone.Core.Models.Data;

namespace C868.Capstone.Services.Data
{
    public partial interface IDataService
    {
        Task<Movie> GetMovieAsync(int movieId);
        Task<List<Movie>> GetMoviesAsync();
        Task<bool> SaveMovieAsync(Movie movie);
        Task<bool> DeleteMovieAsync(Movie movie);
        Task<bool> HasMoviesAsync();
    }
}
