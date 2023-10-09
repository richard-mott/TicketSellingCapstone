using System;
using System.Linq;
using System.Threading.Tasks;
using C868.Capstone.Services.Data;

namespace C868.Capstone.Core.Reports.Builders
{
    public class ActivityLogReportBuilder : ReportBuilder
    {
        private readonly bool includeInfo;
        private readonly bool includeWarning;
        private readonly bool includeError;

        public ActivityLogReportBuilder(string title, DateTime startDate, DateTime endDate,
            bool includeInfo, bool includeWarning, bool includeError, IDataService dataService)
            : base(title, startDate, endDate, dataService)
        {
            this.includeInfo = includeInfo;
            this.includeWarning = includeWarning;
            this.includeError = includeError;
        }

        public override async Task<string> BuildReport(IProgress<int> valueProgress,
            IProgress<string> descriptionProgress)
        {
            valueProgress.Report(0);

            descriptionProgress.Report(@"Loading log entries...");

            AppendTitle();

            var logEntries = (await DataService.GetLogEntriesAsync(StartDate, EndDate))
                .Where(logEntry => (includeInfo && logEntry.Type == LogMessageType.Info) ||
                                   (includeWarning && logEntry.Type == LogMessageType.Warning) ||
                                   (includeError && logEntry.Type == LogMessageType.Error))
                .OrderBy(logEntry => logEntry.Created)
                .ToList();

            var userWidth = (await DataService.GetUsersAsync())
                .Select(user => user.UserName.Length)
                .Max();

            var typeWidth = @"[WARNING]  ".Length;
            
            descriptionProgress.Report(@"Processing log entries...");
            for (var index = 0; index < logEntries.Count; index++)
            {
                var logEntry = logEntries[index];

                Builder.Append($"[{logEntry.Created:u}]");
                Builder.Append($"[{logEntry.Type.ToString().ToUpper()}]".PadRight(typeWidth));
                Builder.Append(@"USER: ");
                Builder.Append(logEntry.User.UserName.PadRight(userWidth));
                Builder.Append($" | {logEntry.Message}");
                Builder.Append(Environment.NewLine);

                valueProgress.Report((index + 1) * 100 / logEntries.Count);
            }

            descriptionProgress.Report(string.Empty);

            return Builder.ToString();
        }
    }
}