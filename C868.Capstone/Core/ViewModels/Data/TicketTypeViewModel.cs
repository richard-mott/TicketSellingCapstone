using System;
using C868.Capstone.Core.Models.Data;
using CommunityToolkit.Mvvm.ComponentModel;

namespace C868.Capstone.Core.ViewModels.Data
{
    public class TicketTypeViewModel : ObservableObject
    {
        private readonly TicketType ticketType;
        public TicketType TicketType => ticketType ?? new TicketType();
        public int Id => TicketType.TicketTypeId;

        public string Name
        {
            get => TicketType.Name;
            set => SetProperty(TicketType.Name, value, TicketType,
                (type, name) => type.Name = name);
        }

        public double Price
        {
            get => TicketType.Price;
            set => SetProperty(TicketType.Price, value, TicketType,
                (type, price) => type.Price = price);
        }

        public Rating Ratings
        {
            get => TicketType.Ratings;
            set => SetProperty(TicketType.Ratings, value, TicketType,
                (type, ratings) => type.Ratings = ratings);
        }

        public TicketTimeType? TicketTimeType
        {
            get => TicketType.TicketTimeType == Core.TicketTimeType.None
                ? (TicketTimeType?)null
                : TicketType.TicketTimeType;
            set
            {
                var result = SetProperty(TicketType.TicketTimeType,
                    value ?? Core.TicketTimeType.None, TicketType,
                    (type, ticketTimeType) =>
                        type.TicketTimeType = ticketTimeType);

                if (result)
                {
                    OnPropertyChanged(nameof(StartTime));
                    OnPropertyChanged(nameof(HasStartTime));

                    OnPropertyChanged(nameof(EndTime));
                    OnPropertyChanged(nameof(HasEndTime));
                }
            }
        }

        public DateTime StartTime
        {
            get => TicketType.StartTime;
            set => SetProperty(TicketType.StartTime, value, TicketType,
                (type, startTime) => type.StartTime = startTime);
        }

        public bool HasStartTime => TicketTimeType == Core.TicketTimeType.During ||
                                    TicketTimeType == Core.TicketTimeType.After;

        public bool HasEndTime => TicketTimeType == Core.TicketTimeType.Before ||
                                  TicketTimeType == Core.TicketTimeType.During;

        public DateTime EndTime
        {
            get => TicketType.EndTime;
            set => SetProperty(TicketType.EndTime, value, TicketType,
                (type, endTime) => type.EndTime = endTime);
        }

        public TicketTypeViewModel(TicketType ticketType)
        {
            this.ticketType = ticketType;
        }

        public bool IsAvailableForShowTime(ShowTime showTime)
        {
            return TicketType.IsAvailableForShowTime(showTime);
        }

        public TicketTypeViewModel Clone()
        {
            return new TicketTypeViewModel(TicketType.Clone());
        }
    }
}