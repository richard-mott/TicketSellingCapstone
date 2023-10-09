using System.Threading.Tasks;

namespace C868.Capstone.Core.ViewModels
{
    public interface IDataLoader
    {
        bool IsIndeterminate { get; }
        bool ShowPercentage { get; }
        bool ShowProgress { get; set; }
        int ProgressValue { get; set; }
        string ProgressDescription { get; set; }

        Task InitializeData();
    }
}