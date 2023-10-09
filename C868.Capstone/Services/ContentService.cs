using System;
using System.Collections.Generic;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace C868.Capstone.Services
{
    public class ContentService : IContentService
    {
        private static readonly Dictionary<Type, Type> contentMappings =
            new Dictionary<Type, Type>();

        public static void RegisterContent<TViewModel, TView>()
            where TViewModel : ObservableObject
            where TView : UserControl
        {
            var viewModelType = typeof(TViewModel);
            var viewType = typeof(TView);

            if (contentMappings.ContainsKey(viewModelType))
            {
                contentMappings[viewModelType] = viewType;
            }
            else
            {
                contentMappings.Add(viewModelType, viewType);
            }
        }

        public UserControl GetContent<TViewModel>(TViewModel viewModel)
            where TViewModel : ObservableObject
        {
            var viewType = contentMappings[typeof(TViewModel)];
            var view = Activator.CreateInstance(viewType) as UserControl;

            view.DataContext = viewModel;

            return view;
        }
    }
}