using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace C868.Capstone.Core.Views.Controls
{
    public partial class ScheduleSeparator : Separator
    {
        public ScheduleSeparator(double size, double xOffset, double yOffset,
            SeparatorOrientation orientation = SeparatorOrientation.Horizontal)
        {
            InitializeComponent();

            var layout = new SeparatorLayout(size, xOffset, yOffset, orientation);

            Height = layout.Height;
            Width = layout.Width;
            Opacity = AppSettings.Schedule.SeparatorOpacity;

            Style = (Style)FindResource(ToolBar.SeparatorStyleKey);
            BorderBrush = (Brush)FindResource(AppSettings.Schedule.SeparatorBorderBrush);
            BorderThickness = (Thickness)FindResource(AppSettings.Schedule.BorderThicknessSmall);
            Margin = (Thickness)FindResource(AppSettings.Schedule.MarginNone);
            Padding = (Thickness)FindResource(AppSettings.Schedule.PaddingNone);

            Canvas.SetTop(this, layout.Top);
            Canvas.SetLeft(this, layout.Left);
        }
    }

    internal class SeparatorLayout
    {
        private const double DefaultX = 0d;
        public const double DefaultY = 0d;
        public const double DefaultSize = 2d;

        public double Top { get; }
        public double Left { get; }
        public double Height { get; }
        public double Width { get; }

        internal SeparatorLayout(double size, double xOffset, double yOffset,
            SeparatorOrientation orientation)
        {
            Top = orientation == SeparatorOrientation.Horizontal
                ? yOffset * AppSettings.Schedule.RowHeight - 1
                : DefaultY;

            Left = orientation == SeparatorOrientation.Horizontal
                ? DefaultX
                : xOffset * AppSettings.Schedule.TimeWidth + AppSettings.Schedule.AuditoriumWidth - 1;

            Height = orientation == SeparatorOrientation.Horizontal
                ? DefaultSize
                : size;

            Width = orientation == SeparatorOrientation.Horizontal
                ? size
                : DefaultSize;
        }
    }
}
