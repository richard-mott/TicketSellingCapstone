using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using C868.Capstone.Core.Messages;
using C868.Capstone.Core.Models.Data;
using C868.Capstone.Core.ViewModels.Controls;
using C868.Capstone.Core.ViewModels.Data;
using CommunityToolkit.Mvvm.Messaging;

namespace C868.Capstone.Core.Views.Controls
{
    public partial class ScheduleViewer : UserControl
    {
        #region Private Fields

        private ScheduleViewerViewModel viewModel;
        private ShowTimeDisplay selectedShowTime = null;

        #endregion

        #region Constructors

        public ScheduleViewer()
        {
            InitializeComponent();
        }

        #endregion

        #region Event Handlers

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            viewModel = DataContext as ScheduleViewerViewModel;
            
            if (viewModel is null)
            {
                return;
            }
            
            viewModel.DataViewChanged += OnDataViewChanged;
            DrawSchedule();

            WeakReferenceMessenger.Default.Register<ScheduleViewer, SelectedMovieChangedMessage>(this,
                (receiver, message) => receiver.Receive(message));
        }

        private void Receive(SelectedMovieChangedMessage message)
        {
            if (selectedShowTime is null)
            {
                return;
            }

            selectedShowTime.Background = (Brush)FindResource(AppSettings.Schedule.ShowTimeBackground);
            selectedShowTime = null;
        }

        private void OnDataViewChanged(object sender, EventArgs e)
        {
            DrawSchedule();
        }
        
        #endregion

        #region Rendering Methods

        private void DrawSchedule()
        {
            // Clear all existing children before drawing the schedule
            Schedule.Children.Clear();

            DrawTimes();
            DrawAuditoriums();
            DrawGrid();
            DrawShowTimes();
            SetCanvasSize();
        }

        private void SetCanvasSize()
        {
            var canvasSize = VisualTreeHelper.GetDescendantBounds(Schedule);

            Schedule.Height = Math.Max(canvasSize.Height, viewModel.RequestedCanvasHeight);
            Schedule.Width = Math.Max(canvasSize.Width, viewModel.RequestedCanvasWidth);
        }

        private void DrawTimes()
        {
            for (var hour = 0; hour < viewModel.TimeColumnCount; hour++)
            {
                var topTime = new TimeDisplay(viewModel.StartTime, hour, xOffset: hour);
                Schedule.Children.Add(topTime);

                var bottomTime = new TimeDisplay(viewModel.StartTime, hour,
                    xOffset: hour, yOffset: viewModel.RowCount - 1);
                Schedule.Children.Add(bottomTime);
            }
        }

        private void DrawAuditoriums()
        {
            var topTitleBlock = new AuditoriumDisplay(
                new AuditoriumViewModel(
                    new Auditorium { Name = AppSettings.Schedule.AuditoriumTitle }),
                yOffset: 0d)
            {
                Background = (Brush)FindResource(AppSettings.Schedule.HeaderBackground)
            };

            Schedule.Children.Add(topTitleBlock);

            // Add one to index to offset for the top title block
            var auditoriums =
                viewModel.Auditoriums
                    .Select((auditorium, index) =>
                        new AuditoriumDisplay(
                            auditorium,
                            yOffset: index + 1,
                            textIndent: TextIndent.Left));

            foreach (var auditorium in auditoriums)
            {
                Schedule.Children.Add(auditorium);
            }

            // Add one to Count to offset for the top title block
            var bottomTitleBlock = new AuditoriumDisplay(
                new AuditoriumViewModel(
                    new Auditorium { Name = AppSettings.Schedule.AuditoriumTitle }),
                yOffset: viewModel.Auditoriums.Count + 1)
            {
                Background = (Brush)FindResource(AppSettings.Schedule.HeaderBackground)
            };
            Schedule.Children.Add(bottomTitleBlock);
        }

        private void DrawGrid()
        {
            // Horizontal separators
            for (var index = 1; index <= viewModel.RowCount; index++)
            {
                var separator = new ScheduleSeparator(viewModel.RequestedCanvasWidth, 0, index);
                Schedule.Children.Add(separator);
            }

            // Vertical separators
            for (var index = 0; index <= viewModel.TimeColumnCount; index++)
            {
                var separator = new ScheduleSeparator(viewModel.RequestedCanvasHeight, index, 0,
                    SeparatorOrientation.Vertical);
                Schedule.Children.Add(separator);
            }
        }

        private void DrawShowTimes()
        {
            for (var index = 0; index < viewModel.Auditoriums.Count; index++)
            {
                var auditorium = viewModel.Auditoriums[index];
                
                foreach (var showTime in viewModel.AuditoriumShowTimes[auditorium.Id])
                {
                    if (showTime.StartTime < viewModel.StartTime || showTime.EndTime > viewModel.EndTime.AddHours(1))
                    {
                        continue;
                    }

                    var showTimeDisplay = new ShowTimeDisplay(showTime, index, viewModel.StartTime.Hour);

                    showTimeDisplay.MouseLeftButtonDown += OnShowTimeClick;
                    Schedule.Children.Add(showTimeDisplay);
                }
            }
        }

        #endregion

        private void OnShowTimeClick(object sender, MouseButtonEventArgs e)
        {
            var showTime = sender as ShowTimeDisplay;

            if (showTime is null)
            {
                return;
            }

            if (selectedShowTime != null)
            {
                if (showTime.ShowTime.Id == selectedShowTime.ShowTime.Id)
                {
                    return;
                }

                selectedShowTime.Background = (Brush)FindResource(AppSettings.Schedule.ShowTimeBackground);
            }

            showTime.Background = (Brush)FindResource(AppSettings.Schedule.SelectedShowTimeBackground);
            selectedShowTime = showTime;

            WeakReferenceMessenger.Default.Send(new SelectedShowTimeChangedMessage(selectedShowTime.ShowTime));
        }
    }
}
