using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using C868.Capstone.Core.Messages;
using C868.Capstone.Core.Models.Data;
using C868.Capstone.Core.ViewModels.Controls;
using C868.Capstone.Core.ViewModels.Data;
using C868.Capstone.Core.ViewModels.Dialogs;
using C868.Capstone.Services;
using C868.Capstone.Services.Data;
using C868.Capstone.Services.Logging;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace C868.Capstone.Core.ViewModels.Content.ShowTimes
{
    public class ShowTimesViewModel : ContentViewModelBase, IDataLoader
    {
        private IProgress<string> descriptionProgress;

        private readonly IServiceProvider services = App.Current.Services;

        private ScheduleViewerViewModel scheduleViewer;
        public ScheduleViewerViewModel ScheduleViewer
        {
            get => scheduleViewer;
            set => SetProperty(ref scheduleViewer, value);
        }

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
                    SelectedShowTime = new ShowTimeViewModel(
                        new ShowTime
                        {
                            MovieId = SelectedMovie?.Id ?? 0,
                            Movie = SelectedMovie?.Movie,
                            StartTime = ScheduleViewer.StartTime
                        });

                    SelectedAuditorium = null;
                    Messenger.Send(new SelectedMovieChangedMessage(selectedMovie));

                    ((IRelayCommand)SaveCommand).NotifyCanExecuteChanged();
                    ((IRelayCommand)DeleteCommand).NotifyCanExecuteChanged();
                }
            }
        }

        private List<AuditoriumViewModel> auditoriums;
        public List<AuditoriumViewModel> Auditoriums
        {
            get => auditoriums;
            set => SetProperty(ref auditoriums, value);
        }

        private AuditoriumViewModel selectedAuditorium;
        public AuditoriumViewModel SelectedAuditorium
        {
            get => selectedAuditorium;
            set => SetProperty(ref selectedAuditorium, value);
        }

        private ShowTimeViewModel selectedShowTime;
        public ShowTimeViewModel SelectedShowTime
        {
            get => selectedShowTime;
            set
            {
                if (SetProperty(ref selectedShowTime, value))
                {
                    if (selectedShowTime is null)
                    {
                        return;
                    }

                    selectedMovie = Movies
                        .FirstOrDefault(movie => movie.Id == selectedShowTime.Movie.Id);
                    OnPropertyChanged(nameof(SelectedMovie));
                    ((IRelayCommand)SaveCommand).NotifyCanExecuteChanged();
                    ((IRelayCommand)DeleteCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public bool IsIndeterminate => true;
        public bool ShowPercentage => false;

        private bool showProgress;
        public bool ShowProgress
        {
            get => showProgress;
            set => SetProperty(ref showProgress, value);
        }

        private int progressValue;
        public int ProgressValue
        {
            get => progressValue;
            set => SetProperty(ref progressValue, value);
        }

        private string progressDescription;
        public string ProgressDescription
        {
            get => progressDescription;
            set => SetProperty(ref progressDescription, value);
        }

        private string auditoriumError;
        public string AuditoriumError
        {
            get => auditoriumError;
            set => SetProperty(ref auditoriumError, value);
        }

        private string startTimeError;
        public string StartTimeError
        {
            get => startTimeError;
            set => SetProperty(ref startTimeError, value);
        }

        public ICommand SaveCommand { get; private set; }
        public ICommand ClearCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }

        public ShowTimesViewModel(IDataService dataService, IDialogService dialogService,
            ILoggingService loggingService)
            : base(dataService, dialogService, loggingService)
        {
            IsActive = true;
            InitializeProgressReporting();
            InitializeCommands();
            
            ScheduleViewer = services.GetService<ScheduleViewerViewModel>();
        }

        public async Task InitializeData()
        {
            Movies = new List<MovieViewModel>();
            SelectedMovie = null;

            Auditoriums = new List<AuditoriumViewModel>();
            SelectedAuditorium = null;

            SelectedShowTime = null;

            try
            {
                ShowProgress = true;

                descriptionProgress.Report("Loading movies...");
                await InitializeMovies();

                descriptionProgress.Report("Loading auditoriums...");
                await InitializeAuditoriums();

                descriptionProgress.Report("Loading show times...");
                await ScheduleViewer.InitializeData();
            }
            catch (Exception exception)
            {
                HandleError(
                    AppSettings.Errors.Data.LoadTitle,
                    string.IsNullOrWhiteSpace(exception.Message)
                        ? AppSettings.Errors.Data.LoadMessage
                        : exception.Message);
            }
            finally
            {
                ShowProgress = false;
            }
        }

        private async Task InitializeMovies()
        {
            Movies = (await DataService.GetMoviesAsync())
                .OrderBy(movie => movie.Name)
                .Select(movie => new MovieViewModel(movie))
                .ToList();

            SelectedMovie = null;
        }

        private async Task InitializeAuditoriums()
        {
            Auditoriums = (await DataService.GetAuditoriumsAsync())
                .OrderBy(auditorium => auditorium.Name)
                .Select(auditorium => new AuditoriumViewModel(auditorium))
                .ToList();

            SelectedAuditorium = null;
        }

        private void InitializeProgressReporting()
        {
            descriptionProgress = new Progress<string>(
                description => ProgressDescription = description);
        }

        private void InitializeCommands()
        {
            SaveCommand = new RelayCommand(ExecuteSaveCommand, () => SelectedMovie != null);
            ClearCommand = new RelayCommand(ExecuteClearCommand);
            DeleteCommand = new RelayCommand(
                ExecuteDeleteCommand,
                () => SelectedShowTime != null &&
                      SelectedShowTime.Id != 0);
        }

        private async void ExecuteSaveCommand()
        {
            try
            {
                await SaveShowTime();
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

        private void ExecuteDeleteCommand()
        {
            try
            {
                var confirmViewModel = new ConfirmDialogViewModel(
                    @"Delete Show Time",
                    $"Are you sure you want to delete \"{SelectedShowTime.Movie.Name} ({SelectedShowTime.StartTime:h:mm tt})?\"");

                DialogService.ShowDialog(confirmViewModel, async result =>
                {
                    if (result == true)
                    {
                        await DeleteShowTime();
                    }
                });
            }
            catch (Exception exception)
            {
                HandleError(
                    AppSettings.Errors.Data.DeleteTitle,
                    string.IsNullOrWhiteSpace(exception.Message)
                        ? AppSettings.Errors.Data.DeleteMessage
                        : exception.Message);
            }
        }

        private void ExecuteClearCommand()
        {
            SelectedMovie = null;
            SelectedShowTime = null;
            SelectedAuditorium = null;
            Messenger.Send(new SelectedMovieChangedMessage(selectedMovie));

            ClearErrors();
        }

        private async Task SaveShowTime()
        {
            if (!IsShowTimeValid())
            {
                return;
            }

            if (await DataService.SaveShowTimeAsync(SelectedShowTime.ShowTime))
            {
                var recordName = $"{SelectedShowTime.Movie.Name}, " +
                                 $"{SelectedShowTime.StartTime:t}, " +
                                 SelectedShowTime.Auditorium.Name;

                LogSave(recordName, @"Show Time");
                
                Messenger.Send(new ShowTimeSavedMessage(SelectedShowTime));
                ExecuteClearCommand();
            }
            else
            {
                throw new InvalidOperationException(AppSettings.Errors.Data.SaveMessage);
            }
        }

        private async Task DeleteShowTime()
        {
            if (await DataService.DeleteShowTimeAsync(SelectedShowTime.ShowTime))
            {
                var recordName = $"{SelectedShowTime.Movie.Name}, " +
                                 $"{SelectedShowTime.StartTime:t}, " +
                                 SelectedShowTime.Auditorium.Name;

                LogDelete(recordName, @"Show Time");

                Messenger.Send(new ShowTimeDeletedMessage(selectedShowTime));
                ExecuteClearCommand();
            }
            else
            {
                throw new InvalidOperationException(AppSettings.Errors.Data.DeleteMessage);
            }
        }

        private void ClearErrors()
        {
            AuditoriumError = string.Empty;
            StartTimeError = string.Empty;
        }

        private bool IsShowTimeValid()
        {
            ClearErrors();

            if (SelectedAuditorium is null)
            {
                AuditoriumError = AppSettings.ValidationErrors.ShowTime.AuditoriumBlank;
                return false;
            }

            SelectedShowTime.Auditorium = SelectedAuditorium;

            return !IsShowOverlapping();
        }

        private bool IsShowOverlapping()
        {
            var currentShows = ScheduleViewer.AuditoriumShowTimes[SelectedAuditorium.Id];
            var overlappingShows = currentShows
                .Where(showTime => IsTimeOverlapping(SelectedShowTime, showTime))
                .Select(showTime => $"{showTime.Movie.Name} ({showTime.StartTime:h:mm tt})")
                .DefaultIfEmpty(string.Empty)
                .Aggregate((current, next) => $"{current}\n{next}");

            if (string.IsNullOrEmpty(overlappingShows))
            {
                return false;
            }

            var errorMessage = AppSettings.Dialogs.OverlappingShowMessage +
                               (overlappingShows.Contains("\n") ? "s:" : ":") +
                               $"\n\n{overlappingShows}";
            
            var errorViewModel = new ErrorDialogViewModel(
                AppSettings.Dialogs.OverlappingShowTitle,
                errorMessage,
                ErrorType.Error);

            DialogService.ShowErrorDialog(errorViewModel);

            return true;
        }

        private bool IsTimeOverlapping(ShowTimeViewModel selectedShow,
            ShowTimeViewModel compareShow)
        {
            if (selectedShow.Id == compareShow.Id)
            {
                return false;
            }

            return IsStartTimeOverlapping(selectedShow, compareShow) ||
                   IsEndTimeOverlapping(selectedShow, compareShow);
        }

        private bool IsStartTimeOverlapping(ShowTimeViewModel selectedTime,
            ShowTimeViewModel compareTime)
        {
            var selectedStart = selectedTime.StartTime;
            var compareStart = compareTime.StartTime;
            var compareEnd = compareTime.EndTime;

            return compareStart < selectedStart && compareEnd >= selectedStart;
        }

        private bool IsEndTimeOverlapping(ShowTimeViewModel selectedTime,
            ShowTimeViewModel compareTime)
        {
            var selectedEnd = selectedTime.EndTime;
            var compareStart = compareTime.StartTime;
            var compareEnd = compareTime.EndTime;

            return compareEnd > selectedEnd && compareStart <= selectedEnd;
        }

        protected override void OnActivated()
        {
            Messenger.Register<ShowTimesViewModel, SelectedShowTimeChangedMessage>(this,
                (receiver, message) => receiver.Receive(message));
        }

        private void Receive(SelectedShowTimeChangedMessage message)
        {
            if (message.Value is null)
            {
                return;
            }

            SelectedShowTime = message.Value;
            SelectedAuditorium = Auditoriums.FirstOrDefault(
                auditorium => auditorium.Id == SelectedShowTime.Auditorium.Id);
        }
    }
}