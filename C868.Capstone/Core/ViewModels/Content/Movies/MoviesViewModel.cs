using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace C868.Capstone.Core.ViewModels.Content.Movies
{
    public class MoviesViewModel : ObservableObject, IDataLoader
    {
        private IProgress<string> descriptionProgress;
        private readonly IServiceProvider services = App.Current.Services;

        private MovieListViewModel movieList;
        public MovieListViewModel MovieList
        {
            get => movieList;
            set => SetProperty(ref movieList, value);
        }

        private MovieEditorViewModel movieEditor;
        public MovieEditorViewModel MovieEditor
        {
            get => movieEditor;
            set => SetProperty(ref movieEditor, value);
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

        public MoviesViewModel()
        {
            MovieList = services.GetService<MovieListViewModel>();
            MovieEditor = services.GetService<MovieEditorViewModel>();

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
            descriptionProgress.Report("Loading movies...");

            await MovieList.InitializeData();

            descriptionProgress.Report(string.Empty);
            ShowProgress = false;
        }
    }
}