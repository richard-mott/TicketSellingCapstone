using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace C868.Capstone.Core.ViewModels.Content.Users
{
    public class UsersViewModel : ObservableRecipient
    {
        private IProgress<int> valueProgess;
        private IProgress<string> descriptionProgress;
        private readonly IServiceProvider services = App.Current.Services;

        private UserListViewModel userList;
        public UserListViewModel UserList
        {
            get => userList;
            set => SetProperty(ref userList, value);
        }

        private UserEditorViewModel userEditor;
        public UserEditorViewModel UserEditor
        {
            get => userEditor;
            set => SetProperty(ref userEditor, value);
        }

        public bool IsIndeterminate => true;

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

        public UsersViewModel()
        {
            UserList = services.GetService<UserListViewModel>();
            UserEditor = services.GetService<UserEditorViewModel>();

            InitializeProgressReporting();
        }

        public async Task InitializeData()
        {
            ShowProgress = true;
            descriptionProgress.Report("Loading users...");

            await UserList.InitializeData();

            descriptionProgress.Report(string.Empty);
            ShowProgress = false;
        }

        private void InitializeProgressReporting()
        {
            valueProgess = new Progress<int>(
                value => ProgressValue = value);
            descriptionProgress = new Progress<string>(
                description => ProgressDescription = description);
        }
    }
}