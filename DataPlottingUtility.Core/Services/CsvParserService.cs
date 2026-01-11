using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DataPlottingUtility.Core.Interfaces;
using DataPlottingUtility.Core.Models;
using DataPlottingUtility.Core.Exceptions;

namespace DataPlottingUtility.Core.Services
{
    /// <summary>
    /// Service for parsing custom CSV files with channel data
    /// </summary>
    public class CsvParserService : ICsvParser
    {
        /// <summary>
        /// Parse a CSV file from the given path
        /// </summary>
        public async Task<ParsingResult> ParseFileAsync(string filePath)
        {
            try
            {
                // Validate file exists
                if (!File.Exists(filePath))
                {
                    return ParsingResult.CreateFailure($"File not found: {filePath}");
                }

                // Open file stream and parse
                using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                var result = await ParseStreamAsync(fileStream);

                // Set source file path if successful
                if (result.Success && result.Data != null)
                {
                    result.Data.SourceFilePath = filePath;
                }

                return result;
            }
            catch (Exception ex)
            {
                return ParsingResult.CreateFailure($"Error reading file: {ex.Message}");
            }
        }

        /// <summary>
        /// Parse CSV data from a stream
        /// </summary>
        public async Task<ParsingResult> ParseStreamAsync(Stream stream)
        {
            var warnings = new List<string>();
            int lineNumber = 0;

            try
            {
                using var reader = new StreamReader(stream);

                // Step 1: Read and parse header line
                lineNumber = 1;
                var headerLine = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(headerLine))
                {
                    return ParsingResult.CreateFailure("File is empty or header line is missing");
                }

                var channelNames = ParseHeaderLine(headerLine);
                if (channelNames.Count == 0)
                {
                    return ParsingResult.CreateFailure("No channels found in header line");
                }

                // Step 2: Read and parse units line
                lineNumber = 2;
                var unitsLine = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(unitsLine))
                {
                    return ParsingResult.CreateFailure("Units line is missing");
                }

                var units = ParseUnitsLine(unitsLine);
                if (units.Count != channelNames.Count)
                {
                    warnings.Add($"Units count ({units.Count}) doesn't match channels count ({channelNames.Count})");
                    // Pad with empty strings if needed
                    while (units.Count < channelNames.Count)
                    {
                        units.Add(string.Empty);
                    }
                }

                // Step 3: Create ChannelData objects
                var channels = CreateChannels(channelNames, units);

                // Step 4: Parse keyword lines
                var metadata = new FileMetadata();
                string? line;
                lineNumber = 3;

                while ((line = await reader.ReadLineAsync()) != null)
                {
                    lineNumber++;

                    // Skip empty lines
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    // Check if this is a keyword line
                    if (!line.StartsWith("\\"))
                        break; // End of keywords, this is first data line

                    try
                    {
                        ParseKeywordLine(line, metadata);
                    }
                    catch (Exception ex)
                    {
                        warnings.Add($"Line {lineNumber}: Failed to parse keyword - {ex.Message}");
                    }
                }

                // Step 5: Parse data lines (line is first data line, or null if no data)
                int rowCount = 0;

                if (line != null)
                {
                    // Process first data line
                    do
                    {
                        if (string.IsNullOrWhiteSpace(line))
                            continue;

                        try
                        {
                            ParseDataLine(line, channels, rowCount);
                            rowCount++;
                        }
                        catch (Exception ex)
                        {
                            warnings.Add($"Line {lineNumber}: Failed to parse data - {ex.Message}");
                        }

                        lineNumber++;
                    }
                    while ((line = await reader.ReadLineAsync()) != null);
                }

                if (rowCount == 0)
                {
                    return ParsingResult.CreateFailure("No data rows found in file");
                }

                // Step 6: Build result
                var dataSet = new DataSet
                {
                    Channels = channels,
                    Metadata = metadata,
                    SampleCount = rowCount,
                    LoadedAt = DateTime.Now
                };

                var result = ParsingResult.CreateSuccess(dataSet);
                result.Warnings = warnings;

