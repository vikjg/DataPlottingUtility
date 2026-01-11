using ScottPlot.WPF;
using DataPlottingUtility.Core.Models;
using DataPlottingUtility.Plotting.Models;
using System.Collections.ObjectModel;

namespace DataPlottingUtility.Plotting.Interfaces
{
    /// 
    /// Service for creating and managing plot controls
    /// 
    public interface IPlotService
    {
        /// 
        /// Create a new plot control for displaying channel data
        /// 
        /// Channel data to plot
        /// Configured WpfPlot control
        WpfPlot CreatePlot(ChannelData channel, ObservableCollection<ChannelData> allChannels = null, string xAxisChannelName = null);

        /// 
        /// Configure plot appearance and behavior
        /// 
        /// The plot control to configure
        /// Channel data for labels
        /// Configuration settings
        void ConfigurePlot(WpfPlot plotControl, ChannelData channel, PlotConfiguration config);

        /// 
        /// Update plot with new data
        /// 
        /// The plot to update
        /// New channel data
        void UpdatePlotData(WpfPlot plotControl, ChannelData channel, ObservableCollection<ChannelData> allChannels = null, string xAxisChannelName = null);
    }
}

