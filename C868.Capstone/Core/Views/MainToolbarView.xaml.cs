using System.Windows;
using System.Windows.Controls;

namespace C868.Capstone.Core.Views
{
    public partial class MainToolbarView : UserControl
    {
        public MainToolbarView()
        {
            InitializeComponent();
        }

        private void ToolBar_OnLoaded(object sender, RoutedEventArgs e)
        {
            var toolBar = (ToolBar)sender;
            if (toolBar.Template.FindName("OverflowGrid", toolBar) is FrameworkElement overflowGrid)
            {
                overflowGrid.Visibility = Visibility.Collapsed;
            }

            if (toolBar.Template.FindName("MainPanelBorder", toolBar) is FrameworkElement mainPanelBorder)
            {
                mainPanelBorder.Margin = new Thickness(0);
            }
        }
    }
}
