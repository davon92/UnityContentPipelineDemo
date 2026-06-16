using System;

namespace DavonAllen.ContentPipelineDemo
{
    public sealed class ItemCatalogPipelineOptions
    {
        public bool IncludeDisabled { get; set; }
        public DateTime? GeneratedAtUtc { get; set; }
    }
}
