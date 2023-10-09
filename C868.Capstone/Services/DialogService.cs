using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using C868.Capstone.Core.Messages;
using C868.Capstone.Core.ViewModels.Dialogs;
using C868.Capstone.Core.Views.Dialogs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace C868.Capstone.Services
{
    public class DialogService : ObservableRecipient, IDialogService
    {
        private static readonly Dictionary<Type, Type> dialogMappings =
            new Dictionary<Type, Type>();

        public void ShowDialog<TViewModel>(TViewModel viewModel, Action<bool?> callback)
            where TViewModel : DialogViewModel
        {
            var dialog = new DialogView
            {
                Owner = Application.Current.MainWindow
            };

            // Set the event handler for the dialog closed event
            void OnClosed(object sender, EventArgs eventArgs)
            {
                callback?.Invoke(dialog.DialogResult);
                dialog.Closed -= OnClosed;
            }

            dialog.Closed += OnClosed;

            // Register to receive the dialog result message
            Messenger.Register<DialogResultMessage>(dialog, (window, message) =>
            {
                Messenger.Unregister<DialogResultMessage>(dialog);
                ((Window)window).DialogResult = message.Value;
            });

            // Initialize the view and its data context
            var viewType = dialogMappings[typeof(TViewModel)];
            var content = Activator.CreateInstance(viewType);
            ((FrameworkElement)content).DataContext = viewModel;

            dialog.Content = content;
            dialog.Title = viewModel.Title;
            dialog.ShowDialog();
        }

        public void ShowErrorDialog(ErrorDialogViewModel viewModel)
        {
            ShowDialog(viewModel, null);
        }

        public static void RegisterDialog<TViewModel, TView>()
            where TViewModel : ObservableObject
            where TView : UserControl
        {
            var viewModelType = typeof(TViewModel);
            var viewType = typeof(TView);

            if (dialogMappings.ContainsKey(viewModelType))
            {
                dialogMappings[viewModelType] = viewType;
            }
            else
            {
                dialogMappings.Add(viewModelType, viewType);
            }
        }
    }
}