using System.Windows;
using C868.Capstone.Core.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace C868.Capstone
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = App.Current.Services.GetService<MainViewModel>();
            InitializeComponent();
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            var dataContext = DataContext as MainViewModel;

            if (dataContext is null)
            {
                return;
            }

            await dataContext.InitializeData();
        }
    }
}
