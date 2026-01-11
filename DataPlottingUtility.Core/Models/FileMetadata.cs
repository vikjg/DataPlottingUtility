using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPlottingUtility.Core.Models
{
    /// <summary>
    /// Metadata extracted from keyword lines in CSV
    /// </summary>
    public class FileMetadata
    {
        public double? SampleRate { get; set; }    // From \SAMPLE RATE =
        public string? Title { get; set; }         // From \TITLE =
        public string? FileName { get; set; }      // From \FILENAME =

        // You might add more later
        public Dictionary<string, string> AdditionalKeywords { get; set; }
            = new Dictionary<string, string>();
    }
}
