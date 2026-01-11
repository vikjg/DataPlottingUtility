using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataPlottingUtility.Core.Models;

namespace DataPlottingUtility.Core.Interfaces
{
    /// <summary>
    /// Interface for CSV parsing operations
    /// </summary>
    public interface ICsvParser
    {
        /// <summary>
        /// Parse a CSV file from the given path
        /// </summary>
        Task<ParsingResult> ParseFileAsync(string filePath);

        /// <summary>
        /// Parse CSV data from a stream
        /// </summary>
        Task<ParsingResult> ParseStreamAsync(Stream stream);

        /// <summary>
        /// Validate if a file matches the expected format
        /// </summary>
        Task<bool> ValidateFormatAsync(string filePath);
    }
}
