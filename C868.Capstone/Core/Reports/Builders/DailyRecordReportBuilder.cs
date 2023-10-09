using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using C868.Capstone.Core.Models.Activities;
using C868.Capstone.Services.Data;

namespace C868.Capstone.Core.Reports.Builders
{
    public class DailyRecordReportBuilder : ReportBuilder
    {
        private const int DescriptionWidth = 15;
        private const int ValueWidth = 15;
        private const int LineWidth = DescriptionWidth + (ValueWidth * 3);

        public DailyRecordReportBuilder(string title, DateTime startDate, DateTime endDate,
            IDataService dataService)
            : base(title, startDate, endDate, dataService)
        {
        }

        public override async Task<string> BuildReport(IProgress<int> valueProgress,
            IProgress<string> descriptionProgress)
        {
            valueProgress.Report(0);

            AppendTitle(LineWidth);

            descriptionProgress.Report(@"Loading daily records...");
            var dailyRecords = (await DataService.GetDailyRecordsAsync(StartDate, EndDate))
                .OrderBy(dailyRecord => dailyRecord.OpenDate)
                .ToList();

            if (dailyRecords.Count == 0)
            {
                Builder.Append(@"NO RECORDS FOUND FOR SELECTED DATE RANGE");
                return Builder.ToString();
            }

            for (var index = 0; index < dailyRecords.Count; index++)
            {
                var dailyRecord = dailyRecords[index];

                descriptionProgress.Report(
                    $"Processing daily record for {dailyRecord.OpenDate:d}...");

                AppendDailyRecord(dailyRecord);
                
                valueProgress.Report((index + 1) * 100 / dailyRecords.Count);
            }

            descriptionProgress.Report(string.Empty);

            if (StartDate.Date != EndDate.Date)
            {
                AppendGrandTotals(dailyRecords);
            }

            return Builder.ToString();
        }
        
        private void AppendDailyRecord(DailyRecord dailyRecord)
        {
            var cashDifference = dailyRecord.CashActual - dailyRecord.CashExpected;
            var creditDifference = dailyRecord.CreditActual - dailyRecord.CreditExpected;
            var checkDifference = dailyRecord.CheckActual - dailyRecord.CheckExpected;

            AppendHeader(dailyRecord);

            Builder.Append(@"│");
            Builder.Append($" EXPECTED".PadRight(DescriptionWidth));
            Builder.Append(
                $"{dailyRecord.CashExpected:#,##0.00 ;(#,##0.00)} ".PadLeft(ValueWidth));
            Builder.Append(
                $"{dailyRecord.CreditExpected:#,##0.00 ;(#,##0.00)} ".PadLeft(ValueWidth));
            Builder.Append(
                $"{dailyRecord.CheckExpected:#,##0.00 ;(#,##0.00)} ".PadLeft(ValueWidth));
            Builder.Append(@"│");
            Builder.Append(Environment.NewLine);

            Builder.Append(@"│");
            Builder.Append($" ACTUAL".PadRight(DescriptionWidth));
            Builder.Append(
                $"{dailyRecord.CashActual:#,##0.00 ;(#,##0.00)} ".PadLeft(ValueWidth));
            Builder.Append(
                $"{dailyRecord.CreditActual:#,##0.00 ;(#,##0.00)} ".PadLeft(ValueWidth));
            Builder.Append(
                $"{dailyRecord.CheckActual:#,##0.00 ;(#,##0.00)} ".PadLeft(ValueWidth));
            Builder.Append(@"│");
            Builder.Append(Environment.NewLine);

            Builder.Append(@"│");
            Builder.Append($" DIFFERENCE".PadRight(DescriptionWidth));
            Builder.Append($"{cashDifference:#,##0.00 ;(#,##0.00)} ".PadLeft(ValueWidth));
            Builder.Append($"{creditDifference:#,##0.00 ;(#,##0.00)} ".PadLeft(ValueWidth));
            Builder.Append($"{checkDifference:#,##0.00 ;(#,##0.00)} ".PadLeft(ValueWidth));
            Builder.Append(@"│");
            Builder.Append(Environment.NewLine);

            AppendBoxBottom(LineWidth);
        }

