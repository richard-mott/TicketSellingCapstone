using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace C868.Capstone.Core.Models.Data
{
    public class ShowTime
    {
        [PrimaryKey, AutoIncrement]
        public int ShowTimeId { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime Created { get; set; }
        
        [ForeignKey(typeof(Movie))]
        public int MovieId { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead)]
        public Movie Movie { get; set; }

        [ForeignKey(typeof(Auditorium))]
        public int AuditoriumId { get; set; }
        
        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead)]
        public Auditorium Auditorium { get; set; }
        
        [Ignore] public DateTime EndTime => StartTime.AddMinutes(Movie.RunTime);
        
        public ShowTime()
        {
            var now = DateTime.Now;

            ShowTimeId = 0;
            MovieId = 0;
            AuditoriumId = 0;

            // Zero out the minutes and seconds for the start time
            StartTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0);

            Created = DateTime.Now;
            Movie = null;
            Auditorium = null;
        }

        public ShowTime Clone()
        {
            return new ShowTime
            {
                ShowTimeId = ShowTimeId,
                MovieId = MovieId,
                AuditoriumId = AuditoriumId,
                
                StartTime = StartTime,
                Created = Created,
                Movie = Movie,
                Auditorium = Auditorium
            };
        }
    }
}
