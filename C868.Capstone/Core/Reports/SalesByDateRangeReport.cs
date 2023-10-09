using System;
using System.Linq;
using System.Threading.Tasks;
using C868.Capstone.Core.Reports.Builders;
using C868.Capstone.Services.Data;

namespace C868.Capstone.Core.Reports
{
    public class SalesByDateRangeReport : IReport
    {
        private readonly IDataService dataService;
        
        public string Name => @"Sales by Date Range";

        public string Title => Options.StartDate == Options.EndDate
            ? $"SALES FOR {Options.StartDate:d}"
            : $"SALES FOR {Options.StartDate:d} - {Options.EndDate:d}";

        public ReportOptions Options { get; }

        public SalesByDateRangeReport(IDataService dataService)
        {
            this.dataService = dataService;
            Options = new ReportOptions(requiresDateRange: true);
        }

        public async Task<string> BuildReport(IProgress<int> valueProgress,
            IProgress<string> descriptionProgress)
        {
            var allMovies = (await dataService.GetMoviesAsync())
                .OrderBy(movie => movie.Name)
                .ToList();

            var reportBuilder = new MovieSalesReportBuilder(
                Title, allMovies, Options.StartDate.Date, Options.EndDate.AddDays(1).Date,
                dataService, false);

            return await reportBuilder.BuildReport(valueProgress, descriptionProgress);
        }
    }
}