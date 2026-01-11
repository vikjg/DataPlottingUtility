using System.Drawing;
using System.Windows.Media;

namespace DataPlottingUtility.Plotting.Models
{
    public class PlotConfiguration
    {
        public int PlotHeight { get; set; } = 200;
        public bool ShareXAxis { get; set; } = true;
        public bool ShowGrid { get; set; } = true;
        public System.Windows.Media.Color BackgroundColor { get; set; } = Colors.White;
        public string FontFamily { get; set; } = "Segoe UI";
        public string XAxisChannelName { get; set; } = string.Empty; 
    }

    public class ChannelPlotSettings
    {
        public string ChannelName { get; set; } = string.Empty;
        public  System.Windows.Media.Color LineColor { get; set; } = Colors.Blue;
        public double LineWidth { get; set; } = 2.0;
        public string Title { get; set; } = string.Empty;
        public bool ShowGrid { get; set; } = true;
    }
}

