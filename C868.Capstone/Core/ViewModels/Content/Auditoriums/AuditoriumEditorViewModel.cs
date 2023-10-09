using System;
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

namespace C868.Capstone.Core.ViewModels.Content.Auditoriums
{
    public class AuditoriumEditorViewModel : ContentViewModelBase
    {
        public string Title => CurrentAuditorium.Id == 0
            ? @"Create Auditorium"
            : @"Edit Auditorium";

        private AuditoriumViewModel currentAuditorium;
        public AuditoriumViewModel CurrentAuditorium
        {
            get => currentAuditorium;
            set
            {
                var newValue = value is null
                    ? new AuditoriumViewModel(new Auditorium())
                    : value.Clone();

                if (SetProperty(ref currentAuditorium, newValue))
                {
                    OnPropertyChanged(nameof(Title));
                    ClearErrors();
                }
            }
        }

        private string nameError;
        public string NameError
        {
            get => nameError;
            set => SetProperty(ref nameError, value);
        }

        private string capacityError;
        public string CapacityError
        {
            get => capacityError;
            set => SetProperty(ref capacityError, value);
        }

        public ICommand SaveCommand { get; private set; }
        public ICommand ClearCommand { get; private set; }

        public AuditoriumEditorViewModel(IDataService dataService, IDialogService dialogService,
            ILoggingService loggingService)
            : base(dataService, dialogService, loggingService)
        {
            CurrentAuditorium = new AuditoriumViewModel(new Auditorium());
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
                await SaveAuditorium();
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
            CurrentAuditorium = new AuditoriumViewModel(new Auditorium());
            ClearErrors();
            Messenger.Send(new ClearSelectedAuditoriumMessage(true));
        }

        private async Task SaveAuditorium()
        {
            if (!IsAuditoriumValid())
            {
                return;
            }

            if (await DataService.SaveAuditoriumAsync(CurrentAuditorium.Auditorium))
            {
                LogSave(CurrentAuditorium.Name, @"Auditorium");

                Messenger.Send(new AuditoriumSavedMessage(CurrentAuditorium));
                CurrentAuditorium = new AuditoriumViewModel(new Auditorium());
            }
            else
            {
                throw new InvalidOperationException(AppSettings.Errors.Data.SaveMessage);
            }
        }

        protected override void OnActivated()
        {
            Messenger.Register<AuditoriumEditorViewModel, SelectedAuditoriumChangedMessage>(this, (receiver, message) => receiver.Receive(message));
        }

        private void Receive(SelectedAuditoriumChangedMessage message)
        {
            CurrentAuditorium = message.Value;
        }

        private bool IsAuditoriumValid()
        {
            var isNameValid = IsNameValid();
            var isCapacityValid = IsCapacityValid();

            return isNameValid && isCapacityValid;
        }

        private bool IsNameValid()
        {
            NameError = string.Empty;

            if (string.IsNullOrWhiteSpace(CurrentAuditorium.Name))
            {
                NameError = AppSettings.ValidationErrors.Auditorium.NameBlank;
                return false;
            }
            return true;
        }

        private bool IsCapacityValid()
        {
            CapacityError = string.Empty;

            if (CurrentAuditorium.Capacity < 1)
            {
                CapacityError = AppSettings.ValidationErrors.Auditorium.CapacityLessThanOne;
                return false;
            }
            return true;
        }

        private void ClearErrors()
        {
            NameError = string.Empty;
            CapacityError = string.Empty;
        }
    }
}