using System;
using System.Threading.Tasks;
using C868.Capstone.Services;
using C868.Capstone.Services.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace C868.Capstone.Core.ViewModels.Content.Selling
{
    public class SellingViewModel : ObservableObject
    {
        private readonly IServiceProvider services = App.Current.Services;

        private TicketSelectorViewModel ticketSelector;
        public TicketSelectorViewModel TicketSelector
        {
            get => ticketSelector;
            set => SetProperty(ref ticketSelector, value);
        }

        private TransactionViewModel transaction;
        public TransactionViewModel Transaction
        {
            get => transaction;
            set => SetProperty(ref transaction, value);
        }

        public SellingViewModel(IDataService dataService, IDialogService dialogService)
        {
            TicketSelector = services.GetService<TicketSelectorViewModel>();
            Transaction = services.GetService<TransactionViewModel>();
        }

        public async Task InitializeData()
        {
            await TicketSelector.InitializeData();
        }
    }
}