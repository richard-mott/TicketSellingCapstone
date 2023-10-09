using C868.Capstone.Core.ViewModels.Dialogs;
using C868.Capstone.Services.Logging;

namespace C868.Capstone.Services.Payment
{
    public class MockCheckPaymentProcessor : IPaymentProcessor
    {
        private readonly IDialogService dialogService;
        private readonly ILoggingService loggingService;

        public MockCheckPaymentProcessor(IDialogService dialogService, ILoggingService loggingService)
        {
            this.dialogService = dialogService;
            this.loggingService = loggingService;
        }

        public bool ProcessPayment(double total)
        {
            var confirmViewModel = new ConfirmDialogViewModel(
                @"Mock Check Processor",
                "This is a mock check payment processor. Click \"Yes\" to " +
                "accept the payment, or \"No\" to decline the payment.");

            var dialogResult = false;
            dialogService.ShowDialog(confirmViewModel, result =>
            {
                loggingService.LogInfo(
                    result == true
                        ? $"Accepted check payment: {total:C}"
                        : $"Declined check payment: {total:C}");

                dialogResult = result ?? false;
            });

            return dialogResult;
        }
    }
}