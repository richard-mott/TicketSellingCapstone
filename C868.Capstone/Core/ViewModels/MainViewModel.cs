using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using C868.Capstone.Core.Messages;
using C868.Capstone.Core.Models.Activities;
using C868.Capstone.Core.Models.Data;
using C868.Capstone.Core.ViewModels.Content.Auditoriums;
using C868.Capstone.Core.ViewModels.Content.Movies;
using C868.Capstone.Core.ViewModels.Content.Reports;
using C868.Capstone.Core.ViewModels.Content.Selling;
using C868.Capstone.Core.ViewModels.Content.ShowTimes;
using C868.Capstone.Core.ViewModels.Content.TicketTypes;
using C868.Capstone.Core.ViewModels.Content.Users;
using C868.Capstone.Core.ViewModels.Data;
using C868.Capstone.Core.ViewModels.Dialogs;
using C868.Capstone.Services;
using C868.Capstone.Services.Data;
using C868.Capstone.Services.Logging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace C868.Capstone.Core.ViewModels
{
    public class MainViewModel : ObservableRecipient, IDataLoader
    {
        private IProgress<int> valueProgress;
        private IProgress<string> descriptionProgress;

        private readonly IDataService dataService;
        private readonly IDialogService dialogService;
        private readonly IContentService contentService;
        private readonly ILoggingService loggingService;
        private readonly Version version;
        private readonly IServiceProvider services = App.Current.Services;

        private string AppUserName => CurrentUser != null ? $" [{CurrentUser.UserName}]" : "";
        public string AppTitle => $"Tix v{version.Major}.{version.Minor}{AppUserName}";

        private User currentUser = null;
        public User CurrentUser
        {
            get => currentUser;
            set
            {
                if (SetProperty(ref currentUser, value))
                {
                    loggingService.User = currentUser;
                    
                    if (currentUser is null)
                    {
                        Content = null;
                    }

                    OnPropertyChanged(nameof(AppTitle));
                }
            }
        }

        private MainToolbarViewModel toolbar;
        public MainToolbarViewModel Toolbar
        {
            get => toolbar;
            set => SetProperty(ref toolbar, value);
        }

        private object content;
        public object Content
        {
            get => content;
            set => SetProperty(ref content, value);
        }

        private DailyRecord currentDailyRecord;
        public DailyRecord CurrentDailyRecord
        {
            get => currentDailyRecord;
            set
            {
                if (SetProperty(ref currentDailyRecord, value))
                {
                    Toolbar.IsDayOpen = currentDailyRecord?.IsOpen ?? false;
                }
            }
        }

        private bool isIndeterminate;
        public bool IsIndeterminate
        {
            get => isIndeterminate;
            set
            {
                if (SetProperty(ref isIndeterminate, value))
                {
                    OnPropertyChanged(nameof(ShowPercentage));
                }
            }
        }

        public bool ShowPercentage => !IsIndeterminate;

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

        public MainViewModel(IDataService dataService, IDialogService dialogService,
            IContentService contentService, ILoggingService loggingService)
        {
            this.dataService = dataService;
            this.dialogService = dialogService;
            this.contentService = contentService;
            this.loggingService = loggingService;

            version = Assembly.GetEntryAssembly()?.GetName().Version
                      ?? new Version(0, 0, 0, 0);

            IsActive = true;
            Toolbar = services.GetService<MainToolbarViewModel>();
            CurrentUser = null;

            InitializeProgressReporting();
        }

        private void InitializeProgressReporting()
        {
            valueProgress = new Progress<int>(
                value => ProgressValue = value);
            descriptionProgress = new Progress<string>(
                description => ProgressDescription = description);
        }

        public async Task InitializeData()
        {
            if (dataService is null)
            {
                HandleError(
                    @"Database Error",
                    @"Unable to access the database. Please contact your " +
                    @"system administrator for assistance.");
                
                return;
            }

            IsIndeterminate = true;
            ShowProgress = true;
            descriptionProgress.Report(@"Loading...");

            await dataService.InitializeTables(valueProgress, descriptionProgress);

            ShowProgress = false;

            var (hasMissingData, message) = await CheckForMissingData();
            if (!hasMissingData)
            {
                await CheckForOpenDailyRecord();
                return;
            }
            
            var confirmViewModel = new ConfirmDialogViewModel(@"Missing Data", message);
            dialogService.ShowDialog(confirmViewModel, async (result) =>
            {
                if (result == true)
                {
                    await LoadSampleData();
                }

            });
        }

        private async Task LoadSampleData()
        {
            try
            {
                IsIndeterminate = false;
                ShowProgress = true;

                await dataService.InitializeData(valueProgress, descriptionProgress);
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

        private async Task CheckForOpenDailyRecord()
        {
            try
            {
                // Set the current daily record to the first record found that is currently open
                // or null if there are not any currently open records
                var openRecord = (await dataService
                        .GetDailyRecordsAsync(DateTime.Now.AddDays(-1), DateTime.Now))
                    .FirstOrDefault(dailyRecord => dailyRecord.IsOpen);

                CurrentDailyRecord = openRecord;
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
        
        private async Task<(bool HasMissingData, string Message)> CheckForMissingData()
        {
            var hasAuditoriums = await dataService.HasAuditoriumsAsync();
            var hasMovies = await dataService.HasMoviesAsync();
            var hasTicketTypes = await dataService.HasTicketTypesAsync();
            var hasShowTimes = await dataService.HasShowTimesAsync();
            var hasTickets = await dataService.HasTicketsAsync();
            var hasLogEntries = await dataService.HasLogEntriesAsync();
            var hasDailyActivities = await dataService.HasDailyRecordsAsync();

            if (hasAuditoriums && hasMovies && hasTicketTypes && hasShowTimes &&
                hasTickets && hasLogEntries && hasDailyActivities)
            {
                return (false, string.Empty);
            }

            var builder = new StringBuilder();

            builder.Append(@"The following sections of the database are empty:");
            builder.Append(Environment.NewLine);
            builder.Append(Environment.NewLine);
            builder.Append(hasAuditoriums ? string.Empty : "AUDITORIUMS" + Environment.NewLine);
            builder.Append(hasMovies ? string.Empty : "MOVIES" + Environment.NewLine);
            builder.Append(hasTicketTypes ? string.Empty : "TICKET TYPES" + Environment.NewLine);
            builder.Append(hasShowTimes ? string.Empty : "SHOW TIMES" + Environment.NewLine);
            builder.Append(hasTickets ? string.Empty : "TICKETS" + Environment.NewLine);
            builder.Append(hasLogEntries ? string.Empty : "LOG ENTRIES" + Environment.NewLine);
            builder.Append(hasDailyActivities ? string.Empty : "DAILY ACTIVITIES" + Environment.NewLine);
            builder.Append(Environment.NewLine);
            builder.Append(@"Would you like to use the sample data?");

            return (true, builder.ToString());
        }

        private async Task OpenDay()
        {
            try
            {
                var dailyRecord = new DailyRecord
                {
                    OpenDate = DateTime.Now,
                    IsOpen = true
                };

                await dataService.SaveDailyRecordAsync(dailyRecord);

                CurrentDailyRecord = dailyRecord;

                var logEntry = new LogEntry
                {
                    Type = LogMessageType.Info,
                    Message = $"Opened day {CurrentDailyRecord.OpenDate:d}",
                    UserId = CurrentUser.UserId,
                    User = CurrentUser
                };

                await dataService.SaveLogEntryAsync(logEntry);
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

        private async Task CloseDay()
        {
            try
            {
                CurrentDailyRecord.CloseDate = DateTime.Now;
                CurrentDailyRecord.CashExpected = await GetExpectedCash();
                CurrentDailyRecord.CreditExpected = await GetExpectedCredit();
                CurrentDailyRecord.CheckExpected = await GetExpectedCheck();
            }
            catch (Exception exception)
            {
                HandleError(
                    AppSettings.Errors.Data.LoadTitle,
                    string.IsNullOrWhiteSpace(exception.Message)
                        ? AppSettings.Errors.Data.LoadMessage
                        : exception.Message);
            }

            try 
            { 
                var dailyRecordViewModel = new CloseDailyRecordViewModel(CurrentDailyRecord);
                var dialogViewModel = new CloseDayDialogViewModel(dailyRecordViewModel);

                dialogService.ShowDialog(dialogViewModel, async result =>
                {
                    if (result == true)
                    {
                        CurrentDailyRecord.IsOpen = false;
                        await dataService.SaveDailyRecordAsync(CurrentDailyRecord);

                        var logEntry = new LogEntry
                        {
                            Type = LogMessageType.Info,
                            Message = $"Closed day {CurrentDailyRecord.CloseDate:d}",
                            UserId = CurrentUser.UserId,
                            User = CurrentUser
                        };

                        await dataService.SaveLogEntryAsync(logEntry);

                        CurrentDailyRecord = null;
                        Toolbar.IsDayOpen = false;
                    }
                });
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

        private async Task<double> GetExpectedCash()
        {
            return (await dataService.GetTicketsAsync(
                    CurrentDailyRecord.OpenDate, CurrentDailyRecord.CloseDate))
                .Where(ticket => ticket.Payment == PaymentType.Cash)
                .Sum(ticket => ticket.Count * ticket.TicketType.Price);
        }

        private async Task<double> GetExpectedCredit()
        {
            return (await dataService.GetTicketsAsync(
                    CurrentDailyRecord.OpenDate, CurrentDailyRecord.CloseDate))
                .Where(ticket => ticket.Payment == PaymentType.Credit)
                .Sum(ticket => ticket.Count * ticket.TicketType.Price);
        }

        private async Task<double> GetExpectedCheck()
        {
            return (await dataService.GetTicketsAsync(
                    CurrentDailyRecord.OpenDate, CurrentDailyRecord.CloseDate))
                .Where(ticket => ticket.Payment == PaymentType.Check)
                .Sum(ticket => ticket.Count * ticket.TicketType.Price);
        }

        protected override void OnActivated()
        {
            Messenger.Register<MainViewModel, LoggedInUserChangedMessage>(this,
                (receiver, message) => receiver.Receive(message));

            Messenger.Register<MainViewModel, OpenDayMessage>(this,
                (receiver, message) => receiver.Receive(message));
            Messenger.Register<MainViewModel, CloseDayMessage>(this,
                (receiver, message) => receiver.Receive(message));
            Messenger.Register<MainViewModel, ShowSellingViewMessage>(this,
                (receiver, message) => receiver.Receive(message));
            Messenger.Register<MainViewModel, ShowReportsViewMessage>(this,
                (receiver, message) => receiver.Receive(message));
            Messenger.Register<MainViewModel, ShowShowTimesViewMessage>(this,
                (receiver, message) => receiver.Receive(message));
            Messenger.Register<MainViewModel, ShowAuditoriumsViewMessage>(this,
                (receiver, message) => receiver.Receive(message));
            Messenger.Register<MainViewModel, ShowMoviesViewMessage>(this,
                (receiver, message) => receiver.Receive(message));
            Messenger.Register<MainViewModel, ShowTicketTypesViewMessage>(this,
                (receiver, message) => receiver.Receive(message));
            Messenger.Register<MainViewModel, ShowUsersViewMessage>(this,
                (receiver, message) => receiver.Receive(message));
        }

        private async void Receive(LoggedInUserChangedMessage message)
        {
            CurrentUser = message.Value;
            await CheckForOpenDailyRecord();
        }

        private async Task Receive(OpenDayMessage message)
        {
            var confirmViewModel = new ConfirmDialogViewModel(
                @"Open Day",
                @"Do you wish to open the day?");

            dialogService.ShowDialog(confirmViewModel, async result =>
            {
                if (result == true)
                {
                    await OpenDay();
                }
            });
        }

        private async Task Receive(CloseDayMessage message)
        {
            await CloseDay();
        }

        private async Task Receive(ShowSellingViewMessage message)
        {
            if (!message.Value)
            {
                return;
            }

            try
            {
                var sellingViewModel = services.GetService<SellingViewModel>();

                if (sellingViewModel is null)
                {
                    throw new NullReferenceException(AppSettings.Errors.ContentPage.Message);
                }

                Content = contentService.GetContent(sellingViewModel);
                await sellingViewModel.InitializeData();
            }
            catch (Exception exception)
            {
                HandleError(
                    AppSettings.Errors.ContentPage.Title,
                    string.IsNullOrWhiteSpace(exception.Message)
                        ? AppSettings.Errors.ContentPage.Message
                        : exception.Message);
            }
        }

        private async Task Receive(ShowReportsViewMessage message)
        {
            if (!message.Value)
            {
                return;
            }

            try
            {
                var reportsViewModel = services.GetService<ReportsViewModel>();

                if (reportsViewModel is null)
                {
                    throw new NullReferenceException(AppSettings.Errors.ContentPage.Message);
                }

                Content = contentService.GetContent(reportsViewModel);
                await reportsViewModel.InitializeData();
            }
            catch (Exception exception)
            {
                HandleError(AppSettings.Errors.ContentPage.Title, exception.Message);
            }
        }

        private async Task Receive(ShowShowTimesViewMessage message)
        {
            if (!message.Value)
            {
                return;
            }

            try
            {
                var showTimesViewModel = services.GetService<ShowTimesViewModel>();

                if (showTimesViewModel is null)
                {
                    throw new NullReferenceException(AppSettings.Errors.ContentPage.Message);
                }

                Content = contentService.GetContent(showTimesViewModel);
                await showTimesViewModel.InitializeData();
            }
            catch (Exception exception)
            {
                HandleError(AppSettings.Errors.ContentPage.Title, exception.Message);
            }
        }

        private async Task Receive(ShowAuditoriumsViewMessage message)
        {
            if (!message.Value)
            {
                return;
            }

            try
            {
                var auditoriumsViewModel = services.GetService<AuditoriumsViewModel>();

                if (auditoriumsViewModel is null)
                {
                    throw new NullReferenceException(AppSettings.Errors.ContentPage.Message);
                }

                Content = contentService.GetContent(auditoriumsViewModel);
                await auditoriumsViewModel.InitializeData();
            }
            catch (Exception exception)
            {
                HandleError(AppSettings.Errors.ContentPage.Title, exception.Message);
            }
        }

        private async Task Receive(ShowMoviesViewMessage message)
        {
            if (!message.Value)
            {
                return;
            }

            try
            {
                var movieEditorViewModel = services.GetService<MoviesViewModel>();

                if (movieEditorViewModel is null)
                {
                    throw new NullReferenceException(AppSettings.Errors.ContentPage.Message);
                }

                Content = contentService.GetContent(movieEditorViewModel);
                await movieEditorViewModel.InitializeData();
            }
            catch (Exception exception)
            {
                HandleError(AppSettings.Errors.ContentPage.Title, exception.Message);
            }
        }

        private async Task Receive(ShowTicketTypesViewMessage message)
        {
            if (!message.Value)
            {
                return;
            }

            try
            {
                var ticketTypeEditorViewModel = services.GetService<TicketTypesViewModel>();

                if (ticketTypeEditorViewModel is null)
                {
                    throw new NullReferenceException(AppSettings.Errors.ContentPage.Message);
                }

                Content = contentService.GetContent(ticketTypeEditorViewModel);
                await ticketTypeEditorViewModel.InitializeData();
            }
            catch (Exception exception)
            {
                HandleError(AppSettings.Errors.ContentPage.Title, exception.Message);
            }
        }

        private async Task Receive(ShowUsersViewMessage message)
        {
            if (!message.Value)
            {
                return;
            }

            try
            {
                var userEditorViewModel = services.GetService<UsersViewModel>();

                if (userEditorViewModel is null)
                {
                    throw new NullReferenceException(AppSettings.Errors.ContentPage.Message);
                }

                Content = contentService.GetContent(userEditorViewModel);
                await userEditorViewModel.InitializeData();
            }
            catch (Exception exception)
            {
                HandleError(AppSettings.Errors.ContentPage.Title, exception.Message);
            }
        }

        private void HandleError(string title, string message)
        {
            var errorViewModel = new ErrorDialogViewModel(title, message, ErrorType.Error);

            dialogService.ShowErrorDialog(errorViewModel);
        }
    }
}