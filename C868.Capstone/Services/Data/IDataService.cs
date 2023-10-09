using System;
using System.Threading.Tasks;

namespace C868.Capstone.Services.Data
{
    public partial interface IDataService
    {
        Task InitializeTables(IProgress<int> valueProgress,
            IProgress<string> descriptionProgress);
        Task InitializeData(IProgress<int> valueProgress,
            IProgress<string> descriptionProgress);
    }
}