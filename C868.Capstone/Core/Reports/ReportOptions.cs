using System;
using System.Collections.Generic;
using C868.Capstone.Core.ViewModels.Data;
using CommunityToolkit.Mvvm.ComponentModel;

namespace C868.Capstone.Core.Reports
{
    public class ReportOptions : ObservableObject
    {
        public bool RequiresMovie { get; }
        public bool RequiresDate { get; }
        public bool RequiresDateRange { get; }
        public bool RequiresLogMessageTypes { get; }

        private List<MovieViewModel> movies;
        public List<MovieViewModel> Movies
        {
            get => movies;
            set => SetProperty(ref movies, value);
        }

        public MovieViewModel Movie { get; set; }
        public DateTime Date { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IncludeInfo { get; set; }
        public bool IncludeWarnings { get; set; }
        public bool IncludeErrors { get; set; }

        public ReportOptions(
            bool requiresMovie = false,
            bool requiresDate = false,
            bool requiresDateRange = false,
            bool requiresLogMessageTypes = false)
        {
            RequiresMovie = requiresMovie;
            RequiresDate = requiresDate;
            RequiresDateRange = requiresDateRange;
            RequiresLogMessageTypes = requiresLogMessageTypes;

            Movie = null;
            Date = DateTime.Today;
            StartDate = DateTime.Today;
            EndDate = DateTime.Today;
            IncludeInfo = true;
            IncludeWarnings = true;
            IncludeErrors = true;
        }
    }
}