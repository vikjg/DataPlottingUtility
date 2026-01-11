using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPlottingUtility.Core.Models
{
    /// <summary>
    /// Complete dataset containing all channels and metadata
    /// </summary>
    public class DataSet
    {
        public List<ChannelData> Channels { get; set; } = new List<ChannelData>();
        public FileMetadata Metadata { get; set; } = new FileMetadata();
        public int SampleCount { get; set; }        // Number of data rows
        public DateTime LoadedAt { get; set; }      // When file was loaded
        public string SourceFilePath { get; set; }  // Original file path
    }
}
