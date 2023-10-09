using System.Collections.Generic;
using System.Threading.Tasks;
using C868.Capstone.Core.Models.Data;

namespace C868.Capstone.Services.Data.SQLite
{
    public partial class SQLiteDataService
    {
        public async Task<Movie> GetMovieAsync(int movieId)
        {
            return await dbContext.FindAsync<Movie>(movieId);
        }

        public async Task<List<Movie>> GetMoviesAsync()
        {
            return await dbContext.Table<Movie>().ToListAsync();
        }

        public async Task<bool> SaveMovieAsync(Movie movie)
        {
            var foundMovie = await GetMovieAsync(movie.MovieId);

            return foundMovie is null
                ? await dbContext.InsertAsync(movie) == 1
                : await dbContext.UpdateAsync(movie) == 1;
        }

        public async Task<bool> DeleteMovieAsync(Movie movie)
        {
            return await dbContext.DeleteAsync(movie) == 1;
        }

        public async Task<bool> HasMoviesAsync()
        {
            return await dbContext.Table<Movie>().CountAsync() > 0;
        }
    }
}
