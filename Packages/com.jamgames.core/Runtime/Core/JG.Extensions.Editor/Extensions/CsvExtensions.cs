#if !DISABLE_CSV_HELPER
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using UnityEngine;

namespace Core.JG.Extensions.Editor.Extensions
{
    public class CsvExtensions
    {
        const string KeyHeader = "Key";
        
        public static Dictionary<string, List<string>> GetParsedCsv(string text)
        {
            var splittedCsv = SplitCsvGrid(text);
            var result = new Dictionary<string, List<string>>();
            var headers = new Dictionary<int, string>();

            for (int y = 0; y < splittedCsv.GetUpperBound(1); y++)
            for (int x = 0; x < splittedCsv.GetUpperBound(0); x++)
            {
                var element = splittedCsv[x, y];

                if (string.IsNullOrWhiteSpace(element))
                    continue;

                if (y == 0)
                {
                    string h = element;

                    headers[x] = h;
                    result.Add(h, new List<string>());
                }
                else
                {
                    if (headers.TryGetValue(x, out string header))
                        result[header].Add(element);
                }
            }

            return result;
        }

        public static Dictionary<string, Dictionary<string, string>> GetParsedCsvWithKeyOld(string text, Predicate<string> ignoreHeader)
        {
            var splittedCsv = SplitCsvGrid(text);

            var result = new Dictionary<string, Dictionary<string, string>>();
            var headers = new Dictionary<int, string>();
            string key = string.Empty;

            for (int y = 0; y < splittedCsv.GetUpperBound(1); y++)
            for (int x = 0; x < splittedCsv.GetUpperBound(0); x++)
            {
                var element = splittedCsv[x, y];

                if (string.IsNullOrWhiteSpace(element))
                    continue;

                if (y == 0)
                {
                    string h = element;
                    if (ignoreHeader != null && ignoreHeader(h))
                        continue;

                    headers[x] = h;
                    result.Add(h, new Dictionary<string, string>());
                }
                else
                {
                    if (x == 0)
                        key = element;
                    else if (headers.TryGetValue(x, out string header))
                        result[header].Add(key, element);
                }
            }

            return result;
        }

        // splits a CSV file into a 2D string array
        static public string[,] SplitCsvGrid(string csvText)
        {
            string[] lines = csvText.Split("\n"[0]);

            // finds the max width of row
            int width = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                string[] row = SplitCsvLine( lines[i] );
                width = Math.Max(width, row.Length);
            }

            // creates new 2D string grid to output to
            string[,] outputGrid = new string[width + 1, lines.Length + 1];
            for (int y = 0; y < lines.Length; y++)
            {
                string[] row = SplitCsvLine( lines[y] );
                for (int x = 0; x < row.Length; x++)
                {
                    outputGrid[x,y] = row[x];

                    // This line was to replace "" with " in my output.
                    // Include or edit it as you wish.
                    outputGrid[x,y] = outputGrid[x,y].Replace("\"\"", "\"");
                }
            }

            return outputGrid;
        }

        // splits a CSV row
        static public string[] SplitCsvLine(string line)
        {
            return (from System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(line,
                    @"(((?<x>(?=[,\r\n]+))|""(?<x>([^""]|"""")+)""|(?<x>[^,\r\n]+)),?)",
                    System.Text.RegularExpressions.RegexOptions.ExplicitCapture)
                select m.Groups[1].Value).ToArray();
        }

        public static Dictionary<string, Dictionary<string, string>> GetParsedCsvWithKey(string text, Predicate<string> ignoreHeader)
        {
            var result = new Dictionary<string, Dictionary<string, string>>();
            var emptyKeys = new List<string>();
            var config = new CsvConfiguration(CultureInfo.InvariantCulture);
            using (var stringReader = new StringReader(text))
            using (var csvReader = new CsvReader(stringReader, config))
            {
                csvReader.Read();
                var canRead = csvReader.ReadHeader();
                if (!canRead)
                {
                    throw new Exception($"Unable to read headers");
                }

                var headers = csvReader.Context
                    .HeaderRecord
                    .ToList()
                    .Where(header => header != KeyHeader)
                    .Where(header => !ignoreHeader(header));

                foreach (var header in headers)
                {
                    if (!result.ContainsKey(header))
                    {
                        result.Add(header, new Dictionary<string, string>());
                    }
                }

                canRead = csvReader.Read();
                while (canRead)
                {
                    var key = csvReader.GetField(KeyHeader);
                    if (string.IsNullOrEmpty(key))
                    {
                        canRead = csvReader.Read();
                        continue;
                    }

                    foreach (var header in headers)
                    {
                        var field = csvReader.GetField(header);
                        if (string.IsNullOrEmpty(field))
                        {
                            emptyKeys.Add(field);
                            Debug.LogError($"Localization: '{header}' with key '{key}' is empty");
                        }

                        // field = RemoveDiacritics(header, field);
                        if (!result[header].ContainsKey(key))
                        {
                            result[header].Add(key, field);
                        }
                        else
                        {
                            Debug.LogError($"Localization: Duplicate Keys for {header} localization: {key}");
                        }
                    }

                    canRead = csvReader.Read();
                }
            }
            
            if (emptyKeys.Count > 0)
            {
                Debug.LogError("Empty keys are exist");
            }

            return result;
        }
    }
}

#endif