using System.Windows;
using System.Windows.Controls;
using C868.Capstone.Core.ViewModels.Dialogs;

namespace C868.Capstone.Core.Views.Dialogs
{
    public partial class LogInDialogView : UserControl
    {
        public LogInDialogView()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // Set the password in the ViewModel
            var viewModel = (LogInDialogViewModel)DataContext;
            viewModel.Password = PasswordInput.Password;
        }
    }
}
