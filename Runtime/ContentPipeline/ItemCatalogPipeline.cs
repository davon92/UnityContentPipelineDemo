using System;
using System.Collections.Generic;
using System.Linq;

namespace DavonAllen.ContentPipelineDemo
{
    public sealed class ItemCatalogPipeline
    {
        private readonly ItemCsvParser parser;
        private readonly ItemCatalogValidator validator;
        private readonly ItemCatalogJsonExporter exporter;

        public ItemCatalogPipeline()
            : this(new ItemCsvParser(), new ItemCatalogValidator(), new ItemCatalogJsonExporter())
        {
        }

        public ItemCatalogPipeline(ItemCsvParser parser, ItemCatalogValidator validator, ItemCatalogJsonExporter exporter)
        {
            this.parser = parser;
            this.validator = validator;
            this.exporter = exporter;
        }

        public ItemCatalogPipelineResult Run(string csvText, ItemCatalogPipelineOptions options)
        {
            options = options ?? new ItemCatalogPipelineOptions();

            ItemCatalogPipelineResult parseResult = parser.Parse(csvText);
            List<PipelineDiagnostic> diagnostics = new List<PipelineDiagnostic>(parseResult.Diagnostics);
            diagnostics.AddRange(validator.Validate(parseResult.Items));

            bool hasErrors = diagnostics.Any(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error);
            List<ItemDefinition> outputItems = parseResult.Items
                .Where(item => options.IncludeDisabled || item.Enabled)
                .ToList();

            string generatedJson = hasErrors
                ? string.Empty
                : exporter.Export(outputItems, options.GeneratedAtUtc ?? DateTime.UtcNow);

            return new ItemCatalogPipelineResult(outputItems, diagnostics, generatedJson);
        }
    }
}
