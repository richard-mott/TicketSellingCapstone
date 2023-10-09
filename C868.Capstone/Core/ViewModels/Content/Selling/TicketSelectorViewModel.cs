using System;
using System.Collections.Generic;
using System.Linq;
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

namespace C868.Capstone.Core.ViewModels.Content.Selling
{
    public class TicketSelectorViewModel : ContentViewModelBase
    {
        private List<ShowViewModel> todayShows;
        private List<TicketTypeViewModel> allTicketTypes;

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
                    LoadShowsForMovie(selectedMovie?.Movie);
                }
            }
        }

        private List<ShowViewModel> show;
        public List<ShowViewModel> Shows
        {
            get => show;
            set => SetProperty(ref show, value);
        }

        private ShowViewModel selectedShow;
        public ShowViewModel SelectedShow
        {
            get => selectedShow;
            set
            {
                if (SetProperty(ref selectedShow, value))
                {
                    LoadTicketTypesForShow(selectedShow?.ShowTime);
                }
            }
        }

        private List<TicketTypeViewModel> ticketTypes;
        public List<TicketTypeViewModel> TicketTypes
        {
            get => ticketTypes;
            set => SetProperty(ref ticketTypes, value);
        }

        public ICommand AddTicketCommand { get; private set; }

        public TicketSelectorViewModel(IDataService dataService, IDialogService dialogService,
            ILoggingService loggingService)
            : base(dataService, dialogService, loggingService)
        {
            IsActive = true;
            InitializeCommands();
        }

        public async Task InitializeData()
        {
            todayShows = new List<ShowViewModel>();
            allTicketTypes = new List<TicketTypeViewModel>();

            Movies = new List<MovieViewModel>();
            SelectedMovie = null;

            Shows = new List<ShowViewModel>();
            SelectedShow = null;

            TicketTypes = new List<TicketTypeViewModel>();
            
            try
            {
                await InitializeShows();
                await InitializeTicketTypes();
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
            AddTicketCommand = new RelayCommand<TicketTypeViewModel>(ExecuteAddTicketCommand);
        }

        private async Task InitializeShows()
        {
            var dbShowTimes = await DataService.GetShowTimesAsync(DateTime.Today);
            
            todayShows = dbShowTimes
                .Select(showTime => new ShowViewModel(showTime))
                .ToList();
            
            Movies = dbShowTimes
                .GroupBy(showTime => showTime.MovieId)
                .Select(showTime => new MovieViewModel(showTime.First().Movie))
                .OrderBy(movie => movie.Name)
                .ToList();
        }

        private async Task InitializeTicketTypes()
        {
            allTicketTypes = (await DataService.GetTicketTypesAsync())
                .Select(ticketType => new TicketTypeViewModel(ticketType))
                .ToList();
        }

        private void ExecuteAddTicketCommand(TicketTypeViewModel selectedTicketType)
        {
            Messenger.Send(new TicketAddedMessage(
                new TicketViewModel(
                    new Ticket
                    {
                        Count = 1,
                        TicketTypeId = selectedTicketType.Id,
                        TicketType = selectedTicketType.TicketType,
                        ShowTimeId = SelectedShow.Id,
                        ShowTime = SelectedShow.ShowTime
                    })));
        }

        private void LoadShowsForMovie(Movie movie)
        {
            if (movie is null)
            {
                Shows = new List<ShowViewModel>();
                return;
            }

            Shows = todayShows
                .Where(showTime => showTime.Movie.Id == movie.MovieId)
                .ToList();
        }

        private void LoadTicketTypesForShow(ShowTime showTime)
        {
            if (showTime is null)
            {
                TicketTypes = new List<TicketTypeViewModel>();
                return;
            }

            TicketTypes = allTicketTypes
                .Where(ticketType => ticketType.IsAvailableForShowTime(showTime))
                .ToList();
        }

        protected override void OnActivated()
        {
            Messenger.Register<TicketSelectorViewModel, TransactionCompleteMessage>(this,
                (receiver, message) => receiver.Receive(message));
        }

        private void Receive(TransactionCompleteMessage message)
        {
            var showTicketCounts = message.Value;

            foreach (KeyValuePair<int, int> showCount in showTicketCounts)
            {
                var show = todayShows.First(showTime => showTime.Id == showCount.Key);

                show.TicketCount += showCount.Value;
            } 
            
            SelectedMovie = null;
            SelectedShow = null;
        }
    }
}