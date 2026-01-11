using System.Linq;
using ScottPlot.WPF;
using DataPlottingUtility.Core.Models;
using DataPlottingUtility.Plotting.Interfaces;
using DataPlottingUtility.Plotting.Models;
using System.Collections.ObjectModel;

namespace DataPlottingUtility.Plotting.Services
{
    /// 
    /// Service for creating and managing ScottPlot controls
    /// 
    public class PlotService : IPlotService
    {
        public WpfPlot CreatePlot(ChannelData channel, ObservableCollection<ChannelData> allChannels = null, string xAxisChannelName = null)
        {
            var wpfPlot = new WpfPlot();

            // Determine X-axis data
            double[] xData;
            double[] yData = channel.Values.ToArray();

            // If we have a custom X-axis channel and it's not "Sample"
            if (!string.IsNullOrEmpty(xAxisChannelName) && xAxisChannelName != "Sample" && allChannels != null)
            {
                // Find the X-axis channel
                var xAxisChannel = allChannels.FirstOrDefault(c => c.Name == xAxisChannelName);
                if (xAxisChannel != null)
                {
                    xData = xAxisChannel.Values.ToArray();
                }
                else
                {
                    // Fallback to sample indices
                    xData = Enumerable.Range(0, channel.Values.Count)
                                      .Select(i => (double)i)
                                      .ToArray();
                }
            }
            else
            {
                // Use sample indices as X-axis
                xData = Enumerable.Range(0, channel.Values.Count)
                                  .Select(i => (double)i)
                                  .ToArray();
            }

            // Add data to plot
            wpfPlot.Plot.Add.Scatter(xData, yData);

            // Set labels
            wpfPlot.Plot.Title(channel.Name);
            wpfPlot.Plot.YLabel(channel.Unit);
            wpfPlot.Plot.XLabel("Sample");

            // Enable grid
            wpfPlot.Plot.Grid.IsVisible = true;

            // Refresh
            wpfPlot.Refresh();

            return wpfPlot;
        }

        public void ConfigurePlot(WpfPlot plotControl, ChannelData channel, PlotConfiguration config)
        {
            // Set height
            plotControl.Height = config.PlotHeight;

            // Configure interactions
            //plotControl.Interaction.EnableZoom = config.EnableZoom;
            //plotControl.Interaction.EnablePan = config.EnablePan;

            // Grid visibility
            plotControl.Plot.Grid.IsVisible = config.ShowGrid;

            // Set X-axis label based on configuration
            if (!string.IsNullOrEmpty(config.XAxisChannelName) && config.XAxisChannelName != "Sample")
            {
                plotControl.Plot.XLabel(config.XAxisChannelName);
            }

            // Refresh
            plotControl.Refresh();
        }

        public void UpdatePlotData(WpfPlot plotControl, ChannelData channel, ObservableCollection<ChannelData> allChannels = null, string xAxisChannelName = null)
        {
            // Clear existing data
            plotControl.Plot.Clear();

            // Determine X-axis data
            double[] xData;
            double[] yData = channel.Values.ToArray();

            // If we have a custom X-axis channel and it's not "Sample"
            if (!string.IsNullOrEmpty(xAxisChannelName) && xAxisChannelName != "Sample" && allChannels != null)
            {
                // Find the X-axis channel
                var xAxisChannel = allChannels.FirstOrDefault(c => c.Name == xAxisChannelName);
                if (xAxisChannel != null)
                {
                    xData = xAxisChannel.Values.ToArray();
                }
                else
                {
                    // Fallback to sample indices
                    xData = Enumerable.Range(0, channel.Values.Count)
                                      .Select(i => (double)i)
                                      .ToArray();
                }
            }
            else
            {
                // Use sample indices as X-axis
                xData = Enumerable.Range(0, channel.Values.Count)
                                  .Select(i => (double)i)
                                  .ToArray();
            }

            // Add new data
            plotControl.Plot.Add.Scatter(xData, yData);

            // Refresh
            plotControl.Refresh();
        }
    }
}

