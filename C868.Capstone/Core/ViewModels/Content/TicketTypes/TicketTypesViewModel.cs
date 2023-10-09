using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace C868.Capstone.Core.ViewModels.Content.TicketTypes
{
    public class TicketTypesViewModel : ObservableObject, IDataLoader
    {
        private IProgress<int> valueProgess;
        private IProgress<string> descriptionProgress;
        private readonly IServiceProvider services = App.Current.Services;

        private TicketTypeListViewModel ticketTypeList;
        public TicketTypeListViewModel TicketTypeList
        {
            get => ticketTypeList;
            set => SetProperty(ref ticketTypeList, value);
        }

        private TicketTypeEditorViewModel ticketTypeEditor;
        public TicketTypeEditorViewModel TicketTypeEditor
        {
            get => ticketTypeEditor;
            set => SetProperty(ref ticketTypeEditor, value);
        }

        public bool IsIndeterminate => true;
        public bool ShowPercentage => false;

        private bool showProgress;
        public bool ShowProgress
        {
            get => showProgress;
            set => SetProperty(ref showProgress, value);
        }

        private int progressValue;
        public int ProgressValue
        {
            get => progressValue;
            set => SetProperty(ref progressValue, value);
        }

        private string progressDescription;
        public string ProgressDescription
        {
            get => progressDescription;
            set => SetProperty(ref progressDescription, value);
        }

        public TicketTypesViewModel()
        {
            TicketTypeList = services.GetService<TicketTypeListViewModel>();
            TicketTypeEditor = services.GetService<TicketTypeEditorViewModel>();

            InitializeProgressReporting();
        }

        private void InitializeProgressReporting()
        {
            valueProgess = new Progress<int>(
                value => ProgressValue = value);
            descriptionProgress = new Progress<string>(
                description => ProgressDescription = description);
        }

        public async Task InitializeData()
        {
            ShowProgress = true;
            descriptionProgress.Report(@"Loading ticket types...");

            await TicketTypeList.InitializeData();
            
            descriptionProgress.Report(string.Empty);
            ShowProgress = false;
        }
    }
}