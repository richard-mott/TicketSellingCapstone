using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using C868.Capstone.Core.Messages;
using C868.Capstone.Core.ViewModels.Data;
using C868.Capstone.Services.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace C868.Capstone.Core.ViewModels.Controls
{
    public class ScheduleViewerViewModel : ObservableRecipient
    {
        private readonly IDataService dataService;

        public List<ShowTimeViewModel> ShowTimes { get; set; } = new List<ShowTimeViewModel>();
        public List<AuditoriumViewModel> Auditoriums { get; set; } = new List<AuditoriumViewModel>();
        public Dictionary<int, List<ShowTimeViewModel>> AuditoriumShowTimes { get; set; } =
            new Dictionary<int, List<ShowTimeViewModel>>();

        public int RowCount { get; private set; }
        public double TimeColumnCount { get; private set; }

        public double RequestedCanvasHeight { get; private set;}
        public double RequestedCanvasWidth { get; private set; }

        private DateTime selectedDate = DateTime.Today;
        public DateTime SelectedDate
        {
            get => selectedDate;
            set
            {
                if (SetProperty(ref selectedDate, value))
                {
                    UpdateShowTimes();
                }
            }
        }

        private DateTime startTime;
        public DateTime StartTime
        {
            get => startTime;
            set
            {
                if (SetProperty(ref startTime, value))
                {
                    CalculateLayout();
                    OnDataViewChanged(EventArgs.Empty);
                }
            }
        }

        private DateTime endTime;
        public DateTime EndTime
        {
            get => endTime;
            set
            {
                if (endTime.Hour == 0 && value.Hour == 23)
                {
                    value = value.AddDays(-1);
                }

                if (endTime.Hour == 23 && value.Hour == 0)
                {
                    value = value.AddDays(1);
                }

                if (SetProperty(ref endTime, value))
                {
                    CalculateLayout();
                    OnDataViewChanged(EventArgs.Empty);
                }
            }
        }

        public event EventHandler DataViewChanged;
        
        public ScheduleViewerViewModel(IDataService dataService)
        {
            this.dataService = dataService;
            IsActive = true;
        }

        public async Task InitializeData()
        {
            await LoadShowTimes();
            OnDataViewChanged(EventArgs.Empty);
        }

        private async Task LoadShowTimes()
        {
            ShowTimes = await GetShowTimes(SelectedDate);
            Auditoriums = await GetAuditoriums();
            AuditoriumShowTimes = GetMappedShowTimes();

            CalculateTimes();
            CalculateLayout();
        }

        private void UpdateShowTimes()
        {
            var loadTask = Task.Run(async () => await LoadShowTimes());
            var continueTask = Task.WhenAll(loadTask);

            continueTask.Wait();
            if (continueTask.Status == TaskStatus.RanToCompletion)
            {
                OnDataViewChanged(EventArgs.Empty);
            }
        }
        private async Task<List<ShowTimeViewModel>> GetShowTimes(DateTime date)
        {
            return (await dataService.GetShowTimesAsync(date))
                .Select(showTime => new ShowTimeViewModel(showTime))
                .ToList();
        }

        private async Task<List<AuditoriumViewModel>> GetAuditoriums()
        {
            return (await dataService.GetAuditoriumsAsync())
                .Select(auditorium => new AuditoriumViewModel(auditorium))
                .ToList();
        }

        private Dictionary<int, List<ShowTimeViewModel>> GetMappedShowTimes()
        {
            var mappedShowTimes = new Dictionary<int, List<ShowTimeViewModel>>();
            foreach (var auditorium in Auditoriums)
            {
                var showTimes = ShowTimes
                    .Where(showTime => showTime.Auditorium.Id == auditorium.Id)
                    .ToList();

                mappedShowTimes.Add(auditorium.Id, showTimes);
            }

            return mappedShowTimes;
        }

        private void CalculateTimes()
        {
            var defaultStartTime = DateTime.Today.AddHours(10);
            var defaultEndTime = DateTime.Today.AddHours(22);

            startTime = ShowTimes
                .Select(showTime => showTime.StartTime)
                .DefaultIfEmpty(defaultStartTime)
                .Min();

            // Ensure at least one empty hour before the Start Time
            startTime = startTime.AddHours(-1).AddMinutes(-1 * startTime.Minute);

            endTime = ShowTimes
                .Select(showTime => showTime.EndTime)
                .DefaultIfEmpty(defaultEndTime)
                .Max();
            
            // Ensure at least one empty hour after the End Time
            endTime = endTime.AddHours(1).AddMinutes(-1 * endTime.Minute);

            // Raise property changed events
            OnPropertyChanged(nameof(StartTime));
            OnPropertyChanged(nameof(EndTime));
        }

        private void CalculateLayout()
        {
            TimeColumnCount = (EndTime - StartTime).TotalHours + 1;
            RowCount = Auditoriums.Count + 2;
            RequestedCanvasHeight = RowCount * AppSettings.Schedule.RowHeight;
            RequestedCanvasWidth = TimeColumnCount * AppSettings.Schedule.TimeWidth +
                                   AppSettings.Schedule.AuditoriumWidth;
        }

        private void OnDataViewChanged(EventArgs e)
        {
            DataViewChanged?.Invoke(this, e);
        }

        protected override void OnActivated()
        {
            Messenger.Register<ScheduleViewerViewModel, ShowTimeSavedMessage>(this,
                (recipient, message) => recipient.Receive(message));
            Messenger.Register<ScheduleViewerViewModel, ShowTimeDeletedMessage>(this,
                (recipient, message) => recipient.Receive(message));
        }

        private async Task Receive(ShowTimeSavedMessage message)
        {
            await LoadShowTimes();
            OnDataViewChanged(EventArgs.Empty);
        }

        private async Task Receive(ShowTimeDeletedMessage message)
        {
            await LoadShowTimes();
            OnDataViewChanged(EventArgs.Empty);
        }
    }
}