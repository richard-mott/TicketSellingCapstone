using C868.Capstone.Core.Models.Data;
using CommunityToolkit.Mvvm.ComponentModel;

namespace C868.Capstone.Core.ViewModels.Data
{
    public class UserViewModel : ObservableObject
    {
        private readonly User user;
        public User User => user ?? new User();
        public int Id => User.UserId;

        public string UserName
        {
            get => User.UserName;
            set => SetProperty(User.UserName, value, User,
                (u, userName) => u.UserName = userName);
        }

        public string Password
        {
            get => User.Password;
            set => SetProperty(User.Password, value, User,
                (u, password) => u.Password = password);
        }

        public UserType UserType
        {
            get => User.UserType;
            set => SetProperty(User.UserType, value, User,
                (u, permissions) => u.UserType = permissions);
        }

        public UserViewModel(User user)
        {
            this.user = user;
        }

        public UserViewModel Clone()
        {
            return new UserViewModel(User.Clone());
        }
    }
}