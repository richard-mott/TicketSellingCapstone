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

namespace C868.Capstone.Core.ViewModels.Content.Users
{
    public class UserEditorViewModel : ContentViewModelBase
    {
        public List<UserType> UserTypes => new List<UserType>
        {
            UserType.User, UserType.Administrator
        };

        public string Title => CurrentUser.Id == 0
            ? "Create User"
            : "Edit User";

        private UserViewModel currentUser = new UserViewModel(new User());
        public UserViewModel CurrentUser
        {
            get => currentUser;
            set
            {
                var newValue = value is null
                    ? new UserViewModel(new User())
                    : value.Clone();

                if (SetProperty(ref currentUser, newValue))
                {
                    ClearErrors();
                    OnPropertyChanged(nameof(Title));
                    OnPropertyChanged(nameof(IsExistingUser));
                    Messenger.Send(new ClearPasswordsMessage(true));
                }
            }
        }

        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public bool IsExistingUser => !string.IsNullOrWhiteSpace(CurrentUser.Password);

        private string userNameError;
        public string UserNameError
        {
            get => userNameError;
            set => SetProperty(ref userNameError, value);
        }

        private string currentPasswordError;
        public string CurrentPasswordError
        {
            get => currentPasswordError;
            set => SetProperty(ref currentPasswordError, value);
        }

        private string newPasswordError;
        public string NewPasswordError
        {
            get => newPasswordError;
            set => SetProperty(ref newPasswordError, value);
        }

        private string confirmPasswordError;
        public string ConfirmPasswordError
        {
            get => confirmPasswordError;
            set => SetProperty(ref confirmPasswordError, value);
        }

        public ICommand SaveCommand { get; private set; }
        public ICommand ClearCommand { get; private set; }

        public UserEditorViewModel(IDataService dataService, IDialogService dialogService,
            ILoggingService loggingService)
            : base(dataService, dialogService, loggingService)
        {
            CurrentUser = new UserViewModel(new User());

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
                await SaveUser();
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
            CurrentUser = new UserViewModel(new User());

            ClearErrors();

            Messenger.Send(new ClearPasswordsMessage(true));
            Messenger.Send(new ClearSelectedUserMessage(true));
        }

        private async Task SaveUser()
        {
            if (!await IsUserValid())
            {
                return;
            }

            CurrentUser.Password = NewPassword;

            if (await DataService.SaveUserAsync(CurrentUser.User))
            {
                LogSave(CurrentUser.UserName, @"User");

                Messenger.Send(new UserSavedMessage(CurrentUser));
                CurrentUser = new UserViewModel(new User());
            }
            else
            {
                throw new InvalidOperationException(AppSettings.Errors.Data.SaveMessage);
            }
        }

        protected override void OnActivated()
        {
            Messenger.Register<UserEditorViewModel, SelectedUserChangedMessage>(this,
                (receiver, message) => receiver.Receive(message));
        }

        private void Receive(SelectedUserChangedMessage message)
        {
            CurrentUser = message.Value;
        }

        private async Task<bool> IsUserValid()
        {
            var isUserNameValid = await IsUserNameValid();

            var isCurrentPasswordValid = await IsCurrentPasswordValid();

            var isNewPasswordValid = IsNewPasswordValid();
            
            return isUserNameValid && isCurrentPasswordValid && isNewPasswordValid;
        }

        private async Task<bool> IsUserNameValid()
        {
            UserNameError = string.Empty;

            // User name cannot be blank
            if (string.IsNullOrWhiteSpace(CurrentUser.UserName))
            {
                UserNameError = AppSettings.ValidationErrors.User.UserNameBlank;
                return false;
            }

            // Verify user name is available
            var users = await DataService.GetUsersAsync();
            var foundUser = users.FirstOrDefault(user => user.UserName == CurrentUser.UserName);

            if (foundUser != null)
            {
                UserNameError = AppSettings.ValidationErrors.User.UserNameTaken;
                return false;
            }

            // User name is valid
            return true;
        }

        private async Task<bool> IsCurrentPasswordValid()
        {
            CurrentPasswordError = string.Empty;

            // Current password cannot be blank
            if (string.IsNullOrWhiteSpace(CurrentUser.Password))
            {
                CurrentPasswordError = AppSettings.ValidationErrors.User.CurrentPasswordBlank;
                return false;
            }

            // Verify current password
            var foundUser = await DataService
                .ValidateUserAsync(CurrentUser.UserName, CurrentPassword);

            if (foundUser is null)
            {
                CurrentPasswordError = AppSettings.ValidationErrors.User.CurrentPasswordInvalid;
            }

            return true;
        }

        private bool IsNewPasswordValid()
        {
            NewPasswordError = string.Empty;
            ConfirmPasswordError = string.Empty;

            // Password cannot be blank
            if (string.IsNullOrWhiteSpace(NewPassword))
            {
                NewPasswordError = AppSettings.ValidationErrors.User.NewPasswordBlank;
                return false;
            }

            // Confirm password cannot be blank
            if (string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                ConfirmPasswordError = AppSettings.ValidationErrors.User.ConfirmPasswordBlank;
                return false;
            }

            // Passwords must match
            if (NewPassword != ConfirmPassword)
            {
                NewPasswordError = AppSettings.ValidationErrors.User.PasswordsDoNotMatch;
                ConfirmPasswordError = AppSettings.ValidationErrors.User.PasswordsDoNotMatch;
                return false;
            }

            // Password is valid
            return true;
        }
        
        private void ClearErrors()
        {
            UserNameError = string.Empty;
            CurrentPasswordError = string.Empty;
            NewPasswordError = string.Empty;
            ConfirmPasswordError = string.Empty;
        }
    }
}