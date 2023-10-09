using System;
using System.Windows.Input;
using C868.Capstone.Core.Messages;
using C868.Capstone.Core.Models.Data;
using C868.Capstone.Core.ViewModels.Dialogs;
using C868.Capstone.Services;
using C868.Capstone.Services.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace C868.Capstone.Core.ViewModels
{
    public class MainToolbarViewModel : ObservableRecipient
    {
        private readonly IDataService dataService;
        private readonly IDialogService dialogService;
        private readonly IServiceProvider services = App.Current.Services;

        private User currentUser = null;
        public User CurrentUser
        {
            get => currentUser;
            set
            {
                if (SetProperty(ref currentUser, value))
                {
                    OnPropertyChanged(nameof(LogInOutButtonIcon));
                    OnPropertyChanged(nameof(LogInOutButtonText));
                    OnPropertyChanged(nameof(LogInOutCommand));

                    NotifyToolbarCommands();
                    
                    Messenger.Send(new LoggedInUserChangedMessage(CurrentUser));
                }
            }
        }
        
        private bool isDayOpen;
        public bool IsDayOpen
        {
            get => isDayOpen;
            set
            {
                if (SetProperty(ref isDayOpen, value))
                {
                    OnPropertyChanged(nameof(OpenCloseButtonIcon));
                    OnPropertyChanged(nameof(OpenCloseButtonText));
                    OnPropertyChanged(nameof(OpenCloseCommand));
                    ((IRelayCommand)SellingCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public string LogInOutButtonIcon =>
            CurrentUser is null
                ? AppSettings.Icons.LogIn
                : AppSettings.Icons.LogOut;

        public string LogInOutButtonText =>
            CurrentUser is null
                ? AppSettings.Captions.LogIn
                : AppSettings.Captions.LogOut;

        public string OpenCloseButtonIcon =>
            isDayOpen
                ? AppSettings.Icons.Close
                : AppSettings.Icons.Open;

        public string OpenCloseButtonText =>
            isDayOpen
                ? AppSettings.Captions.Close
                : AppSettings.Captions.Open;

        public string SellingButtonIcon { get; set; }
        public string SellingButtonText { get; set; }

        public string ReportsButtonIcon { get; private set; }
        public string ReportsButtonText { get; private set; }

        public string ShowTimesButtonIcon { get; private set; }
        public string ShowTimesButtonText { get; private set; }

        public string AuditoriumsButtonIcon { get; private set; }
        public string AuditoriumsButtonText { get; private set; }

        public string MoviesButtonIcon { get; private set; }
        public string MoviesButtonText { get; private set; }

        public string TicketsButtonIcon { get; private set; }
        public string TicketsButtonText { get; private set; }

        public string UsersButtonIcon { get; private set; }
        public string UsersButtonText { get; private set; }

        public ICommand LogInOutCommand => CurrentUser is null
            ? new RelayCommand(ExecuteLogInCommand)
            : new RelayCommand(ExecuteLogOutCommand);

        public ICommand OpenCloseCommand => IsDayOpen
            ? new RelayCommand(ExecuteCloseCommand, IsUserLoggedIn)
            : new RelayCommand(ExecuteOpenCommand, IsUserLoggedIn);

        public ICommand SellingCommand { get; private set; }
        public ICommand ReportsCommand { get; private set; }
        public ICommand ShowTimesCommand { get; private set; }
        public ICommand AuditoriumsCommand { get; private set; }
        public ICommand MoviesCommand { get; private set; }
        public ICommand TicketsCommand { get; private set; }
        public ICommand UsersCommand { get; private set; }

        public MainToolbarViewModel(IDataService dataService, IDialogService dialogService)
        {
            this.dataService = dataService;
            this.dialogService = dialogService;

            InitializeButtons();
            InitializeButtonCommands();
            InitializeApplicationStatus();
        }

        private void InitializeApplicationStatus()
        {
            IsDayOpen = false;
        }
        
        public void InitializeButtons()
        {
            SellingButtonIcon = AppSettings.Icons.Selling;
            SellingButtonText = AppSettings.Captions.Selling;

            ReportsButtonIcon = AppSettings.Icons.Reports;
            ReportsButtonText = AppSettings.Captions.Reports;

            ShowTimesButtonIcon = AppSettings.Icons.ShowTimes;
            ShowTimesButtonText = AppSettings.Captions.ShowTimes;

            AuditoriumsButtonIcon = AppSettings.Icons.Auditoriums;
            AuditoriumsButtonText = AppSettings.Captions.Auditoriums;

            MoviesButtonIcon = AppSettings.Icons.Movies;
            MoviesButtonText = AppSettings.Captions.Movies;

            TicketsButtonIcon = AppSettings.Icons.TicketTypes;
            TicketsButtonText = AppSettings.Captions.TicketTypes;

            UsersButtonIcon = AppSettings.Icons.Users;
            UsersButtonText = AppSettings.Captions.Users;
        }

        public void InitializeButtonCommands()
        {
            SellingCommand = new RelayCommand(
                ExecuteSellingCommand,
                () => IsDayOpen && IsUserLoggedIn());
            ReportsCommand = new RelayCommand(ExecuteReportsCommand, IsUserAdmin);
            ShowTimesCommand = new RelayCommand(ExecuteShowTimesCommand, IsUserAdmin);
            AuditoriumsCommand = new RelayCommand(ExecuteAuditoriumsCommand, IsUserAdmin);
            MoviesCommand = new RelayCommand(ExecuteMoviesCommand, IsUserAdmin);
            TicketsCommand = new RelayCommand(ExecuteTicketsCommand, IsUserAdmin);
            UsersCommand = new RelayCommand(ExecuteUsersCommand, IsUserAdmin);
        }

        private void ExecuteLogInCommand()
        {
            var loginViewModel = services.GetService<LogInDialogViewModel>();

            if (loginViewModel is null)
            {
                return;
            }

            loginViewModel.Title = AppSettings.Dialogs.LogInTitle;

            dialogService.ShowDialog(loginViewModel, result =>
            {
                if (result == true)
                {
                    CurrentUser = loginViewModel?.User;
                }
            });
        }

        private void ExecuteLogOutCommand()
        {
            CurrentUser = null;
        }

        private void ExecuteOpenCommand()
        {
            Messenger.Send(new OpenDayMessage(true));
        }

        private void ExecuteCloseCommand()
        {
            Messenger.Send(new CloseDayMessage(true));
        }

        private void ExecuteSellingCommand()
        {
            Messenger.Send(new ShowSellingViewMessage(true));
        }

        private void ExecuteReportsCommand()
        {
            Messenger.Send(new ShowReportsViewMessage(true));
        }

        private void ExecuteShowTimesCommand()
        {
            Messenger.Send(new ShowShowTimesViewMessage(true));
        }

        private void ExecuteAuditoriumsCommand()
        {
            Messenger.Send(new ShowAuditoriumsViewMessage(true));
        }

        private void ExecuteMoviesCommand()
        {
            Messenger.Send(new ShowMoviesViewMessage(true));
        }

        private void ExecuteTicketsCommand()
        {
            Messenger.Send(new ShowTicketTypesViewMessage(true));
        }

        private void ExecuteUsersCommand()
        {
            Messenger.Send(new ShowUsersViewMessage(true));
        }

        private void NotifyToolbarCommands()
        {
            OnPropertyChanged(nameof(OpenCloseCommand));
            ((IRelayCommand)SellingCommand)?.NotifyCanExecuteChanged();
            ((IRelayCommand)ReportsCommand)?.NotifyCanExecuteChanged();
            ((IRelayCommand)ShowTimesCommand)?.NotifyCanExecuteChanged();
            ((IRelayCommand)AuditoriumsCommand)?.NotifyCanExecuteChanged();
            ((IRelayCommand)MoviesCommand)?.NotifyCanExecuteChanged();
            ((IRelayCommand)TicketsCommand)?.NotifyCanExecuteChanged();
            ((IRelayCommand)UsersCommand)?.NotifyCanExecuteChanged();
        }

        private bool IsUserLoggedIn()
        {
            return CurrentUser != null;
        }

        private bool IsUserAdmin()
        {
            return CurrentUser?.UserType == UserType.Administrator;
        }
    }
}