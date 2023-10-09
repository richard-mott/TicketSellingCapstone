using System;
using System.Collections.Generic;
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

namespace C868.Capstone.Core.ViewModels.Content.TicketTypes
{
    public class TicketTypeEditorViewModel : ContentViewModelBase
    {
        public List<TicketTimeType> TicketTimeTypes => new List<TicketTimeType>
        {
            TicketTimeType.Before, TicketTimeType.During, TicketTimeType.After
        };

        public string Title => CurrentTicketType.Id == 0
            ? "Create Ticket Type"
            : "Edit Ticket Type";

        private TicketTypeViewModel currentTicketType;
        public TicketTypeViewModel CurrentTicketType
        {
            get => currentTicketType;
            set
            {
                var newValue = value is null
                    ? new TicketTypeViewModel(new TicketType())
                    : value.Clone();

                if (SetProperty(ref currentTicketType, newValue))
                {
                    OnPropertyChanged(nameof(Title));
                    SetRatings();
                    ClearErrors();
                }
            }
        }

        public List<Rating> CurrentRatings { get; private set; }
        public List<Rating> AvailableRatings { get; private set; }

        private Rating? selectedCurrentRating;
        public Rating? SelectedCurrentRating
        {
            get => selectedCurrentRating;
            set
            {
                if (SetProperty(ref selectedCurrentRating, value))
                {
                    ((RelayCommand)RemoveRatingCommand).NotifyCanExecuteChanged();
                }
            }
        }

        private Rating? selectedAvailableRating;
        public Rating? SelectedAvailableRating
        {
            get => selectedAvailableRating;
            set
            {
                if (SetProperty(ref selectedAvailableRating, value))
                {
                    ((RelayCommand)AddRatingCommand).NotifyCanExecuteChanged();
                }
            }
        }

        private string nameError;
        public string NameError
        {
            get => nameError;
            set => SetProperty(ref nameError, value);
        }

        private string priceError;
        public string PriceError
        {
            get => priceError;
            set => SetProperty(ref priceError, value);
        }

        private string ratingsError;
        public string RatingsError
        {
            get => ratingsError;
            set => SetProperty(ref ratingsError, value);
        }

        private string timeTypeError;
        public string TimeTypeError
        {
            get => timeTypeError;
            set => SetProperty(ref timeTypeError, value);
        }

        private string timeError;
        public string TimeError
        {
            get => timeError;
            set => SetProperty(ref timeError, value);
        }

        public ICommand AddRatingCommand { get; private set; }
        public ICommand RemoveRatingCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand ClearCommand { get; private set; }

        public TicketTypeEditorViewModel(IDataService dataService, IDialogService dialogService,
            ILoggingService loggingService)
            : base(dataService, dialogService, loggingService)
        {
            CurrentTicketType = new TicketTypeViewModel(new TicketType());

            IsActive = true;
            InitializeCommands();
            SetRatings();
        }

        private void InitializeCommands()
        {
            AddRatingCommand = new RelayCommand(ExecuteAddRatingCommand,
                () => SelectedAvailableRating != null);
            RemoveRatingCommand = new RelayCommand(ExecuteRemoveRatingCommand,
                () => SelectedCurrentRating != null);
            SaveCommand = new RelayCommand(ExecuteSaveCommand);
            ClearCommand = new RelayCommand(ExecuteClearCommand);
        }

        private void SetRatings()
        {
            CurrentRatings = new List<Rating>();
            AvailableRatings = new List<Rating>();

            foreach (var rating in Enum.GetValues(typeof(Rating)))
            {
                if ((Rating)rating != Rating.None)
                {
                    ProcessRating((Rating)rating);
                }
            }

            OnPropertyChanged(nameof(CurrentRatings));
            OnPropertyChanged(nameof(AvailableRatings));
        }

        private void ProcessRating(Rating rating)
        {
            if ((CurrentTicketType.Ratings & rating) == rating)
            {
                CurrentRatings.Add(rating);
            }
            else
            {
                AvailableRatings.Add(rating);
            }
        }

