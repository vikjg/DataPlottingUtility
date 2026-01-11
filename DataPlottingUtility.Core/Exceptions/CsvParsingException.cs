using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPlottingUtility.Core.Exceptions
{
    public class CsvParsingException : Exception
    {
        public int? LineNumber { get; set; }
        public string? LineContent { get; set; }

        public CsvParsingException(string message) : base(message) { }

        public CsvParsingException(string message, int lineNumber, string lineContent)
            : base(message)
        {
            LineNumber = lineNumber;
            LineContent = lineContent;
        }
    }
}
