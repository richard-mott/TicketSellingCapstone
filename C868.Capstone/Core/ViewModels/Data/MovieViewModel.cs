using C868.Capstone.Core.Models.Data;
using CommunityToolkit.Mvvm.ComponentModel;

namespace C868.Capstone.Core.ViewModels.Data
{
    public class MovieViewModel : ObservableObject
    {
        private readonly Movie movie;
        public Movie Movie => movie ?? new Movie();
        public int Id => Movie.MovieId;

        public string Name
        {
            get => Movie.Name;
            set => SetProperty(Movie.Name, value, Movie,
                (mov, name) => mov.Name = name);
        }

        public Rating? Rating
        {
            get => Movie.Rating == Core.Rating.None
                ? (Rating?)null
                : Movie.Rating;
            set => SetProperty(Movie.Rating, value ?? Core.Rating.None, Movie,
                (mov, rating) => mov.Rating = rating);
        }

        public int RunTime
        {
            get => Movie.RunTime;
            set => SetProperty(Movie.RunTime, value, Movie,
                (mov, runTime) => mov.RunTime = runTime);
        }

        public string Cast
        {
            get => Movie.Cast;
            set => SetProperty(Movie.Cast, value, Movie,
                (mov, cast) => mov.Cast = cast);
        }

        public string Description
        {
            get => Movie.Description;
            set => SetProperty(Movie.Description, value, Movie,
                (mov, description) => mov.Description = description);
        }

        public MovieViewModel(Movie movie)
        {
            this.movie = movie;
        }

        public MovieViewModel Clone()
        {
            return new MovieViewModel(Movie.Clone());
        }
    }
}