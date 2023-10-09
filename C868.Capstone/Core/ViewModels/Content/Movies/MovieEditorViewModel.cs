using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using C868.Capstone.Core.Messages;
using C868.Capstone.Core.Models.Data;
using C868.Capstone.Core.ViewModels.Data;
using C868.Capstone.Services;
using C868.Capstone.Services.Data;
using C868.Capstone.Services.Logging;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace C868.Capstone.Core.ViewModels.Content.Movies
{
    public class MovieEditorViewModel : ContentViewModelBase
    {
        public List<Rating> Ratings => new List<Rating>
        {
            Rating.G, Rating.PG, Rating.PG13, Rating.R, Rating.NC17
        };

        public string Title => CurrentMovie.Id == 0
            ? "Create Movie"
            : "Edit Movie";

        private MovieViewModel currentMovie;
        public MovieViewModel CurrentMovie
        {
            get => currentMovie;
            set
            {
                var newValue = value is null
                    ? new MovieViewModel(new Movie())
                    : value.Clone();

                if (SetProperty(ref currentMovie, newValue))
                {
                    OnPropertyChanged(nameof(Title));
                    ClearErrors();
                }
            }
        }

        private string nameError;
        public string NameError
        {
            get => nameError;
            set => SetProperty(ref nameError, value);
        }

        private string ratingError;
        public string RatingError
        {
            get => ratingError;
            set => SetProperty(ref ratingError, value);
        }

        private string runTimeError;
        public string RunTimeError
        {
            get => runTimeError;
            set => SetProperty(ref runTimeError, value);
        }

        private string castError;
        public string CastError
        {
            get => castError;
            set => SetProperty(ref castError, value);
        }

        private string descriptionError;
        public string DescriptionError
        {
            get => descriptionError;
            set => SetProperty(ref descriptionError, value);
        }

        public ICommand SaveCommand { get; private set; }
        public ICommand ClearCommand { get; private set; }

        public MovieEditorViewModel(IDataService dataService, IDialogService dialogService,
            ILoggingService loggingService)
            : base(dataService, dialogService, loggingService)
        {
            CurrentMovie = new MovieViewModel(new Movie());

            IsActive = true;
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            SaveCommand = new RelayCommand(ExecuteSaveCommand);
            ClearCommand = new RelayCommand(ExecuteClearCommand);
        }

        private async void ExecuteSaveCommand()
        {
            try
            {
                await SaveMovie();
            }
            catch (Exception exception)
            {
                HandleError(
                    AppSettings.Errors.Data.SaveTitle,
                    string.IsNullOrWhiteSpace(exception.Message)
                        ? AppSettings.Errors.Data.SaveMessage
                        : exception.Message);
            }
        }

        private void ExecuteClearCommand()
        {
            CurrentMovie = new MovieViewModel(new Movie());
            ClearErrors();
            Messenger.Send(new ClearSelectedMovieMessage(true));
        }

        private async Task SaveMovie()
        {
            if (!IsMovieValid())
            {
                return;
            }

            if (await DataService.SaveMovieAsync(CurrentMovie.Movie))
            {
                LogSave(CurrentMovie.Name, @"Movie");

                Messenger.Send(new MovieSavedMessage(CurrentMovie));
                CurrentMovie = new MovieViewModel(new Movie());
            }
            else
            {
                throw new InvalidOperationException(AppSettings.Errors.Data.SaveMessage);
            }
        }

        protected override void OnActivated()
        {
            Messenger.Register<MovieEditorViewModel, SelectedMovieChangedMessage>(this,
                (receiver, message) => receiver.Receive(message));
        }

        private void Receive(SelectedMovieChangedMessage message)
        {
            CurrentMovie = message.Value;
        }

        private bool IsMovieValid()
        {
            var isNameValid = IsNameValid(CurrentMovie.Name);
            var isRatingValid = IsRatingValid(CurrentMovie.Rating);
            var isRunTimeValid = IsRunTimeValid(CurrentMovie.RunTime);
            var isCastValid = IsCastValid(CurrentMovie.Cast);
            var isDescriptionValid = IsDescriptionValid(CurrentMovie.Description);

            return isNameValid && isRatingValid && isRunTimeValid &&
                   isCastValid && isDescriptionValid;
        }

        private bool IsNameValid(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                NameError = AppSettings.ValidationErrors.Movie.NameBlank;
                return false;
            }

            NameError = string.Empty;
            return true;
        }

        private bool IsRatingValid(Rating? rating)
        {
            if (rating is null || rating == Rating.None)
            {
                RatingError = AppSettings.ValidationErrors.Movie.RatingBlank;
                return false;
            }

            RatingError = string.Empty;
            return true;
        }

        private bool IsRunTimeValid(int runTime)
        {
            if (runTime < 1)
            {
                RunTimeError = AppSettings.ValidationErrors.Movie.RunTimeLessThanOne;
                return false;
            }

            RunTimeError = string.Empty;
            return true;
        }

        private bool IsCastValid(string cast)
        {
            if (string.IsNullOrWhiteSpace(cast))
            {
                CastError = AppSettings.ValidationErrors.Movie.CastBlank;
                return false;
            }

            CastError = string.Empty;
            return true;
        }

        private bool IsDescriptionValid(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                DescriptionError = AppSettings.ValidationErrors.Movie.DescriptionBlank;
                return false;
            }

            DescriptionError = string.Empty;
            return true;
        }

        private void ClearErrors()
        {
            NameError = string.Empty;
            RatingError = string.Empty;
            RunTimeError = string.Empty;
            CastError = string.Empty;
            DescriptionError = string.Empty;
        }
    }
}