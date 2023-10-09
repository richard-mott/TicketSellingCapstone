using System.Windows;
using System.Windows.Controls;
using C868.Capstone.Core.Messages;
using C868.Capstone.Core.ViewModels.Content.Users;
using CommunityToolkit.Mvvm.Messaging;

namespace C868.Capstone.Core.Views.Content.Users
{
    public partial class UserEditorView : UserControl
    {
        public UserEditorView()
        {
            InitializeComponent();

            WeakReferenceMessenger.Default.Register<UserEditorView, ClearPasswordsMessage>(this,
                (receiver, messasge) =>
                {
                    receiver.CurrentPasswordInput.Clear();
                    receiver.NewPasswordInput.Clear();
                    receiver.ConfirmPasswordInput.Clear();
                });
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Set the password in the ViewModel
            var viewModel = (UserEditorViewModel)DataContext;

            viewModel.CurrentPassword = CurrentPasswordInput.Password;
            viewModel.NewPassword = NewPasswordInput.Password;
            viewModel.ConfirmPassword = ConfirmPasswordInput.Password;
        }
    }
}
