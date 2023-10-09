using System;
using System.Threading.Tasks;

namespace C868.Capstone.Core.Reports
{
    public interface IReport
    {
        string Name { get;}
        string Title { get; }
        ReportOptions Options { get; }

        Task<string> BuildReport(IProgress<int> valueProgress,
            IProgress<string> descriptionProgress);
    }
}
