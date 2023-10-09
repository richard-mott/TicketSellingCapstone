using System;
using System.Threading.Tasks;
using C868.Capstone.Core.Reports.Builders;
using C868.Capstone.Services.Data;

namespace C868.Capstone.Core.Reports
{
    public class AllDailyRecordsReport : IReport
    {
        private readonly IDataService dataService;

        public string Name => @"All Daily Records";
        public string Title => @"ALL DAILY RECORDS";
        public ReportOptions Options { get; }

        public AllDailyRecordsReport(IDataService dataService)
        {
            this.dataService = dataService;
            Options = new ReportOptions();
        }

        public Task<string> BuildReport(IProgress<int> valueProgress,
            IProgress<string> descriptionProgress)
        {
            var reportBuilder = new DailyRecordReportBuilder(
                Title, DateTime.MinValue, DateTime.MaxValue, dataService);

            return reportBuilder.BuildReport(valueProgress, descriptionProgress);
        }
    }
}