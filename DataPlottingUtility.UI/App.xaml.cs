using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using DataPlottingUtility.Core.Services;
using DataPlottingUtility.Plotting.Interfaces;
using DataPlottingUtility.Plotting.Services;
using DataPlottingUtility.UI.Views;
using DataPlottingUtility.UI.ViewModels;
using DataPlottingUtility.UI.Services;
using DataPlottingUtility.Core.Interfaces;

namespace DataPlottingUtility.UI
{
    public partial class App : Application
    {
        private ServiceProvider? _serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                var services = new ServiceCollection();
                ConfigureServices(services);
                _serviceProvider = services.BuildServiceProvider();

                // Resolve and show main window
                var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to start application: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown(1);
            }
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Register Core services
            services.AddSingleton<ICsvParser, CsvParserService>();

            // Register Plotting services
            services.AddSingleton<IPlotService, PlotService>();

            // Register UI services
            services.AddSingleton<IDialogService, DialogService>();

            // Register ViewModels
            services.AddTransient<MainWindowViewModel>();

            // Register Views
            services.AddTransient<MainWindow>();
        }
    }
}

