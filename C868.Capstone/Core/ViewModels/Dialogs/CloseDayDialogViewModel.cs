using C868.Capstone.Core.ViewModels.Data;

namespace C868.Capstone.Core.ViewModels.Dialogs
{
    public class CloseDayDialogViewModel : DialogViewModel
    {
        public CloseDailyRecordViewModel DailyRecord { get; }

        public CloseDayDialogViewModel(CloseDailyRecordViewModel dailyRecordViewModel)
        {
            Title = @"Close Day";
            DailyRecord = dailyRecordViewModel;
        }
    }
}