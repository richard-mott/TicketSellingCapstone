using System;
using SQLite;

namespace C868.Capstone.Core.Models.Data
{
    public class TicketType
    {
        [PrimaryKey, AutoIncrement]
        public int TicketTypeId { get; set; }

        public string Name { get; set; }
        public double Price { get; set; }
        public Rating Ratings { get; set; }

        public TicketTimeType TicketTimeType { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime Created { get; set; }

        public TicketType()
        {
            var now = DateTime.Now;

            TicketTypeId = 0;

            Name = string.Empty;
            Price = 0.0d;
            Ratings = Rating.None;
            TicketTimeType = TicketTimeType.None;

            // Zero out the minutes and seconds for the start time
            StartTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0);

            // Zero out the minutes and seconds for the end time
            EndTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0);

            Created = now;
        }

        public bool IsAvailableForShowTime(ShowTime showTime)
        {
            var isRatingValid = (showTime.Movie.Rating & Ratings) == showTime.Movie.Rating;
            if (!isRatingValid)
            {
                return false;
            }

            var showStart = showTime.StartTime.TimeOfDay;
            switch (TicketTimeType)
            {
                case TicketTimeType.Before:
                    return showStart < EndTime.TimeOfDay;
                case TicketTimeType.During:
                    return showStart >= StartTime.TimeOfDay &&
                           showStart < EndTime.TimeOfDay;
                case TicketTimeType.After:
                    return showStart >= StartTime.TimeOfDay;
                case TicketTimeType.None:
                default:
                    return false;
            }
        }

        public TicketType Clone()
        {
            return new TicketType
            {
                TicketTypeId = TicketTypeId,

                Name = Name,
                Price = Price,
                Ratings = Ratings,
                TicketTimeType = TicketTimeType,
                StartTime = StartTime,
                EndTime = EndTime,
                Created = Created
            };
        }
    }
}
