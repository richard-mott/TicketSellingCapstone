using System;
using System.Threading.Tasks;
using C868.Capstone.Core.Reports.Builders;
using C868.Capstone.Services.Data;

namespace C868.Capstone.Core.Reports
{
    public class AllActivityLogsReport : IReport
    {
        private readonly IDataService dataService;

        public string Name => "All Activity Logs";
        public string Title => "ALL ACTIVITY LOGS";
        public ReportOptions Options { get; }

        public AllActivityLogsReport(IDataService dataService)
        {
            this.dataService = dataService;
            Options = new ReportOptions(requiresLogMessageTypes: true);
        }

        public Task<string> BuildReport(IProgress<int> valueProgress,
            IProgress<string> descriptionProgress)
        {
            var reportBuilder = new ActivityLogReportBuilder(
                Title, DateTime.MinValue, DateTime.MaxValue, Options.IncludeInfo,
                Options.IncludeWarnings, Options.IncludeErrors, dataService);

            return reportBuilder.BuildReport(valueProgress, descriptionProgress);
        }
    }
}