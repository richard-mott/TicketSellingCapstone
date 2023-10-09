using System;
using System.Linq;
using System.Threading.Tasks;
using C868.Capstone.Core.Reports.Builders;
using C868.Capstone.Services.Data;

namespace C868.Capstone.Core.Reports
{
    public class AllSalesReport : IReport
    {
        private readonly IDataService dataService;
        
        public string Name => @"All Sales";
        public string Title => @"SALES DATA FOR ALL MOVIES";
        public ReportOptions Options { get; }

        public AllSalesReport(IDataService dataService)
        {
            this.dataService = dataService;

            Options = new ReportOptions(false, false, false);
        }

        public async Task<string> BuildReport(IProgress<int> valueProgress,
            IProgress<string> descriptionProgress)
        {
            descriptionProgress.Report(@"Loading movies...");
            var allMovies = (await dataService.GetMoviesAsync())
                .OrderBy(movie => movie.Name)
                .ToList();

            var reportBuilder = new MovieSalesReportBuilder(
                Title, allMovies, DateTime.MinValue, DateTime.MaxValue, dataService);

            return await reportBuilder.BuildReport(valueProgress, descriptionProgress);
        }
    }
}