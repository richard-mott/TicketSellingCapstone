using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using C868.Capstone.Core.Messages;
using C868.Capstone.Core.ViewModels.Data;
using C868.Capstone.Core.ViewModels.Dialogs;
using C868.Capstone.Services;
using C868.Capstone.Services.Data;
using C868.Capstone.Services.Logging;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace C868.Capstone.Core.ViewModels.Content.Movies
{
    public class MovieListViewModel : ContentViewModelBase
    {
        private List<MovieViewModel> allMovies;

        private List<MovieViewModel> movies;
        public List<MovieViewModel> Movies
        {
            get => movies;
            set => SetProperty(ref movies, value);
        }

        private MovieViewModel selectedMovie;
        public MovieViewModel SelectedMovie
        {
            get => selectedMovie;
            set
            {
                if (SetProperty(ref selectedMovie, value))
                {
                    ((IRelayCommand)DeleteCommand).NotifyCanExecuteChanged();
                    Messenger.Send(new SelectedMovieChangedMessage(selectedMovie));
                }
            }
        }

        private string searchText;
        public string SearchText
        {
            get => searchText;
            set => SetProperty(ref searchText, value);
        }
        
        public ICommand SearchCommand { get; private set; }
        public ICommand ClearSearchCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }

        public MovieListViewModel(IDataService dataService, IDialogService dialogService,
            ILoggingService loggingService)
            : base(dataService, dialogService, loggingService)
        {
            IsActive = true;
            InitializeCommands();
        }

        public async Task InitializeData()
        {
            allMovies = new List<MovieViewModel>();

            Movies = new List<MovieViewModel>();
            SelectedMovie = null;

            try
            {
                allMovies = (await DataService.GetMoviesAsync())
                    .Select(movie => new MovieViewModel(movie))
                    .ToList();

                Movies = GetFilteredMovies();
            }
            catch (Exception exception)
            {
                HandleError(
                    AppSettings.Errors.Data.LoadTitle,
                    string.IsNullOrWhiteSpace(exception.Message)
                        ? AppSettings.Errors.Data.LoadMessage
                        : exception.Message);
            }
        }

        private void InitializeCommands()
        {
            SearchCommand = new RelayCommand(ExecuteSearchCommand);
            ClearSearchCommand = new RelayCommand(ExecuteClearSearchCommand);
            DeleteCommand = new RelayCommand(
                ExecuteDeleteCommand,
                () => SelectedMovie != null && SelectedMovie.Id != 0);
        }

        private void ExecuteSearchCommand()
        {
            Movies = GetFilteredMovies();
        }

        private void ExecuteClearSearchCommand()
        {
            SearchText = string.Empty;
            Movies = GetFilteredMovies();
        }

        private void ExecuteDeleteCommand()
        {
            var confirmViewModel = new ConfirmDialogViewModel(
                @"Delete Movie?",
                $"Are you sure you want to delete \"{SelectedMovie.Name}?\"");

            DialogService.ShowDialog(confirmViewModel, async result =>
            {
                if (result == true)
                {
                    await DeleteMovie();
                }
            });
        }

        private List<MovieViewModel> GetFilteredMovies()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                return new List<MovieViewModel>(allMovies);
            }

            var fuzzySearch = Regex
                .Replace(SearchText, "\\s+", "")
                .ToUpper();

            return allMovies
                .Where(movie =>
                    Regex.Replace(movie.Name, "\\s+", "")
                        .ToUpper()
                        .Contains(fuzzySearch))
                .OrderBy(movie => movie.Name)
                .ToList();
        }

        private async Task DeleteMovie()
        {
            if (await DataService.DeleteMovieAsync(SelectedMovie.Movie))
            {
                LogDelete(SelectedMovie.Name, @"Movie");

                allMovies.Remove(SelectedMovie);
                Movies = GetFilteredMovies();
            }
            else
            {
                throw new InvalidOperationException(AppSettings.Errors.Data.DeleteMessage);
            }
        }

        protected override void OnActivated()
        {
            Messenger.Register<MovieListViewModel, ClearSelectedMovieMessage>(this,
                (receiver, message) => receiver.Receive(message));
            Messenger.Register<MovieListViewModel, MovieSavedMessage>(this,
                (receiver, message) => receiver.Receive(message));
        }

        private void Receive(MovieSavedMessage message)
        {
            allMovies.Add(message.Value);
            Movies = GetFilteredMovies();
        }

        private void Receive(ClearSelectedMovieMessage message)
        {
            if (message.Value)
            {
                SelectedMovie = null;
            }
        }
    }
}