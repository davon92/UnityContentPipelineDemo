using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DavonAllen.ContentPipelineDemo
{
    public sealed class ItemCsvParser
    {
        private static readonly string[] RequiredHeaders =
        {
            "id",
            "displayname",
            "category",
            "rarity",
            "power",
            "cost",
            "maxStackSize",
            "iconkey",
            "description",
            "tags",
            "enabled"
        };

        public ItemCatalogPipelineResult Parse(string csvText)
        {
            List<ItemDefinition> items = new List<ItemDefinition>();
            List<PipelineDiagnostic> diagnostics = new List<PipelineDiagnostic>();

            if (string.IsNullOrWhiteSpace(csvText))
            {
                diagnostics.Add(new PipelineDiagnostic(DiagnosticSeverity.Error, "CSV_EMPTY", "CSV input is empty.", 0, string.Empty));
                return new ItemCatalogPipelineResult(items, diagnostics, string.Empty);
            }

            string normalized = csvText.Replace("\r\n", "\n").Replace('\r', '\n');
            string[] lines = normalized.Split('\n');
            int headerLineIndex = FindHeaderLineIndex(lines);

            if (headerLineIndex < 0)
            {
                diagnostics.Add(new PipelineDiagnostic(DiagnosticSeverity.Error, "CSV_HEADER_MISSING", "CSV header row was not found.", 0, string.Empty));
                return new ItemCatalogPipelineResult(items, diagnostics, string.Empty);
            }

            IReadOnlyList<string> headers = CsvLineParser.ParseLine(lines[headerLineIndex])
                .Select(header => NormalizeHeader(header))
                .ToList();

            ValidateHeaders(headers, diagnostics, headerLineIndex + 1);

            for (int lineIndex = headerLineIndex + 1; lineIndex < lines.Length; lineIndex++)
            {
                string line = lines[lineIndex];

                if (ShouldSkipLine(line))
                {
                    continue;
                }

                IReadOnlyList<string> values = CsvLineParser.ParseLine(line);
                Dictionary<string, string> row = BuildRow(headers, values);
                int lineNumber = lineIndex + 1;

                ItemDefinition item = new ItemDefinition
                {
                    Id = GetValue(row, "id"),
                    DisplayName = GetValue(row, "displayname"),
                    Category = GetValue(row, "category"),
                    Rarity = ParseRarity(GetValue(row, "rarity"), diagnostics, lineNumber),
                    Power = ParseInt(GetValue(row, "power"), "power", diagnostics, lineNumber),
                    Cost = ParseInt(GetValue(row, "cost"), "cost", diagnostics, lineNumber),
                    MaxStackSize = ParseInt(GetValue(row, "maxStackSize"), "maxStackSize", diagnostics, lineNumber),
                    IconKey = GetValue(row, "iconkey"),
                    Description = GetValue(row, "description"),
                    Tags = ParseTags(GetValue(row, "tags")),
                    Enabled = ParseBool(GetValue(row, "enabled"), diagnostics, lineNumber),
                    SourceLine = lineNumber
                };

                items.Add(item);
            }

            return new ItemCatalogPipelineResult(items, diagnostics, string.Empty);
        }

        private static int FindHeaderLineIndex(string[] lines)
        {
            for (int index = 0; index < lines.Length; index++)
            {
                if (!ShouldSkipLine(lines[index]))
                {
                    return index;
                }
            }

            return -1;
        }

        private static bool ShouldSkipLine(string line)
        {
            string trimmed = (line ?? string.Empty).Trim();
            return string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("#", StringComparison.Ordinal);
        }

        private static void ValidateHeaders(IReadOnlyList<string> headers, List<PipelineDiagnostic> diagnostics, int lineNumber)
        {
            HashSet<string> headerSet = new HashSet<string>(headers);

            foreach (string requiredHeader in RequiredHeaders)
            {
                if (!headerSet.Contains(requiredHeader))
                {
                    diagnostics.Add(new PipelineDiagnostic(
                        DiagnosticSeverity.Error,
                        "CSV_REQUIRED_COLUMN",
                        "Required column '" + requiredHeader + "' is missing.",
                        lineNumber,
                        requiredHeader));
                }
            }
        }

        private static Dictionary<string, string> BuildRow(IReadOnlyList<string> headers, IReadOnlyList<string> values)
        {
            Dictionary<string, string> row = new Dictionary<string, string>();

            for (int index = 0; index < headers.Count; index++)
            {
                string value = index < values.Count ? values[index] : string.Empty;
                row[headers[index]] = value;
            }

            return row;
        }

        private static string NormalizeHeader(string header)
        {
            return (header ?? string.Empty).Trim().Replace(" ", string.Empty).ToLowerInvariant();
        }

        private static string GetValue(Dictionary<string, string> row, string key)
        {
            string value;
            return row.TryGetValue(key, out value) ? value.Trim() : string.Empty;
        }

        private static ItemRarity ParseRarity(string value, List<PipelineDiagnostic> diagnostics, int lineNumber)
        {
            ItemRarity rarity;

            if (Enum.TryParse(value, true, out rarity))
            {
                return rarity;
            }

            diagnostics.Add(new PipelineDiagnostic(
                DiagnosticSeverity.Error,
                "RARITY_INVALID",
                "Rarity '" + value + "' is not supported.",
                lineNumber,
                "rarity"));

            return ItemRarity.Common;
        }

        private static int ParseInt(string value, string fieldName, List<PipelineDiagnostic> diagnostics, int lineNumber)
        {
            int parsedValue;

            if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedValue))
            {
                return parsedValue;
            }

            diagnostics.Add(new PipelineDiagnostic(
                DiagnosticSeverity.Error,
                "NUMBER_INVALID",
                "Field '" + fieldName + "' must be an integer.",
                lineNumber,
                fieldName));

            return 0;
        }

        private static bool ParseBool(string value, List<PipelineDiagnostic> diagnostics, int lineNumber)
        {
            string normalized = (value ?? string.Empty).Trim().ToLowerInvariant();

            if (normalized == "true" || normalized == "yes" || normalized == "1")
            {
                return true;
            }

            if (normalized == "false" || normalized == "no" || normalized == "0")
            {
                return false;
            }

            diagnostics.Add(new PipelineDiagnostic(
                DiagnosticSeverity.Error,
                "BOOL_INVALID",
                "Enabled must be true/false, yes/no, or 1/0.",
                lineNumber,
                "enabled"));

            return false;
        }

        private static string[] ParseTags(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return new string[0];
            }

            return value
                .Split(new[] { ';', '|' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(tag => tag.Trim())
                .Where(tag => !string.IsNullOrEmpty(tag))
                .ToArray();
        }
    }
}
