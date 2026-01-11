using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPlottingUtility.Core.Models
{
    /// <summary>
    /// Wrapper for parsing results with success/error information
    /// </summary>
    public class ParsingResult
    {
        public bool Success { get; set; }
        public DataSet? Data { get; set; }
        public string? ErrorMessage { get; set; }
        public List<string> Warnings { get; set; } = new List<string>();

        public static ParsingResult CreateSuccess(DataSet data)
            => new ParsingResult { Success = true, Data = data };

        public static ParsingResult CreateFailure(string error)
            => new ParsingResult { Success = false, ErrorMessage = error };
    }
}
