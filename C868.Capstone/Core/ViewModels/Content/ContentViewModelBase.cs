using C868.Capstone.Core.ViewModels.Dialogs;
using C868.Capstone.Services;
using C868.Capstone.Services.Data;
using C868.Capstone.Services.Logging;
using CommunityToolkit.Mvvm.ComponentModel;

namespace C868.Capstone.Core.ViewModels.Content
{
    public class ContentViewModelBase : ObservableRecipient
    {
        protected readonly IDataService DataService;
        protected readonly IDialogService DialogService;
        protected readonly ILoggingService LoggingService;

        public ContentViewModelBase(IDataService dataService, IDialogService dialogService,
            ILoggingService loggingService)
        {
            DataService = dataService;
            DialogService = dialogService;
            LoggingService = loggingService;
        }

        protected void LogSave(string recordName, string recordType)
        {
            LoggingService.LogInfo(
                $"Saved {recordType}: {recordName}");
        }

        protected void LogDelete(string recordName, string recordType)
        {
            LoggingService.LogInfo(
                $"Deleted {recordType}: {recordName}");
        }
        
        protected void HandleError(string title, string message)
        {
            LoggingService.LogError($"{title} : {message}");

            var errorViewModel = new ErrorDialogViewModel(
                title, message, ErrorType.Error);

            DialogService.ShowErrorDialog(errorViewModel);
        }
    }
}