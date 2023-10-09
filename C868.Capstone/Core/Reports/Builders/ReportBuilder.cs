using System;
using System.Text;
using System.Threading.Tasks;
using C868.Capstone.Services.Data;

namespace C868.Capstone.Core.Reports.Builders
{
    public class ReportBuilder
    {
        protected readonly IDataService DataService;
        protected readonly StringBuilder Builder;
        protected readonly DateTime StartDate;
        protected readonly DateTime EndDate;

        protected const int DefaultLineWidth = 100;

        public string Title { get; }

        public ReportBuilder(string title, DateTime startDate, DateTime endDate,
            IDataService dataService)
        {
            Title = title;
            StartDate = startDate;

            // Subtract one day from the end date if it is set to DateTime.MaxValue
            // This avoids loops causing errors by exceeding the maximum value of DateTime
            EndDate = endDate == DateTime.MaxValue ? endDate.AddDays(-1) : endDate;
            DataService = dataService;

            Builder = new StringBuilder();
        }

        public virtual async Task<string> BuildReport(IProgress<int> valueProgress,
            IProgress<string> descriptionProgress)
        {
            valueProgress.Report(0);
            descriptionProgress.Report(@"Building report title...");

            AppendTitle(DefaultLineWidth);
            var result = await Task.FromResult(Builder.ToString());

            valueProgress.Report(100);
            descriptionProgress.Report(string.Empty);
            
            return result;
        }

        protected virtual void AppendTitle(int lineWidth = DefaultLineWidth)
        {
            AppendBoxTop(lineWidth);

            // Pad the title and generated on lines to {lineWidth} to
            // account for the spacing at the start and end of the line
            Builder.Append($"│ {Title.PadRight(lineWidth - 2)} │");
            Builder.Append(Environment.NewLine);
            
            var generatedOn = $"GENERATED ON: {DateTime.Now:g}";
            Builder.Append($"│ {generatedOn.PadRight(lineWidth - 2)} │");
            Builder.Append(Environment.NewLine);

            AppendBoxBottom(lineWidth);
        }

        protected virtual void AppendBoxTop(int lineWidth)
        {
            Builder.Append("┌");
            Builder.Append(new string('─', lineWidth));
            Builder.Append("┐");
            Builder.Append(Environment.NewLine);
        }

        protected virtual void AppendBoxMiddle(int lineWidth)
        {
            Builder.Append("├");
            Builder.Append(new string('─', lineWidth));
            Builder.Append("┤");
            Builder.Append(Environment.NewLine);
        }

        protected virtual void AppendBoxBottom(int lineWidth)
        {
            Builder.Append("└");
            Builder.Append(new string('─', lineWidth));
            Builder.Append("┘");
            Builder.Append(Environment.NewLine);
        }
    }
}