using C868.Capstone.Core.ViewModels.Dialogs;
using C868.Capstone.Services.Logging;

namespace C868.Capstone.Services.Payment
{
    public class MockCashPaymentProcessor : IPaymentProcessor
    {
        private readonly IDialogService dialogService;
        private readonly ILoggingService loggingService;

        public MockCashPaymentProcessor(IDialogService dialogService,
            ILoggingService loggingService)
        {
            this.dialogService = dialogService;
            this.loggingService = loggingService;
        }

        public bool ProcessPayment(double total)
        {
            var confirmViewModel = new ConfirmDialogViewModel(
                @"Mock Cash Processor",
                "This is a mock cash payment processor. Click \"Yes\" to " +
                "accept the payment, or \"No\" to decline the payment.");

            var dialogResult = false;
            dialogService.ShowDialog(confirmViewModel, result =>
            {
                loggingService.LogInfo(
                    result == true
                        ? $"Accepted cash payment: {total:C}"
                        : $"Declined cash payment: {total:C}");

                dialogResult = result ?? false;
            });

            return dialogResult;
        }
    }
}