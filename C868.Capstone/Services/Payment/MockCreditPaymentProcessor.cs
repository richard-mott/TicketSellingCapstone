using C868.Capstone.Core.ViewModels.Dialogs;
using C868.Capstone.Services.Logging;

namespace C868.Capstone.Services.Payment
{
    public class MockCreditPaymentProcessor : IPaymentProcessor
    {
        private readonly IDialogService dialogService;
        private readonly ILoggingService loggingService;

        public MockCreditPaymentProcessor(IDialogService dialogService, ILoggingService loggingService)
        {
            this.dialogService = dialogService;
            this.loggingService = loggingService;
        }

        public bool ProcessPayment(double total)
        {
            var confirmViewModel = new ConfirmDialogViewModel(
                @"Mock Credit Card Processor",
                "This is a mock credit card payment processor. Click \"Yes\" to " +
                "accept the payment, or \"No\" to decline the payment.");

            var dialogResult = false;
            dialogService.ShowDialog(confirmViewModel, (result) =>
            {
                loggingService.LogInfo(
                    result == true
                        ? $"Accepted credit card payment: {total:C}"
                        : $"Declined credit card payment: {total:C}");

                dialogResult = result ?? false;
            });

            return dialogResult;
        }
    }
}