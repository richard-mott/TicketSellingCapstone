using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace C868.Capstone.Services
{
    public interface IContentService
    {
        UserControl GetContent<TViewModel>(TViewModel viewModel)
            where TViewModel : ObservableObject;
    }
}