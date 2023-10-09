using System.Threading.Tasks;
using C868.Capstone.Core.Models.Data;
using C868.Capstone.Services.Data;

namespace C868.Capstone.Core.ViewModels.Dialogs
{
    public class LogInDialogViewModel : DialogViewModel
    {
        private readonly IDataService dataService;

        public User User { get; private set; }

        public string UserName
        {
            get => User.UserName;
            set => SetProperty(User.UserName, value, User,
                (user, userName) => user.UserName = userName);
        }

        public string Password
        {
            get => User.Password;
            set => SetProperty(User.Password, value, User,
                (user, password) => user.Password = password);
        }

        private string userNameError = string.Empty;
        public string UserNameError
        {
            get => userNameError;
            set => SetProperty(ref userNameError, value);
        }

        private string passwordError = string.Empty;
        public string PasswordError
        {
            get => passwordError;
            set => SetProperty(ref passwordError, value);
        }

        public LogInDialogViewModel(IDataService dataService)
        {
            this.dataService = dataService;
            User = new User();
        }

        protected override async void ExecuteOKCommand()
        {
            UserNameError = string.Empty;
            PasswordError = string.Empty;

            var user = await ValidateUser();

            if (user == null)
            {
                return;
            }

            User = user;
            base.ExecuteOKCommand();
        }

        private async Task<User> ValidateUser()
        {
            var isUserNameBlank = string.IsNullOrWhiteSpace(UserName);
            var isPasswordBlank = string.IsNullOrWhiteSpace(Password);

            if (isUserNameBlank)
            {
                UserNameError = AppSettings.ValidationErrors.LogIn.BlankUserName;
            }

            if (isPasswordBlank)
            {
                PasswordError = AppSettings.ValidationErrors.LogIn.BlankPassword;
            }

            if (isUserNameBlank || isPasswordBlank)
            {
                return null;
            }

            var validatedUser = await dataService.ValidateUserAsync(UserName, Password);

            if (validatedUser is null)
            {
                PasswordError = AppSettings.ValidationErrors.LogIn.IncorrectLogIn;
            }

            return validatedUser;
        }
    }
}