        private void AppendHeader(DailyRecord dailyRecord)
        {
            var recordOpen = dailyRecord.IsOpen
                ? @"[OPEN]"
                : @"[CLOSED]";

            var recordDescription = dailyRecord.OpenDate.Date == dailyRecord.CloseDate.Date
                ? $"{dailyRecord.OpenDate:g} TO {dailyRecord.CloseDate:t}"
                : $"{dailyRecord.OpenDate:g} TO {dailyRecord.CloseDate:g}";

            AppendBoxTop(LineWidth);

            Builder.Append(@"│");
            Builder.Append($" {recordOpen} {recordDescription}".PadRight(LineWidth));
            Builder.Append(@"│");
            Builder.Append(Environment.NewLine);

            AppendBoxMiddle(LineWidth);

            Builder.Append(@"│");
            Builder.Append(new string(' ', DescriptionWidth));
            Builder.Append(@"CASH  ".PadLeft(ValueWidth));
            Builder.Append(@"CREDIT  ".PadLeft(ValueWidth));
            Builder.Append(@"CHECK  ".PadLeft(ValueWidth));
            Builder.Append(@"│");
            Builder.Append(Environment.NewLine);

            AppendBoxMiddle(LineWidth);
        }

        private void AppendGrandTotals(List<DailyRecord> dailyRecords)
        {
            var totalExpectedCash = dailyRecords
                .Sum(dailyRecord => dailyRecord.CashExpected);
            var totalActualCash = dailyRecords
                .Sum(dailyRecord => dailyRecord.CashActual);
            var cashDifference = totalActualCash - totalExpectedCash;

            var totalExpectedCredit = dailyRecords
                .Sum(dailyRecord => dailyRecord.CreditExpected);
            var totalActualCredit = dailyRecords
                .Sum(dailyRecord => dailyRecord.CreditActual);
            var creditDifference = totalActualCredit - totalExpectedCredit;

            var totalExpectedCheck = dailyRecords
                .Sum(dailyRecord => dailyRecord.CheckExpected);
            var totalActualCheck = dailyRecords
                .Sum(dailyRecord => dailyRecord.CheckActual);
            var checkDifference = totalActualCheck - totalExpectedCheck;

            AppendBoxTop(LineWidth);
            Builder.Append(@"│");
            Builder.Append(@" GRAND TOTALS".PadRight(LineWidth));
            Builder.Append(@"│");
            Builder.Append(Environment.NewLine);
            AppendBoxMiddle(LineWidth);

            Builder.Append(@"│");
            Builder.Append(@" EXPECTED".PadRight(DescriptionWidth));
            Builder.Append($"{totalExpectedCash:#,##0.00 ;(#,##0.00)} ".PadLeft(ValueWidth));
            Builder.Append($"{totalExpectedCredit:#,##0.00 ;(#,##0.00)} ".PadLeft(ValueWidth));
            Builder.Append($"{totalExpectedCheck:#,##0.00 ;(#,##0.00)} ".PadLeft(ValueWidth));
            Builder.Append(@"│");
            Builder.Append(Environment.NewLine);

            Builder.Append(@"│");
            Builder.Append(@" ACTUAL".PadRight(DescriptionWidth));
            Builder.Append($"{totalActualCash:#,##0.00 ;(#,##0.00)} ".PadLeft(ValueWidth));
            Builder.Append($"{totalActualCredit:#,##0.00 ;(#,##0.00)} ".PadLeft(ValueWidth));
            Builder.Append($"{totalActualCheck:#,##0.00 ;(#,##0.00)} ".PadLeft(ValueWidth));
            Builder.Append(@"│");
            Builder.Append(Environment.NewLine);

            Builder.Append(@"│");
            Builder.Append(@" DIFFERENCE".PadRight(DescriptionWidth));
            Builder.Append($"{cashDifference:#,##0.00 ;(#,##0.00)} ".PadLeft(ValueWidth));
            Builder.Append($"{creditDifference:#,##0.00 ;(#,##0.00)} ".PadLeft(ValueWidth));
            Builder.Append($"{checkDifference:#,##0.00 ;(#,##0.00)} ".PadLeft(ValueWidth));
            Builder.Append(@"│");
            Builder.Append(Environment.NewLine);

            AppendBoxBottom(LineWidth);
        }
    }
}