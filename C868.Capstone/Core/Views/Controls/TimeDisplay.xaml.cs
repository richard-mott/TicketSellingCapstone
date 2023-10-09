using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace C868.Capstone.Core.Views.Controls
{
    public partial class TimeDisplay : TextBlock
    {
        public TimeDisplay(DateTime startTime, int hourOffset, double xOffset = 0d,
            double yOffset = 0d, TextIndent textIndent = TextIndent.None)
        {
            InitializeComponent();

            var layout = new TimeLayout(startTime, hourOffset, xOffset, yOffset, textIndent);
            
            Text = layout.Caption;
            Height = layout.Height;
            Width = layout.Width;
            Padding = layout.Padding;

            FontSize = (double)FindResource(AppSettings.Schedule.FontSizeNormal);
            Margin = (Thickness)FindResource(AppSettings.Schedule.MarginNone);
            Padding = (Thickness)FindResource(AppSettings.Schedule.PaddingNormal);
            Background = (Brush)FindResource(AppSettings.Schedule.HeaderBackground);

            Canvas.SetTop(this, layout.Top);
            Canvas.SetLeft(this, layout.Left);

            var name = $"TimeDisplay_{hourOffset}_{xOffset}_{yOffset}";
            Name = name;
            RegisterName(name, this);
        }
    }

    internal class TimeLayout
    {
        private readonly FrameworkElement element = new FrameworkElement();

        public string Caption { get; }

        public double Top { get; }
        public double Left { get; }
        public double Height { get; }
        public double Width { get; }

        public Thickness Padding { get; }
        
        internal TimeLayout(DateTime startTime, int hourOffset, double xOffset,
            double yOffset, TextIndent textIndent)
        {
            var time = startTime.AddHours(hourOffset);
            Caption = time.Hour % 24 == 0
                ? @"Midnight"
                : time.Hour % 12 == 0
                    ? @"Noon"
                    : time.ToString("htt");

            Top = yOffset * AppSettings.Schedule.RowHeight;
            Left = (xOffset * AppSettings.Schedule.TimeWidth) + AppSettings.Schedule.AuditoriumWidth;
            Height = AppSettings.Schedule.RowHeight;
            Width = AppSettings.Schedule.TimeWidth;
            Padding = GetPadding(textIndent);
        }

        private Thickness GetPadding(TextIndent textIndent)
        {
            switch (textIndent)
            {
                case TextIndent.Left:
                    return (Thickness)element.FindResource(AppSettings.Schedule.PaddingLeftIndent);
                case TextIndent.Right:
                    return (Thickness)element.FindResource(AppSettings.Schedule.PaddingRightIndent);
                case TextIndent.None:
                default:
                    return (Thickness)element.FindResource(AppSettings.Schedule.PaddingNormal);
            }
        }
    }
}
