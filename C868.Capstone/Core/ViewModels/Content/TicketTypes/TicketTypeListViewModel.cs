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

namespace C868.Capstone.Core.ViewModels.Content.TicketTypes
{
    public class TicketTypeListViewModel : ContentViewModelBase
    {
        private List<TicketTypeViewModel> allTicketTypes;

        private List<TicketTypeViewModel> ticketTypes;
        public List<TicketTypeViewModel> TicketTypes
        {
            get => ticketTypes;
            set => SetProperty(ref ticketTypes, value);
        }

        private TicketTypeViewModel selectedTicketType;
        public TicketTypeViewModel SelectedTicketType
        {
            get => selectedTicketType;
            set
            {
                if (SetProperty(ref selectedTicketType, value))
                {
                    ((IRelayCommand)DeleteCommand).NotifyCanExecuteChanged();
                    Messenger.Send(new SelectedTicketTypeChangedMessage(selectedTicketType));
                }
            }
        }

        private string searchText;
        public string SearchText
        {
            get => searchText;
            set => SetProperty(ref searchText, value);
        }

        private bool isLoading;
        public bool IsLoading
        {
            get => isLoading;
            set => SetProperty(ref isLoading, value);
        }

        public ICommand SearchCommand { get; private set; }
        public ICommand ClearSearchCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }

        public TicketTypeListViewModel(IDataService dataService, IDialogService dialogService,
            ILoggingService loggingService)
            : base(dataService, dialogService, loggingService)
        {
            TicketTypes = new List<TicketTypeViewModel>();
            SelectedTicketType = null;

            IsActive = true;
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            SearchCommand = new RelayCommand(ExecuteSearchCommand);
            ClearSearchCommand = new RelayCommand(ExecuteClearSearchCommand);
            DeleteCommand = new RelayCommand(
                ExecuteDeleteCommand,
                () => SelectedTicketType != null && SelectedTicketType.Id != 0);
        }

        public async Task InitializeData()
        {
            allTicketTypes = new List<TicketTypeViewModel>();

            TicketTypes = new List<TicketTypeViewModel>();
            SelectedTicketType = null;

            try
            {
                IsLoading = true;

                allTicketTypes = (await DataService.GetTicketTypesAsync())
                    .Select(ticketType => new TicketTypeViewModel(ticketType))
                    .ToList();
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
                TicketTypes = GetFilteredTicketTypes();
                IsLoading = false;
            }
        }

        private void ExecuteSearchCommand()
        {
            TicketTypes = GetFilteredTicketTypes();
        }

        private void ExecuteClearSearchCommand()
        {
            SearchText = string.Empty;
            TicketTypes = GetFilteredTicketTypes();
        }

        private void ExecuteDeleteCommand()
        {
            try
            {
                var confirmViewModel = new ConfirmDialogViewModel(
                    @"Delete Ticket Type?",
                    $"Are you sure you want to delete \"{SelectedTicketType.Name}?\"");

                DialogService.ShowDialog(confirmViewModel, async result =>
                {
                    if (result == true)
                    {
                        await DeleteTicketType();
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

        private List<TicketTypeViewModel> GetFilteredTicketTypes()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                return new List<TicketTypeViewModel>(allTicketTypes);
            }

            var fuzzySearch = Regex
                .Replace(SearchText, "\\s+", "")
                .ToUpper();

            return allTicketTypes
                .Where(ticketType =>
                    Regex.Replace(ticketType.Name, "\\s+", "")
                        .ToUpper()
                        .Contains(fuzzySearch))
                .OrderBy(ticketType => ticketType.Name)
                .ToList();
        }

        private async Task DeleteTicketType()
        { 
            if (await DataService.DeleteTicketTypeAsync(SelectedTicketType.TicketType))
            {
                LogDelete(SelectedTicketType.Name, @"Ticket Type");

                allTicketTypes.Remove(SelectedTicketType);
                TicketTypes = GetFilteredTicketTypes();
            }
            else
            {
                throw new InvalidOperationException(AppSettings.Errors.Data.DeleteMessage);
            }
        }

        protected override void OnActivated()
        {
            Messenger.Register<TicketTypeListViewModel, TicketTypeSavedMessage>(this,
                (receiver, message) => receiver.Receive(message));
            Messenger.Register<TicketTypeListViewModel, ClearSelectedTicketTypeMessage>(
                this,
                (receiver, message) => receiver.Receive(message));
        }

        private async void Receive(TicketTypeSavedMessage message)
        {
            allTicketTypes.Add(message.Value);
            GetFilteredTicketTypes();
        }

        private void Receive(ClearSelectedTicketTypeMessage message)
        {
            if (message.Value)
            {
                SelectedTicketType = null;
            }
        }
    }
}