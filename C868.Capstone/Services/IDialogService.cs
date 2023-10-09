using System;
using C868.Capstone.Core.ViewModels.Dialogs;

namespace C868.Capstone.Services
{
    public interface IDialogService
    {
        void ShowDialog<TViewModel>(TViewModel viewModel, Action<bool?> callback)
            where TViewModel : DialogViewModel;

        void ShowErrorDialog(ErrorDialogViewModel viewModel);
    }
}