using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace C868.Capstone.Core.ViewModels.Dialogs
{
    public class ConfirmDialogViewModel : DialogViewModel
    {
        private string message;

        public string Message
        {
            get => message;
            set => SetProperty(ref message, value);
        }

        public ICommand YesCommand { get; private set; }
        public ICommand NoCommand { get; private set; }

        public ConfirmDialogViewModel(string title, string message)
        {
            Title = title;
            Message = message;

            YesCommand = new RelayCommand(ExecuteYesCommand);
            NoCommand = new RelayCommand(ExecuteNoCommand);
        }

        private void ExecuteYesCommand()
        {
            base.ExecuteOKCommand();
        }

        private void ExecuteNoCommand()
        {
            base.ExecuteCancelCommand();
        }
    }
}