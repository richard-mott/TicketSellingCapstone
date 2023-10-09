using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace C868.Capstone.Core.ViewModels.Content.Auditoriums
{
    public class AuditoriumsViewModel : ObservableObject, IDataLoader
    {
        private IProgress<string> descriptionProgress;
        private readonly IServiceProvider services = App.Current.Services;

        private AuditoriumListViewModel auditoriumList;
        public AuditoriumListViewModel AuditoriumList
        {
            get => auditoriumList;
            set => SetProperty(ref auditoriumList, value);
        }

        private AuditoriumEditorViewModel auditoriumEditor;
        public AuditoriumEditorViewModel AuditoriumEditor
        {
            get => auditoriumEditor;
            set => SetProperty(ref auditoriumEditor, value);
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

        public AuditoriumsViewModel()
        {
            AuditoriumList = services.GetService<AuditoriumListViewModel>();
            AuditoriumEditor = services.GetService <AuditoriumEditorViewModel>();

            InitializeProgressReporting();
        }

        private void InitializeProgressReporting()
        {
            descriptionProgress = new Progress<string>(
                description => ProgressDescription = description);
        }

        public async Task InitializeData()
        {
            ShowProgress = true;
            descriptionProgress.Report(@"Loading auditoriums...");
            
            await AuditoriumList.InitializeData();
            
            descriptionProgress.Report(string.Empty);
            ShowProgress = false;
        }
    }
}