using C868.Capstone.Core.Models.Data;
using CommunityToolkit.Mvvm.ComponentModel;

namespace C868.Capstone.Core.ViewModels.Data
{
    public class TicketViewModel : ObservableObject
    {
        private readonly Ticket ticket;
        public Ticket Ticket => ticket ?? new Ticket();
        public int Id => Ticket.TicketId;

        public int Count
        {
            get => Ticket.Count;
            set
            {
                if (SetProperty(Ticket.Count, value, Ticket,
                        (tick, count) => tick.Count = count))
                {
                    OnPropertyChanged(nameof(TotalPrice));
                }
            }
        }

        public PaymentType Payment
        {
            get => Ticket.Payment;
            set => SetProperty(Ticket.Payment, value, Ticket,
                (tick, payment) => tick.Payment = payment);
        }

        public TicketType TicketType
        {
            get => Ticket.TicketType;
            set => SetProperty(Ticket.TicketType, value, Ticket,
                (tick, ticketType) => tick.TicketType = ticketType);
        }

        public ShowTime ShowTime
        {
            get => Ticket.ShowTime;
            set => SetProperty(Ticket.ShowTime, value, Ticket,
                (tick, showTime) => tick.ShowTime = showTime);
        }

        public double TotalPrice => Count * TicketType.Price;

        public TicketViewModel(Ticket ticket)
        {
            this.ticket = ticket;
        }
    }
}