        private void ExecuteAddRatingCommand()
        {
            if (SelectedAvailableRating is null)
            {
                return;
            }

            CurrentTicketType.Ratings |= (Rating)SelectedAvailableRating;
            SelectedAvailableRating = null;
            SetRatings();
        }

        private void ExecuteRemoveRatingCommand()
        {
            if (SelectedCurrentRating is null)
            {
                return;
            }

            CurrentTicketType.Ratings ^= (Rating)SelectedCurrentRating;
            SelectedCurrentRating = null;
            SetRatings();
        }

        private async void ExecuteSaveCommand()
        {
            try
            {
                await SaveTicketType();
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
            CurrentTicketType = new TicketTypeViewModel(new TicketType());

            ClearErrors();

            Messenger.Send(new ClearSelectedTicketTypeMessage(true));
        }

        private async Task SaveTicketType()
        {

            if (!IsTicketTypeValid())
            {
                return;
            }

            if (await DataService.SaveTicketTypeAsync(CurrentTicketType.TicketType))
            {
                LogSave(CurrentTicketType.Name, @"Ticket Type");

                Messenger.Send(new TicketTypeSavedMessage(CurrentTicketType));
                CurrentTicketType = new TicketTypeViewModel(new TicketType());
            }
            else
            {
                throw new InvalidOperationException(AppSettings.Errors.Data.SaveMessage);
            }
        }

        protected override void OnActivated()
        {
            Messenger.Register<TicketTypeEditorViewModel, SelectedTicketTypeChangedMessage>(this,
                (receiver, message) => receiver.Receive(message));
        }

        private void Receive(SelectedTicketTypeChangedMessage message)
        {
            CurrentTicketType = message.Value;
        }

        private bool IsTicketTypeValid()
        {
            var isNameValid = IsNameValid();
            var isPriceValid = IsPriceValid();
            var isTimeTypeValid = IsTimeTypeValid();
            var isTimeValid = IsTimeValid();
            var areRatingsValid = AreRatingsValid();

            return isNameValid && isPriceValid && isTimeTypeValid &&
                   isTimeValid && areRatingsValid;
        }

        private bool IsNameValid()
        {
            NameError = string.Empty;

            if (string.IsNullOrWhiteSpace(CurrentTicketType.Name))
            {
                NameError = AppSettings.ValidationErrors.TicketType.NameBlank;
                return false;
            }

            return true;
        }

        private bool IsPriceValid()
        {
            PriceError = string.Empty;

            if (CurrentTicketType.Price <= 0.0d)
            {
                PriceError = AppSettings.ValidationErrors.TicketType.PriceBelowZero;
                return false;
            }

            return true;
        }

        private bool IsTimeTypeValid()
        {
            TimeTypeError = string.Empty;

            if (CurrentTicketType.TicketTimeType == TicketTimeType.None)
            {
                TimeTypeError = AppSettings.ValidationErrors.TicketType.TimeTypeBlank;
                return false;
            }

            return true;
        }

        private bool IsTimeValid()
        {
            TimeError = string.Empty;

            // Only need to validate if both times are set
            if (CurrentTicketType.TicketTimeType != TicketTimeType.During)
            {
                return true;
            }

            if (CurrentTicketType.StartTime < CurrentTicketType.EndTime)
            {
                return true;
            }

            TimeError = AppSettings.ValidationErrors.TicketType.StartTimeAfterEnd;
            return false;

        }

        private bool AreRatingsValid()
        {
            if (CurrentRatings.Count == 0)
            {
                RatingsError = AppSettings.ValidationErrors.TicketType.RatingsBlank;
                return false;
            }

            return true;
        }
        
        private void ClearErrors()
        {
            NameError = string.Empty;
            PriceError = string.Empty;
            TimeTypeError = string.Empty;
            TimeError = string.Empty;
            RatingsError = string.Empty;
        }
    }
}