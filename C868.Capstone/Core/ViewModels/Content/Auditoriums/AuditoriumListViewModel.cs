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

namespace C868.Capstone.Core.ViewModels.Content.Auditoriums
{
    public class AuditoriumListViewModel : ContentViewModelBase
    {
        private List<AuditoriumViewModel> allAuditoriums;

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
            set
            {
                if (SetProperty(ref selectedAuditorium, value))
                {
                    ((IRelayCommand)DeleteCommand).NotifyCanExecuteChanged();
                    Messenger.Send(new SelectedAuditoriumChangedMessage(selectedAuditorium));
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

        public AuditoriumListViewModel(IDataService dataService, IDialogService dialogService, ILoggingService loggingService)
            : base(dataService, dialogService, loggingService)
        {
            IsActive = true;
            InitializeCommands();
        }

        public async Task InitializeData()
        {
            allAuditoriums = new List<AuditoriumViewModel>();

            Auditoriums = new List<AuditoriumViewModel>();
            SelectedAuditorium = null;

            try
            {
                allAuditoriums = (await DataService.GetAuditoriumsAsync())
                    .Select(auditorium => new AuditoriumViewModel(auditorium))
                    .ToList();

                Auditoriums = GetFilteredAuditoriums();
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
            SearchCommand = new RelayCommand(ExecuteSearchCommand);
            ClearSearchCommand = new RelayCommand(ExecuteClearSearchCommand);

            DeleteCommand = new RelayCommand(
                ExecuteDeleteCommand,
                () => SelectedAuditorium != null &&
                      SelectedAuditorium.Id != 0);
        }

        private void ExecuteSearchCommand()
        {
            Auditoriums = GetFilteredAuditoriums();
        }

        private void ExecuteClearSearchCommand()
        {
            SearchText = string.Empty;
            Auditoriums = GetFilteredAuditoriums();
        }

        private void ExecuteDeleteCommand()
        {
            try
            {
                var confirmViewModel = new ConfirmDialogViewModel(
                    @"Delete Auditorium?",
                    $"Are you sure you want to delete \"{SelectedAuditorium.Name}?\"");

                DialogService.ShowDialog(confirmViewModel, async result =>
                {
                    if (result == true)
                    {
                        await DeleteAuditorium();
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

        private async Task DeleteAuditorium()
        {
            if (await DataService.DeleteAuditoriumAsync(SelectedAuditorium.Auditorium))
            {
                LogDelete(SelectedAuditorium.Name, @"Auditorium");

                allAuditoriums.Remove(SelectedAuditorium);
                GetFilteredAuditoriums();
            }
            else
            {
                throw new InvalidOperationException(AppSettings.Errors.Data.DeleteMessage);
            }
        }

        private List<AuditoriumViewModel> GetFilteredAuditoriums()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                return new List<AuditoriumViewModel>(allAuditoriums);
            }

            var fuzzySearch = Regex
                .Replace(SearchText, "\\s+", "")
                .ToUpper();

            return allAuditoriums
                .Where(auditorium =>
                    Regex.Replace(auditorium.Name, "\\s+", "")
                        .ToUpper()
                        .Contains(fuzzySearch))
                .OrderBy(auditorium => auditorium.Name)
                .ToList();
        }

        protected override void OnActivated()
        {
            Messenger.Register<AuditoriumListViewModel, ClearSelectedAuditoriumMessage>(this,
                (recipient, message) => recipient.Receive(message));
            Messenger.Register<AuditoriumListViewModel, AuditoriumSavedMessage>(this,
                (recipient, message) => recipient.Receive(message));
        }

        private void Receive(AuditoriumSavedMessage message)
        {
            allAuditoriums.Add(message.Value);
            Auditoriums = GetFilteredAuditoriums();
        }

        private void Receive(ClearSelectedAuditoriumMessage message)
        {
            if (message.Value)
            {
                SelectedAuditorium = null;
            }
        }
    }
}