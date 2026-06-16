namespace DavonAllen.ContentPipelineDemo
{
    public sealed class ItemDefinition
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string Category { get; set; }
        public ItemRarity Rarity { get; set; }
        public int Power { get; set; }
        public int Cost { get; set; }
        public string IconKey { get; set; }
        public string Description { get; set; }
        public string[] Tags { get; set; }
        public bool Enabled { get; set; }
        public int SourceLine { get; set; }

        public ItemDefinition()
        {
            Id = string.Empty;
            DisplayName = string.Empty;
            Category = string.Empty;
            IconKey = string.Empty;
            Description = string.Empty;
            Tags = new string[0];
            Enabled = true;
        }
    }
}
