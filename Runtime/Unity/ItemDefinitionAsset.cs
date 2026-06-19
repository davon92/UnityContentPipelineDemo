using UnityEngine;

namespace DavonAllen.ContentPipelineDemo
{
    [CreateAssetMenu(menuName = "Davon/Content Pipeline/Item Definition")]
    public sealed class ItemDefinitionAsset : ScriptableObject
    {
        [SerializeField] private string itemId;
        [SerializeField] private string displayName;
        [SerializeField] private string category;
        [SerializeField] private ItemRarity rarity;
        [SerializeField] private int power;
        [SerializeField] private int cost;
        [SerializeField] private int maxStackSize;
        [SerializeField] private string iconKey;
        [SerializeField] private string description;
        [SerializeField] private string[] tags;
        [SerializeField] private bool enabledForCatalog;

        public string ItemId { get { return itemId; } }
        public string DisplayName { get { return displayName; } }
        public string Category { get { return category; } }
        public ItemRarity Rarity { get { return rarity; } }
        public int Power { get { return power; } }
        public int Cost { get { return cost; } }
        public int MaxStackSize { get { return maxStackSize; } }
        public string IconKey { get { return iconKey; } }
        public string Description { get { return description; } }
        public string[] Tags { get { return tags; } }
        public bool EnabledForCatalog { get { return enabledForCatalog; } }

        public void Apply(ItemDefinition item)
        {
            itemId = item.Id;
            displayName = item.DisplayName;
            category = item.Category;
            rarity = item.Rarity;
            power = item.Power;
            cost = item.Cost;
            maxStackSize = item.MaxStackSize;
            iconKey = item.IconKey;
            description = item.Description;
            tags = item.Tags;
            enabledForCatalog = item.Enabled;
        }
    }
}
