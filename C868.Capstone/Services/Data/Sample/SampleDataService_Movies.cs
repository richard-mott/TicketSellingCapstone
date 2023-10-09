using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using C868.Capstone.Core.Models.Data;

namespace C868.Capstone.Services.Data.Sample
{
    public partial class SampleDataService
    {
        private List<Movie> movies;

        public async Task<Movie> GetMovieAsync(int movieId)
        {
            return await Task.FromResult(
                movies.FirstOrDefault(
                    movie => movie.MovieId == movieId));
        }

        public async Task<List<Movie>> GetMoviesAsync()
        {
            return await Task.FromResult(new List<Movie>(movies));
        }

        public async Task<bool> SaveMovieAsync(Movie movie)
        {
            return await Task.FromResult(
                movie.MovieId == 0
                    ? await InsertMovieAsync(movie)
                    : await UpdateMovieAsync(movie));
        }

        public async Task<bool> DeleteMovieAsync(Movie movie)
        {
            var foundMovie = movies.FirstOrDefault(
                found => found.MovieId == movie.MovieId);

            return await Task.FromResult(movies.Remove(foundMovie));
            
        }

        public async Task<bool> HasMoviesAsync()
        {
            return await Task.FromResult(movies.Count > 0);
        }

        private async Task<bool> InsertMovieAsync(Movie newMovie)
        {
            return await Task.Run(() =>
            {
                var lastMovieId = movies
                    .Select(m => m.MovieId)
                    .DefaultIfEmpty()
                    .Max();

                newMovie.MovieId = lastMovieId + 1;
                movies.Add(newMovie);

                return true;
            });
        }

        private async Task<bool> UpdateMovieAsync(Movie newMovie)
        {
            return await Task.Run(() =>
            {
                var oldMovie = movies
                    .FirstOrDefault(m => m.MovieId == newMovie.MovieId);

                movies.Add(newMovie);

                return movies.Remove(oldMovie);
            });
        }
    }
}
