using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPlottingUtility.Core.Models
{
    /// <summary>
    /// Represents a single data channel with its metadata and values
    /// </summary>
    public class ChannelData
    {
        public string Name { get; set; }           // From header line
        public string Unit { get; set; }           // From units line
        public List<double> Values { get; set; }   // The actual data points
        public int Index { get; set; }             // Column index in CSV
    }
}
