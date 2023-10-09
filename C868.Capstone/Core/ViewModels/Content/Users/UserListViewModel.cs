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

namespace C868.Capstone.Core.ViewModels.Content.Users
{
    public class UserListViewModel : ContentViewModelBase
    {
        private List<UserViewModel> allUsers;

        private List<UserViewModel> users;
        public List<UserViewModel> Users
        {
            get => users;
            set => SetProperty(ref users, value);
        }

        private UserViewModel selectedUser;
        public UserViewModel SelectedUser
        {
            get => selectedUser;
            set
            {
                if (SetProperty(ref selectedUser, value))
                {
                    ((IRelayCommand)DeleteCommand).NotifyCanExecuteChanged();
                    Messenger.Send(new SelectedUserChangedMessage(selectedUser));
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

        public UserListViewModel(IDataService dataService, IDialogService dialogService,
            ILoggingService loggingService)
            : base(dataService, dialogService, loggingService)
        {
            IsActive = true;
            InitializeCommands();
        }

        public async Task InitializeData()
        {
            allUsers = (await DataService.GetUsersAsync())
                .Select(user => new UserViewModel(user))
                .ToList();

            Users = GetFilteredUsers();
        }

        private void InitializeCommands()
        {
            SearchCommand = new RelayCommand(ExecuteSearchCommand);
            ClearSearchCommand = new RelayCommand(ExecuteClearSearchCommand);
            DeleteCommand = new RelayCommand(
                ExecuteDeleteCommand,
                () => SelectedUser != null && SelectedUser.Id != 0);
        }

        private void ExecuteSearchCommand()
        {
            Users = GetFilteredUsers();
        }

        private void ExecuteClearSearchCommand()
        {
            SearchText = string.Empty;
            Users = GetFilteredUsers();
        }

        private void ExecuteDeleteCommand()
        {
            try
            {
                var confirmViewModel = new ConfirmDialogViewModel(
                    @"Delete User?",
                    $"Are you sure you want to delete \"{SelectedUser.UserName}?\"");

                DialogService.ShowDialog(confirmViewModel, async result =>
                {
                    if (result == true)
                    {
                        await DeleteUser();
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

        private async Task DeleteUser()
        {
            if (await IsLastAdminUser())
            {
                return;
            }

            // Delete confirmed
            if (await DataService.DeleteUserAsync(SelectedUser.User))
            {
                LogDelete(SelectedUser.UserName, "User");

                allUsers.Remove(SelectedUser);
                Users = GetFilteredUsers();
            }
            else
            {
                throw new InvalidOperationException(AppSettings.Errors.Data.DeleteMessage);
            }
        }

        private async Task<bool> IsLastAdminUser()
        {
            if (SelectedUser.UserType != UserType.Administrator)
            {
                return false;
            }

            var admins = (await DataService.GetUsersAsync())
                .Where(user => user.UserType == UserType.Administrator)
                .ToList();

            if (admins.Count > 1)
            {
                return false;
            }

            LoggingService.LogWarning(
                $"Attempted to delete last admin user: {SelectedUser.UserName}");

            var errorViewModel = new ErrorDialogViewModel(
                AppSettings.Dialogs.LastAdminTitle,
                AppSettings.Dialogs.LastAdminMessage,
                ErrorType.Error);

            DialogService.ShowErrorDialog(errorViewModel);
            
            return true;
        }

        private List<UserViewModel> GetFilteredUsers()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                return new List<UserViewModel>(allUsers);
            }

            var fuzzySearch = Regex
                .Replace(SearchText, "\\s+", "")
                .ToUpper();

            return allUsers
                .Where(user =>
                    Regex.Replace(user.UserName, "\\s+", "")
                        .ToUpper()
                        .Contains(fuzzySearch))
                .OrderBy(user => user.UserName)
                .ToList();
        }

        protected override void OnActivated()
        {
            Messenger.Register<UserListViewModel, UserSavedMessage>(this,
                (recipient, message) => recipient.Receive(message));
            Messenger.Register<UserListViewModel, ClearSelectedUserMessage>(this,
                (recipient, message) => recipient.Receive(message));
        }

        private async void Receive(UserSavedMessage message)
        {
            allUsers.Add(message.Value);
            Users = GetFilteredUsers();
        }

        private void Receive(ClearSelectedUserMessage message)
        {
            if (message.Value)
            {
                SelectedUser = null;
            }
        }
    }
}