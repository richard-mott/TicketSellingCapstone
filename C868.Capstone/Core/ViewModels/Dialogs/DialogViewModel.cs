using System.Windows.Input;
using C868.Capstone.Core.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace C868.Capstone.Core.ViewModels.Dialogs
{
    public class DialogViewModel : ObservableRecipient
    {
        private string title;
        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        public ICommand OKCommand { get; }
        public ICommand CancelCommand { get; }

        public DialogViewModel()
        {
            Title = "Default Title";
            OKCommand = new RelayCommand(ExecuteOKCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);
        }

        protected virtual void ExecuteOKCommand()
        {
            Messenger.Send(new DialogResultMessage(true));
        }

        protected virtual void ExecuteCancelCommand()
        {
            Messenger.Send(new DialogResultMessage(false));
        }
    }
}