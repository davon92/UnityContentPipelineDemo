using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DavonAllen.ContentPipelineDemo
{
    public sealed class ItemCatalogValidator
    {
        private static readonly Regex IdPattern = new Regex("^[a-z0-9_]+$", RegexOptions.Compiled);

        public IReadOnlyList<PipelineDiagnostic> Validate(IEnumerable<ItemDefinition> items)
        {
            List<PipelineDiagnostic> diagnostics = new List<PipelineDiagnostic>();
            Dictionary<string, ItemDefinition> seenIds = new Dictionary<string, ItemDefinition>(StringComparer.OrdinalIgnoreCase);

            foreach (ItemDefinition item in items)
            {
                ValidateRequiredText(item.Id, "id", "ID_REQUIRED", "Item id is required.", item.SourceLine, diagnostics);
                ValidateRequiredText(item.DisplayName, "displayName", "DISPLAY_NAME_REQUIRED", "Display name is required.", item.SourceLine, diagnostics);
                ValidateRequiredText(item.Category, "category", "CATEGORY_REQUIRED", "Category is required.", item.SourceLine, diagnostics);

                if (!string.IsNullOrEmpty(item.Id) && !IdPattern.IsMatch(item.Id))
                {
                    diagnostics.Add(new PipelineDiagnostic(
                        DiagnosticSeverity.Error,
                        "ID_FORMAT_INVALID",
                        "Item id must use lowercase letters, numbers, and underscores only.",
                        item.SourceLine,
                        "id"));
                }

                if (!string.IsNullOrEmpty(item.Id))
                {
                    ItemDefinition existing;
                    if (seenIds.TryGetValue(item.Id, out existing))
                    {
                        diagnostics.Add(new PipelineDiagnostic(
                            DiagnosticSeverity.Error,
                            "ID_DUPLICATE",
                            "Duplicate item id '" + item.Id + "' also appears on line " + existing.SourceLine + ".",
                            item.SourceLine,
                            "id"));
                    }
                    else
                    {
                        seenIds.Add(item.Id, item);
                    }
                }

                if (item.Power < 0)
                {
                    diagnostics.Add(new PipelineDiagnostic(DiagnosticSeverity.Error, "POWER_NEGATIVE", "Power cannot be negative.", item.SourceLine, "power"));
                }

                if (item.Cost < 0)
                {
                    diagnostics.Add(new PipelineDiagnostic(DiagnosticSeverity.Error, "COST_NEGATIVE", "Cost cannot be negative.", item.SourceLine, "cost"));
                }

                if (string.IsNullOrWhiteSpace(item.IconKey))
                {
                    diagnostics.Add(new PipelineDiagnostic(DiagnosticSeverity.Warning, "ICON_MISSING", "Icon key is empty; UI may show a fallback icon.", item.SourceLine, "iconKey"));
                }

                if (!string.IsNullOrWhiteSpace(item.Description) && item.Description.Length < 12)
                {
                    diagnostics.Add(new PipelineDiagnostic(DiagnosticSeverity.Warning, "DESCRIPTION_SHORT", "Description is short; consider adding useful player-facing context.", item.SourceLine, "description"));
                }

                AddDuplicateTagWarnings(item, diagnostics);
            }

            return diagnostics;
        }

        private static void ValidateRequiredText(
            string value,
            string fieldName,
            string code,
            string message,
            int lineNumber,
            List<PipelineDiagnostic> diagnostics)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            diagnostics.Add(new PipelineDiagnostic(DiagnosticSeverity.Error, code, message, lineNumber, fieldName));
        }

        private static void AddDuplicateTagWarnings(ItemDefinition item, List<PipelineDiagnostic> diagnostics)
        {
            HashSet<string> seenTags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (string tag in item.Tags)
            {
                if (!seenTags.Add(tag))
                {
                    diagnostics.Add(new PipelineDiagnostic(
                        DiagnosticSeverity.Warning,
                        "TAG_DUPLICATE",
                        "Duplicate tag '" + tag + "' on item '" + item.Id + "'.",
                        item.SourceLine,
                        "tags"));
                    return;
                }
            }
        }
    }
}
