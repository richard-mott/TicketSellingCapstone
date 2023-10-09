using System;
using System.Threading.Tasks;
using C868.Capstone.Core.Reports.Builders;
using C868.Capstone.Services.Data;

namespace C868.Capstone.Core.Reports
{
    public class ActivityLogByDateRangeReport : IReport
    {
        private readonly IDataService dataService;

        public string Name => @"Activity Log by Date Range";
        public string Title => Options.StartDate == Options.EndDate
            ? $"ACTIVITY LOG FOR {Options.StartDate:d}"
            : $"ACTIVITY LOG FOR {Options.StartDate:d} - {Options.EndDate:d}";

        public ReportOptions Options { get; }

        public ActivityLogByDateRangeReport(IDataService dataService)
        {
            this.dataService = dataService;
            Options = new ReportOptions(requiresDateRange: true, requiresLogMessageTypes: true);
        }

        public Task<string> BuildReport(IProgress<int> valueProgress,
            IProgress<string> descriptionProgress)
        {
            var reportBuilder = new ActivityLogReportBuilder(
                Title, Options.StartDate.Date, Options.EndDate.AddDays(1).Date,
                Options.IncludeInfo, Options.IncludeWarnings, Options.IncludeErrors,
                dataService);

            return reportBuilder.BuildReport(valueProgress, descriptionProgress);
        }
    }
}