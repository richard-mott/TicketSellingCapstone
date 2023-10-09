using System;
using System.Threading.Tasks;
using C868.Capstone.Core.Reports.Builders;
using C868.Capstone.Services.Data;

namespace C868.Capstone.Core.Reports
{
    public class DailyRecordsByDateRangeReport : IReport
    {
        private readonly IDataService dataService;

        public string Name => @"Daily Records by Date Range";
        public string Title => Options.StartDate == Options.EndDate
            ? $"DAILY RECORD FOR {Options.StartDate:d}"
            : $"DAILY RECORDS FOR {Options.StartDate:d} - {Options.EndDate:d}";

        public ReportOptions Options { get; }

        public DailyRecordsByDateRangeReport(IDataService dataService)
        {
            this.dataService = dataService;
            Options = new ReportOptions(requiresDateRange: true);
        }

        public async Task<string> BuildReport(IProgress<int> valueProgress,
            IProgress<string> descriptionProgress)
        {
            var reportBuilder = new DailyRecordReportBuilder(
                Title, Options.StartDate.Date, Options.EndDate.AddDays(1).Date, dataService);

            return await reportBuilder.BuildReport(valueProgress, descriptionProgress);
        }
    }
}