                return result;
            }
            catch (CsvParsingException ex)
            {
                return ParsingResult.CreateFailure($"Line {ex.LineNumber ?? lineNumber}: {ex.Message}");
            }
            catch (Exception ex)
            {
                return ParsingResult.CreateFailure($"Parsing error at line {lineNumber}: {ex.Message}");
            }
        }

        /// <summary>
        /// Validate if a file matches the expected format
        /// </summary>
        public async Task<bool> ValidateFormatAsync(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    return false;

                using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                using var reader = new StreamReader(fileStream);

                // Check for header line
                var headerLine = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(headerLine))
                    return false;

                // Check for units line
                var unitsLine = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(unitsLine))
                    return false;

                // Check for at least one keyword line
                var keywordLine = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(keywordLine) || !keywordLine.StartsWith("\\"))
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Parse the header line to extract channel names
        /// </summary>
        private List<string> ParseHeaderLine(string line)
        {
            return line.Split(',')
                       .Select(s => s.Trim())
                       .Where(s => !string.IsNullOrWhiteSpace(s))
                       .ToList();
        }

        /// <summary>
        /// Parse the units line to extract units for each channel
        /// </summary>
        private List<string> ParseUnitsLine(string line)
        {
            return line.Split(',')
                       .Select(s => s.Trim())
                       .ToList();
        }

        /// <summary>
        /// Create ChannelData objects from names and units
        /// </summary>
        private List<ChannelData> CreateChannels(List<string> names, List<string> units)
        {
            var channels = new List<ChannelData>();

            for (int i = 0; i < names.Count; i++)
            {
                channels.Add(new ChannelData
                {
                    Name = names[i],
                    Unit = i < units.Count ? units[i] : string.Empty,
                    Index = i,
                    Values = new List<double>()
                });
            }

            return channels;
        }

        /// <summary>
        /// Parse a keyword line and update metadata
        /// </summary>
        private void ParseKeywordLine(string line, FileMetadata metadata)
        {
            // Remove leading backslash
            var content = line.TrimStart('\\').Trim();

            // Split on equals sign
            var parts = content.Split(new[] { '=' }, 2);
            if (parts.Length != 2)
            {
                // Store as additional keyword with no value
                metadata.AdditionalKeywords[content] = string.Empty;
                return;
            }

            var keyword = parts[0].Trim().ToUpperInvariant();
            var value = parts[1].Trim();

            // Handle known keywords
            switch (keyword)
            {
                case "SAMPLE RATE":
                    if (double.TryParse(value, out var sampleRate))
                    {
                        metadata.SampleRate = sampleRate;
                    }
                    else
                    {
                        throw new CsvParsingException($"Invalid sample rate value: {value}");
                    }
                    break;

                case "TITLE":
                    metadata.Title = value;
                    break;

                case "FILENAME":
                    metadata.FileName = value;
                    break;

                default:
                    // Store unknown keywords
                    metadata.AdditionalKeywords[keyword] = value;
                    break;
            }
        }

        /// <summary>
        /// Parse a data line and add values to channels
        /// </summary>
        private void ParseDataLine(string line, List<ChannelData> channels, int rowIndex)
        {
            var values = line.Split(',').Select(s => s.Trim()).ToArray();

            if (values.Length != channels.Count)
            {
                throw new CsvParsingException(
                    $"Data row has {values.Length} values but {channels.Count} channels expected");
            }

            for (int i = 0; i < channels.Count; i++)
            {
                if (double.TryParse(values[i], out var numericValue))
                {
                    channels[i].Values.Add(numericValue);
                }
                else if (string.IsNullOrWhiteSpace(values[i]))
                {
                    // Empty value - use NaN
                    channels[i].Values.Add(double.NaN);
                }
                else
                {
                    // Invalid numeric value - use NaN and note the issue
                    channels[i].Values.Add(double.NaN);
                    throw new CsvParsingException(
                        $"Invalid numeric value '{values[i]}' for channel '{channels[i].Name}' at row {rowIndex + 1}");
                }
            }
        }
    }
}
