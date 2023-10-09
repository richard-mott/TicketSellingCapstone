using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using C868.Capstone.Core.Reports;
using C868.Capstone.Core.ViewModels.Data;
using C868.Capstone.Services;
using C868.Capstone.Services.Data;
using C868.Capstone.Services.Logging;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace C868.Capstone.Core.ViewModels.Content.Reports
{
    public class ReportsViewModel : ContentViewModelBase, IDataLoader
    {
        private IProgress<int> valueProgress;
        private IProgress<string> descriptionProgress;
        private List<MovieViewModel> movies;

        private readonly IServiceProvider services = App.Current.Services;

        public List<IReport> Reports { get; }

        private IReport selectedReport;
        public IReport SelectedReport
        {
            get => selectedReport;
            set
            {
                if (SetProperty(ref selectedReport, value))
                {
                    SelectedReport.Options.Movies = movies;
                    OnPropertyChanged(nameof(Options));
                    ((IRelayCommand)GenerateReportCommand).NotifyCanExecuteChanged();
                }
            }
        }
        
        private string results;
        public string Results
        {
            get => results;
            set => SetProperty(ref results, value);
        }

        public ReportOptions Options => SelectedReport?.Options ?? new ReportOptions();

        private bool isIndeterminate;
        public bool IsIndeterminate
        {
            get => isIndeterminate;
            set
            {
                if (SetProperty(ref isIndeterminate, value))
                {
                    OnPropertyChanged(nameof(ShowPercentage));
                }
            }
        }

        public bool ShowPercentage => !IsIndeterminate;

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

        public ICommand GenerateReportCommand { get; private set; }

        public ReportsViewModel(IDataService dataService, IDialogService dialogService,
            ILoggingService loggingService)
            : base(dataService, dialogService, loggingService)
        {
            InitializeProgressReporting();
            InitializeCommands();

            Reports = GetReports();
        }

        public async Task InitializeData()
        {
            SelectedReport = null;

            try
            {
                IsIndeterminate = true;
                ShowProgress = true;
                descriptionProgress.Report("Loading movies...");

                movies = (await DataService.GetMoviesAsync())
                    .OrderBy(movie => movie.Name)
                    .Select(movie => new MovieViewModel(movie))
                    .ToList();

            }
            catch (Exception exception)
            {
                HandleError(
                    AppSettings.Errors.Data.LoadTitle,
                    string.IsNullOrWhiteSpace(exception.Message)
                        ? AppSettings.Errors.Data.LoadMessage
                        : exception.Message);
            }
            finally
            {
                descriptionProgress.Report(string.Empty);
                ShowProgress = false;
            }
            
        }

        private void InitializeProgressReporting()
        {
            valueProgress = new Progress<int>(
                value => ProgressValue = value);
            descriptionProgress = new Progress<string>(
                description => ProgressDescription = description);
        }

        private void InitializeCommands()
        {
            GenerateReportCommand = new RelayCommand(
                ExecuteGenerateReportCommand,
                () => SelectedReport != null);
        }

        private List<IReport> GetReports()
        {
            return services.GetServices<IReport>().ToList();
        }

        private async void ExecuteGenerateReportCommand()
        {
            try
            {
                IsIndeterminate = false;
                ShowProgress = true;

                Results = await SelectedReport.BuildReport(valueProgress, descriptionProgress);
            }
            catch (Exception exception)
            {
                HandleError(
                    AppSettings.Errors.Data.LoadTitle,
                    string.IsNullOrWhiteSpace(exception.Message)
                        ? AppSettings.Errors.Data.LoadMessage
                        : exception.Message);
            }
            finally
            {
                ShowProgress = false;
            }
        }
    }
}