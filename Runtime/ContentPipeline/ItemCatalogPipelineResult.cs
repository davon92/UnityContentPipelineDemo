using System.Collections.Generic;
using System.Linq;

namespace DavonAllen.ContentPipelineDemo
{
    public sealed class ItemCatalogPipelineResult
    {
        public ItemCatalogPipelineResult(
            IReadOnlyList<ItemDefinition> items,
            IReadOnlyList<PipelineDiagnostic> diagnostics,
            string generatedJson)
        {
            Items = items;
            Diagnostics = diagnostics;
            GeneratedJson = generatedJson ?? string.Empty;
        }

        public IReadOnlyList<ItemDefinition> Items { get; private set; }
        public IReadOnlyList<PipelineDiagnostic> Diagnostics { get; private set; }
        public string GeneratedJson { get; private set; }

        public bool HasErrors
        {
            get { return Diagnostics.Any(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error); }
        }

        public bool HasWarnings
        {
            get { return Diagnostics.Any(diagnostic => diagnostic.Severity == DiagnosticSeverity.Warning); }
        }
    }
}
