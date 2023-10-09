using System;
using System.Windows;
using C868.Capstone.Core;
using C868.Capstone.Core.Reports;
using C868.Capstone.Core.ViewModels;
using C868.Capstone.Core.ViewModels.Content.Auditoriums;
using C868.Capstone.Core.ViewModels.Content.Movies;
using C868.Capstone.Core.ViewModels.Content.Reports;
using C868.Capstone.Core.ViewModels.Content.Selling;
using C868.Capstone.Core.ViewModels.Content.ShowTimes;
using C868.Capstone.Core.ViewModels.Content.TicketTypes;
using C868.Capstone.Core.ViewModels.Content.Users;
using C868.Capstone.Core.ViewModels.Controls;
using C868.Capstone.Core.ViewModels.Dialogs;
using C868.Capstone.Core.Views.Content.Auditoriums;
using C868.Capstone.Core.Views.Content.Movies;
using C868.Capstone.Core.Views.Content.Reports;
using C868.Capstone.Core.Views.Content.Selling;
using C868.Capstone.Core.Views.Content.ShowTimes;
using C868.Capstone.Core.Views.Content.TicketTypes;
using C868.Capstone.Core.Views.Content.Users;
using C868.Capstone.Core.Views.Dialogs;
using C868.Capstone.Services;
using C868.Capstone.Services.Data;
using C868.Capstone.Services.Data.Sample;
using C868.Capstone.Services.Data.SQLite;
using C868.Capstone.Services.Logging;
using C868.Capstone.Services.Payment;
using Microsoft.Extensions.DependencyInjection;

namespace C868.Capstone
{
    public partial class App : Application
    {
        public new static App Current => (App)Application.Current;

        public IServiceProvider Services { get; }

        public delegate IPaymentProcessor PaymentProcessorResolver(PaymentType payment);

        public App()
        {
            Services = ConfigureServices();

            RegisterDialogs();
            RegisterContent();

            InitializeComponent();
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            RegisterServices(services);
            RegisterViewModels(services);
            RegisterPaymentProcessors(services);
            RegisterReports(services);

            return services.BuildServiceProvider();
        }

        private static void RegisterServices(ServiceCollection services)
        {
            services.AddSingleton<IDataService, SQLiteDataService>();
            services.AddSingleton<SampleDataService>();

            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<IContentService, ContentService>();
            services.AddSingleton<ILoggingService, SQLiteLoggingService>();
        }

        private static void RegisterViewModels(ServiceCollection services)
        {
            // Main window view models
            services.AddTransient<MainViewModel>();
            services.AddSingleton<MainToolbarViewModel>();

            // Log in dialog
            services.AddTransient<LogInDialogViewModel>();

            // Selling view models
            services.AddTransient<SellingViewModel>();
            services.AddTransient<TicketSelectorViewModel>();
            services.AddTransient<TransactionViewModel>();

            // Reports view models
            services.AddTransient<ReportsViewModel>();
            
            // Show times view models
            services.AddTransient<ShowTimesViewModel>();
            services.AddTransient<ScheduleViewerViewModel>();

            // Auditoriums view models
            services.AddTransient<AuditoriumsViewModel>();
            services.AddTransient<AuditoriumListViewModel>();
            services.AddTransient<AuditoriumEditorViewModel>();

            // Movies view models
            services.AddTransient<MoviesViewModel>();
            services.AddTransient<MovieListViewModel>();
            services.AddTransient<MovieEditorViewModel>();

            // Ticket types view models
            services.AddTransient<TicketTypesViewModel>();
            services.AddTransient<TicketTypeListViewModel>();
            services.AddTransient<TicketTypeEditorViewModel>();

            // Users view models
            services.AddTransient<UsersViewModel>();
            services.AddTransient<UserListViewModel>();
            services.AddTransient<UserEditorViewModel>();
        }

        private static void RegisterPaymentProcessors(ServiceCollection services)
        {
            services.AddTransient<MockCashPaymentProcessor>();
            services.AddTransient<MockCheckPaymentProcessor>();
            services.AddTransient<MockCreditPaymentProcessor>();

            services.AddTransient<PaymentProcessorResolver>(serviceProvider => token =>
            {
                switch (token)
                {
                    case PaymentType.Credit:
                        return serviceProvider.GetService<MockCreditPaymentProcessor>();
                    case PaymentType.Check:
                        return serviceProvider.GetService<MockCheckPaymentProcessor>();
                    case PaymentType.Cash:
                    default:
                        return serviceProvider.GetService<MockCashPaymentProcessor>();
                }
            });
        }

        private static void RegisterReports(ServiceCollection services)
        {
            services.AddTransient<IReport, AllSalesReport>();
            services.AddTransient<IReport, SalesByDateRangeReport>();
            services.AddTransient<IReport, AllDailyRecordsReport>();
            services.AddTransient<IReport, DailyRecordsByDateRangeReport>();
            services.AddTransient<IReport, AllActivityLogsReport>();
            services.AddTransient<IReport, ActivityLogByDateRangeReport>();
        }

        private static void RegisterDialogs()
        {
            DialogService.RegisterDialog<LogInDialogViewModel, LogInDialogView>();
            DialogService.RegisterDialog<ErrorDialogViewModel, ErrorDialogView>();
            DialogService.RegisterDialog<ConfirmDialogViewModel, ConfirmDialogView>();
            DialogService.RegisterDialog<CloseDayDialogViewModel, CloseDayDialogView>();
        }

        private static void RegisterContent()
        {
            ContentService.RegisterContent<SellingViewModel, SellingView>();
            ContentService.RegisterContent<ReportsViewModel, ReportsView>();
            ContentService.RegisterContent<ShowTimesViewModel, ShowTimesView>();
            ContentService.RegisterContent<AuditoriumsViewModel, AuditoriumsView>();
            ContentService.RegisterContent<MoviesViewModel, MoviesView>();
            ContentService.RegisterContent<TicketTypesViewModel, TicketTypesView>();
            ContentService.RegisterContent<UsersViewModel, UsersView>();
        }
    }
}
