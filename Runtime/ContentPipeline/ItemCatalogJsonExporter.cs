using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace DavonAllen.ContentPipelineDemo
{
    public sealed class ItemCatalogJsonExporter
    {
        public string Export(IEnumerable<ItemDefinition> items, DateTime generatedAtUtc)
        {
            List<ItemDefinition> orderedItems = items
                .OrderBy(item => item.Id, StringComparer.OrdinalIgnoreCase)
                .ToList();

            StringBuilder json = new StringBuilder();
            json.AppendLine("{");
            json.AppendLine("  \"generatedAtUtc\": \"" + Escape(generatedAtUtc.ToString("o", CultureInfo.InvariantCulture)) + "\",");
            json.AppendLine("  \"itemCount\": " + orderedItems.Count + ",");
            json.AppendLine("  \"items\": [");

            for (int index = 0; index < orderedItems.Count; index++)
            {
                ItemDefinition item = orderedItems[index];
                json.AppendLine("    {");
                json.AppendLine("      \"id\": \"" + Escape(item.Id) + "\",");
                json.AppendLine("      \"displayName\": \"" + Escape(item.DisplayName) + "\",");
                json.AppendLine("      \"category\": \"" + Escape(item.Category) + "\",");
                json.AppendLine("      \"rarity\": \"" + item.Rarity + "\",");
                json.AppendLine("      \"power\": " + item.Power + ",");
                json.AppendLine("      \"cost\": " + item.Cost + ",");
                json.AppendLine("      \"maxStackSize\": " + item.MaxStackSize + ",");
                json.AppendLine("      \"iconKey\": \"" + Escape(item.IconKey) + "\",");
                json.AppendLine("      \"description\": \"" + Escape(item.Description) + "\",");
                json.AppendLine("      \"tags\": [" + FormatTags(item.Tags) + "],");
                json.AppendLine("      \"enabled\": " + (item.Enabled ? "true" : "false"));
                json.Append("    }");

                if (index < orderedItems.Count - 1)
                {
                    json.Append(",");
                }

                json.AppendLine();
            }

            json.AppendLine("  ]");
            json.AppendLine("}");
            return json.ToString();
        }

        private static string FormatTags(IEnumerable<string> tags)
        {
            return string.Join(", ", tags.Select(tag => "\"" + Escape(tag) + "\"").ToArray());
        }

        private static string Escape(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            return value
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t");
        }
    }
}
