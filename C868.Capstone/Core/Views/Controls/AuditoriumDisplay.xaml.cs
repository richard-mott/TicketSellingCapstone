using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using C868.Capstone.Core.ViewModels.Data;

namespace C868.Capstone.Core.Views.Controls
{
    public partial class AuditoriumDisplay : TextBlock
    {
        public AuditoriumDisplay(AuditoriumViewModel auditorium, double xOffset = 0d,
            double yOffset = 0d, TextIndent textIndent = TextIndent.None)
        {
            InitializeComponent();
            var layout = new AuditoriumLayout(auditorium, xOffset, yOffset, textIndent);

            Text = layout.Caption;
            Height = layout.Height;
            Width = layout.Width;
            Padding = layout.Padding;

            FontSize = (double)FindResource(AppSettings.Schedule.FontSizeNormal);
            Margin = (Thickness)FindResource(AppSettings.Schedule.MarginNone);
            Background = (Brush)FindResource(AppSettings.Schedule.AuditoriumBackground);

            Canvas.SetTop(this, layout.Top);
            Canvas.SetLeft(this, layout.Left);
        }
    }

    internal class AuditoriumLayout
    {
        private readonly FrameworkElement element = new FrameworkElement();

        public string Caption { get; }
        public double Top { get; }
        public double Left { get; }
        public double Height { get; }
        public double Width { get; }
        public Thickness Padding { get; }

        internal AuditoriumLayout(AuditoriumViewModel auditorium, double xOffset, double yOffset,
            TextIndent textIndent)
        {
            Caption = auditorium.Name;
            Top = yOffset * AppSettings.Schedule.RowHeight;
            Left = xOffset * AppSettings.Schedule.AuditoriumWidth;
            Height = AppSettings.Schedule.RowHeight;
            Width = AppSettings.Schedule.AuditoriumWidth;
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
