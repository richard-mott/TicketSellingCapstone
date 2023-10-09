using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using C868.Capstone.Core.ViewModels.Data;

namespace C868.Capstone.Core.Views.Controls
{
    public partial class ShowTimeDisplay : Border
    {
        public ShowTimeViewModel ShowTime { get; }

        public ShowTimeDisplay(ShowTimeViewModel showTime, int auditoriumIndex, int startHour)
        {
            InitializeComponent();

            ShowTime = showTime;

            var layout = new ShowTimeLayout(showTime, auditoriumIndex, startHour);
            var textBlock = new TextBlock
            {
                Text = layout.Caption,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = (Thickness)FindResource(AppSettings.Schedule.MarginNone),
                Padding = (Thickness)FindResource(AppSettings.Schedule.PaddingNormal)
            };

            Height = layout.Height;
            Width = layout.Width;

            BorderBrush = (Brush)FindResource(AppSettings.Schedule.ShowTimeBorderBrush);
            BorderThickness = (Thickness)FindResource(AppSettings.Schedule.BorderThicknessNormal);
            Background = (Brush)FindResource(AppSettings.Schedule.ShowTimeBackground);

            Child = textBlock;

            Canvas.SetTop(this, layout.Top);
            Canvas.SetLeft(this, layout.Left);
        }
    }

    internal class ShowTimeLayout
    {
        public string Caption { get; }
        public double Top { get; }
        public double Left { get; }
        public double Height { get; }
        public double Width { get; }

        internal ShowTimeLayout(ShowTimeViewModel showTime, int auditoriumIndex, int startHour)
        {
            Caption = showTime.Movie.Name;

            Top = (auditoriumIndex + 1) * AppSettings.Schedule.RowHeight - 1;

            var startTime = (DateTime)showTime.StartTime;
            var hourOffset = (startTime.Hour - startHour) * AppSettings.Schedule.TimeWidth +
                             AppSettings.Schedule.AuditoriumWidth;
            var minuteOffset = startTime.Minute / 60d * AppSettings.Schedule.TimeWidth;

            Left = hourOffset + minuteOffset - 1;

            Height = AppSettings.Schedule.RowHeight + 2;
            Width = showTime.Movie.RunTime / 60d * AppSettings.Schedule.TimeWidth;
        }
    }
}
