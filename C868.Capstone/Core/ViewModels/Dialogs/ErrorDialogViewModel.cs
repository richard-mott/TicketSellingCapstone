namespace C868.Capstone.Core.ViewModels.Dialogs
{
    public class ErrorDialogViewModel : DialogViewModel
    {
        private string errorMessage;
        public string ErrorMessage
        {
            get => errorMessage;
            set => SetProperty(ref errorMessage, value);
        }

        private ErrorType errorType;
        public ErrorType ErrorType
        {
            get => errorType;
            set
            {
                if (SetProperty(ref errorType, value))
                {
                    OnPropertyChanged(nameof(Image));
                }
            }
        }

        public string Image => errorType == ErrorType.Warning
            ? AppSettings.Icons.Warning
            : AppSettings.Icons.Error;

        public ErrorDialogViewModel(string title, string errorMessage, ErrorType errorType)
        {
            Title = title;
            ErrorMessage = errorMessage;
            ErrorType = errorType;
        }
    }
}
