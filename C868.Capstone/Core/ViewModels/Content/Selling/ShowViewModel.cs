using C868.Capstone.Core.Models.Data;
using C868.Capstone.Core.ViewModels.Data;

namespace C868.Capstone.Core.ViewModels.Content.Selling
{
    public class ShowViewModel : ShowTimeViewModel
    {
        private int ticketCount;
        public int TicketCount
        {
            get => ticketCount;
            set
            {
                if (SetProperty(ref ticketCount, value))
                {
                    OnPropertyChanged(nameof(AvailableSeating));
                    OnPropertyChanged(nameof(AvailableFraction));
                }
            }
        }

        public string AvailableSeating =>
            $"Available Seating: {Auditorium.Capacity - TicketCount}";

        public double AvailableFraction =>
            (Auditorium.Capacity - TicketCount) / (double)Auditorium.Capacity;

        public ShowViewModel(ShowTime showTime) : base(showTime)
        {
        }
    }
}