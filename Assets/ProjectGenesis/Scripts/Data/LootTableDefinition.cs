using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectGenesis.Data
{
    public enum LootRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic
    }

    [Serializable]
    public sealed class LootTableEntry
    {
        [SerializeField] private ItemDefinition item;
        [SerializeField] private LootRarity rarity = LootRarity.Common;
        [SerializeField, Range(0f, 1f)] private float dropChance = 0.1f;

        public ItemDefinition Item => item;
        public LootRarity Rarity => rarity;
        public float DropChance => Mathf.Clamp01(dropChance);

        public LootTableEntry(ItemDefinition itemDefinition, LootRarity itemRarity, float chance)
        {
            item = itemDefinition;
            rarity = itemRarity;
            dropChance = Mathf.Clamp01(chance);
        }
    }

    [CreateAssetMenu(fileName = "LT_NewLootTable", menuName = "Project Genesis/Loot Table Definition")]
    public sealed class LootTableDefinition : ScriptableObject
    {
        [SerializeField] private List<LootTableEntry> entries = new();

        public IReadOnlyList<LootTableEntry> Entries => entries;

        public float TotalDropChance
        {
            get
            {
                float total = 0f;
                foreach (LootTableEntry entry in entries)
                {
                    if (entry != null)
                    {
                        total += entry.DropChance;
                    }
                }

                return total;
            }
        }

        public bool TryRoll(float normalizedRoll, out LootTableEntry selectedEntry)
        {
            selectedEntry = null;
            float roll = Mathf.Clamp01(normalizedRoll);
            float cumulativeChance = 0f;

            foreach (LootTableEntry entry in entries)
            {
                if (entry == null || entry.Item == null || entry.DropChance <= 0f)
                {
                    continue;
                }

                cumulativeChance += entry.DropChance;
                if (roll < cumulativeChance || cumulativeChance >= 1f)
                {
                    selectedEntry = entry;
                    return true;
                }
            }

            return false;
        }

        public bool TryValidate(out string error)
        {
            if (entries == null || entries.Count == 0)
            {
                error = "Loot table has no entries.";
                return false;
            }

            for (int index = 0; index < entries.Count; index++)
            {
                LootTableEntry entry = entries[index];
                if (entry == null || entry.Item == null)
                {
                    error = $"Loot entry {index} has no item.";
                    return false;
                }

                if (entry.DropChance <= 0f)
                {
                    error = $"Loot entry {index} has a zero drop chance.";
                    return false;
                }
            }

            if (TotalDropChance > 1f + 0.0001f)
            {
                error = $"Total drop chance is {TotalDropChance:P2}, which exceeds 100%.";
                return false;
            }

            error = string.Empty;
            return true;
        }

        public void ConfigureSingleItem(
            ItemDefinition item,
            float chance,
            LootRarity rarity = LootRarity.Common)
        {
            entries = new List<LootTableEntry>
            {
                new(item, rarity, chance)
            };
        }

        public void ConfigureEntries(params LootTableEntry[] configuredEntries)
        {
            entries = configuredEntries != null
                ? new List<LootTableEntry>(configuredEntries)
                : new List<LootTableEntry>();
        }
    }
}
