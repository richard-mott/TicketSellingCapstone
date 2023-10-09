using System;
using C868.Capstone.Core.Models.Data;
using CommunityToolkit.Mvvm.ComponentModel;

namespace C868.Capstone.Core.ViewModels.Data
{
    public class ShowTimeViewModel : ObservableObject
    {
        private readonly ShowTime showTime;
        public ShowTime ShowTime => showTime ?? new ShowTime();
        public int Id => ShowTime.ShowTimeId;

        public DateTime StartTime
        {
            get => ShowTime.StartTime;
            set
            {
                if (SetProperty(ShowTime.StartTime, value, ShowTime,
                        (show, startTime) => show.StartTime = startTime))
                {
                    OnPropertyChanged(nameof(EndTime));
                }
            }
        }

        public DateTime EndTime => ShowTime.EndTime;

        private AuditoriumViewModel auditorium;
        public AuditoriumViewModel Auditorium
        {
            get => auditorium;
            set
            {
                if (SetProperty(ref auditorium, value))
                {
                    ShowTime.Auditorium = value.Auditorium;
                    ShowTime.AuditoriumId = value.Auditorium is null ? 0 : value.Id;
                };
            }
        }

        private MovieViewModel movie;
        public MovieViewModel Movie
        {
            get => movie;
            set
            {
                if (SetProperty(ref movie, value))
                {
                    ShowTime.Movie = value.Movie;
                    ShowTime.MovieId = value.Movie is null ? 0 : value.Id;
                }
            }
        }

        public ShowTimeViewModel(ShowTime showTime)
        {
            this.showTime = showTime;

            Auditorium = new AuditoriumViewModel(showTime.Auditorium);
            Movie = new MovieViewModel(showTime.Movie);
        }
    }
}