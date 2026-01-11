using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataPlottingUtility.Core;
using DataPlottingUtility.Core.Interfaces;
using DataPlottingUtility.Core.Models;
using DataPlottingUtility.Core.Services;
using DataPlottingUtility.Plotting.Interfaces;
using DataPlottingUtility.Plotting.Models;
using DataPlottingUtility.UI.Services;
using ScottPlot.WPF;

namespace DataPlottingUtility.UI.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly ICsvParser _csvParser;
        private readonly IDialogService _dialogService;
        private readonly IPlotService _plotService;

        public MainWindowViewModel(
            ICsvParser csvParser,
            IDialogService dialogService,
            IPlotService plotService)
        {
            _csvParser = csvParser;
            _dialogService = dialogService;
            _plotService = plotService;
            Channels = new ObservableCollection<ChannelViewModel>();
            PlotControls = new ObservableCollection<WpfPlot>();
        }

        [ObservableProperty]
        private DataSet? _loadedData;

        [ObservableProperty]
        private string? _loadedFileName;

        [ObservableProperty]
        private bool _isDataLoaded = false;

        [ObservableProperty]
        private bool _isLoading = false;

        [ObservableProperty]
        private string? _statusMessage;

        public ObservableCollection<ChannelViewModel> Channels { get; }
        public ObservableCollection<WpfPlot> PlotControls { get; }

        public ObservableCollection<ChannelViewModel> SelectedChannels =>
            new ObservableCollection<ChannelViewModel>(Channels.Where(c => c.IsSelected));

        [ObservableProperty]
        private PlotConfiguration _plotConfig = new PlotConfiguration();
       
        // New property for available X-axis channels - made observable
        public ObservableCollection<string> XAxisChannels
        {
            get
            {
                if (LoadedData?.Channels != null)
                {
                    var channelNames = LoadedData.Channels.Select(c => c.Name).ToList();
                    channelNames.Insert(0, "Sample"); // Add default option
                    return new ObservableCollection<string>(channelNames);
                }
                return new ObservableCollection<string> { "Sample" };
            }
        }

        [RelayCommand]
        private async Task LoadFileAsync()
        {
            // Show file dialog
            var filePath = _dialogService.OpenFileDialog("CSV Files (*.csv)|*.csv|All Files (*.*)|*.*", "Select CSV File");

            if (string.IsNullOrEmpty(filePath))
                return;

            IsLoading = true;
            StatusMessage = "Loading file...";

            try
            {
                // Parse the file
                var result = await _csvParser.ParseFileAsync(filePath);

                if (result == null)
                {
                    throw new InvalidOperationException("Failed to parse CSV file.");
                }

                // Set loaded data
                LoadedData = result.Data;
                LoadedFileName = Path.GetFileName(filePath);

                // Create channel view models
                Channels.Clear();
                foreach (var channel in result.Data.Channels)
                {
                    Channels.Add(new ChannelViewModel(this)
                    {
                        Name = channel.Name,
                        Unit = channel.Unit,
                        IsSelected = true,
                        Data = channel
                    });
                }

                StatusMessage = $"Loaded {result.Data.Channels.Count} channels from {Path.GetFileName(filePath)}";
                IsDataLoaded = true;

                // Update plots
                UpdatePlots();
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Failed to load file: {ex.Message}", "Load Error");
                StatusMessage = $"Error loading file: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void UpdatePlots()
        {
            // Clear existing plots
            PlotControls.Clear();

            if (LoadedData == null)
            {
                StatusMessage = "No data loaded";
                return;
            }

            // Get selected channels
            var selectedChannels = Channels.Where(c => c.IsSelected).ToList();

            if (selectedChannels.Count == 0)
            {
                StatusMessage = "No channels selected";
                PlotControls.Clear();
                return;
            }


            // Clear plots if we have more than needed
            while (PlotControls.Count > selectedChannels.Count)
            {
                PlotControls.RemoveAt(PlotControls.Count - 1);
            }

            // Create a plot for each selected channel
            //foreach (var channelVM in selectedChannels)
            //{
            //    if (channelVM.Data != null)
            //    {
            //        var plot = _plotService.CreatePlot(channelVM.Data);
            //        _plotService.ConfigurePlot(plot, channelVM.Data, PlotConfig);
            //        PlotControls.Add(plot);
            //    }
            //}

            //StatusMessage = $"Displaying {selectedChannels.Count} plots";
            //OnPropertyChanged(nameof(XAxisChannels)); // Force refresh of X-axis channels

            for (int i = 0; i < selectedChannels.Count; i++)
            {
                var channelVM = selectedChannels[i];
                if (channelVM.Data == null) continue;

                if (i < PlotControls.Count)
                {
                    // Update existing plot
                    var observableChannels = new ObservableCollection<ChannelData>(LoadedData.Channels);
                    _plotService.UpdatePlotData(PlotControls[i], channelVM.Data, observableChannels, PlotConfig.XAxisChannelName);
                }
                else
                {
                    // Create new plot control
                    var observableChannels = new ObservableCollection<ChannelData>(LoadedData.Channels);
                    var plot = _plotService.CreatePlot(channelVM.Data, observableChannels, PlotConfig.XAxisChannelName);
                    _plotService.ConfigurePlot(plot, channelVM.Data, PlotConfig);
                    PlotControls.Add(plot);
                }
            }

            StatusMessage = $"Displaying {selectedChannels.Count} plots";
            OnPropertyChanged(nameof(XAxisChannels));
        }
        
        [RelayCommand]
        private void UpdateXAxisChannel(string channelName)
        {
            PlotConfig.XAxisChannelName = channelName;
            UpdatePlots(); // Rebuild plots with new X-axis
        }

        [RelayCommand]
        public void OnXAxisChannelChanged()
        {
            if (IsDataLoaded && SelectedChannels.Any())
            {
                UpdatePlots();
            }
        }

        partial void OnPlotConfigChanged(PlotConfiguration value)
        {
            // When XAxisChannelName changes, update the plots automatically
            if (IsDataLoaded && SelectedChannels.Any())
            {
                UpdatePlots();
            }
        }
            
        [RelayCommand]
        private void ExportPlot()
        {
            // Placeholder for export functionality
            _dialogService.ShowInfo("Export functionality would be implemented here.", "Export");
        }

        [RelayCommand]
        private void Exit()
        {
            Application.Current.Shutdown();
        }

        public partial class ChannelViewModel : ObservableObject
        {
            private readonly MainWindowViewModel _parent;

            public ChannelViewModel(MainWindowViewModel parent)
            {
                _parent = parent;
            }

            [ObservableProperty]
            private string _name = string.Empty;

            [ObservableProperty]
            private string _unit = string.Empty;

            [ObservableProperty]
            private bool _isSelected;

            partial void OnIsSelectedChanged(bool value)
            {
                _parent.UpdatePlots();
            }

            public ChannelData? Data { get; set; }
        }
    }
}


