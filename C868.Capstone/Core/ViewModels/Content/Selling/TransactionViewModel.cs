using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using C868.Capstone.Core.Messages;
using C868.Capstone.Core.ViewModels.Data;
using C868.Capstone.Services;
using C868.Capstone.Services.Data;
using C868.Capstone.Services.Logging;
using C868.Capstone.Services.Payment;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace C868.Capstone.Core.ViewModels.Content.Selling
{
    public class TransactionViewModel : ContentViewModelBase
    {
        private readonly IPaymentProcessor cashProcessor;
        private readonly IPaymentProcessor creditProcessor;
        private readonly IPaymentProcessor checkProcessor;

        private readonly IServiceProvider services = App.Current.Services;

        private ObservableCollection<TicketViewModel> tickets;
        public ObservableCollection<TicketViewModel> Tickets
        {
            get => tickets;
            set => SetProperty(ref tickets, value);
        }

        public double Total => tickets.Sum(ticket => ticket.TotalPrice);

        public ICommand RemoveTicketCommand { get; private set; }
        public ICommand PayCashCommand { get; private set; }
        public ICommand PayCreditCommand { get; private set; }
        public ICommand PayCheckCommand { get; private set; }

        public TransactionViewModel(IDataService dataService, IDialogService dialogService,
            ILoggingService loggingService)
            : base(dataService, dialogService, loggingService)
        {
            Tickets = new ObservableCollection<TicketViewModel>();

            var paymentResolver = services.GetService<App.PaymentProcessorResolver>();
            cashProcessor = paymentResolver(PaymentType.Cash);
            creditProcessor = paymentResolver(PaymentType.Credit);
            checkProcessor = paymentResolver(PaymentType.Check);

            IsActive = true;
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            RemoveTicketCommand = new RelayCommand<object>(ExecuteRemoveTicketCommand);
            PayCashCommand = new RelayCommand(ExecutePayCashCommand, () => Tickets.Count > 0);
            PayCreditCommand = new RelayCommand(ExecutePayCreditCommand, () => Tickets.Count > 0);
            PayCheckCommand = new RelayCommand(ExecutePayCheckCommand, () => Tickets.Count > 0);
        }

        private void ExecuteRemoveTicketCommand(object param)
        {
            MouseButtonEventArgs args = param as MouseButtonEventArgs;

            if (!(args?.Source is FrameworkElement element))
            {
                return;
            }

            if (!(element.DataContext is TicketViewModel ticketViewModel))
            {
                return;
            }

            ticketViewModel.Count--;
            OnPropertyChanged(nameof(Total));

            if (ticketViewModel.Count == 0)
            {
                Tickets.Remove(ticketViewModel);
                RefreshCanExecute();
            }
        }

        private async void ExecutePayCashCommand()
        {
            if (cashProcessor.ProcessPayment(Total))
            {
                await ProcessSuccessfulPayment(PaymentType.Cash);
            }
        }

        private async void ExecutePayCreditCommand()
        {
            if (creditProcessor.ProcessPayment(Total))
            {
                await ProcessSuccessfulPayment(PaymentType.Credit);
            }
        }

        private async void ExecutePayCheckCommand()
        {
            if (checkProcessor.ProcessPayment(Total))
            {
                await ProcessSuccessfulPayment(PaymentType.Check);
            }
        }

        private async Task ProcessSuccessfulPayment(PaymentType payment)
        {
            try
            {
                foreach (var ticket in Tickets)
                {
                    ticket.Payment = payment;
                    await DataService.SaveTicketAsync(ticket.Ticket);
                }

                var showTicketCounts = Tickets
                    .Select(ticket => ticket.ShowTime.ShowTimeId)
                    .Distinct()
                    .ToList()
                    .ToDictionary(
                        showId => showId,
                        showId => Tickets
                            .Where(ticket => ticket.ShowTime.ShowTimeId == showId)
                            .Sum(ticket => ticket.Count));

                Messenger.Send(new TransactionCompleteMessage(showTicketCounts));
                Tickets.Clear();
                RefreshCanExecute();
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

        private void RefreshCanExecute()
        {
            ((IRelayCommand)PayCashCommand).NotifyCanExecuteChanged();
            ((IRelayCommand)PayCreditCommand).NotifyCanExecuteChanged();
            ((IRelayCommand)PayCheckCommand).NotifyCanExecuteChanged();
        }

        protected override void OnActivated()
        {
            Messenger.Register<TransactionViewModel, TicketAddedMessage>(this,
                (recipient, message) => recipient.Receive(message));
        }

        private void Receive(TicketAddedMessage message)
        {
            var newTicket = message.Value;
            var foundTicket = tickets.FirstOrDefault(
                ticket => IsTicketMatching(ticket, newTicket));

            if (foundTicket is null)
            {
                tickets.Add(newTicket);
                OnPropertyChanged(nameof(Total));
                RefreshCanExecute();
                return;
            }

            foundTicket.Count += newTicket.Count;
            OnPropertyChanged(nameof(Total));
            RefreshCanExecute();
        }

        private bool IsTicketMatching(TicketViewModel ticket, TicketViewModel compareTicket)
        {
            return ticket.ShowTime.ShowTimeId == compareTicket.ShowTime.ShowTimeId &&
                   ticket.TicketType.TicketTypeId == compareTicket.TicketType.TicketTypeId;
        }
    }
}