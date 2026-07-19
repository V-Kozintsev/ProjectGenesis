using System;
using ProjectGenesis.Data;
using UnityEditor;
using UnityEngine;

namespace ProjectGenesis.Tools.Editor
{
    public static class LootTableDevelopmentValidator
    {
        private const string WolfLootTablePath = "Assets/ProjectGenesis/Data/LootTables/LT_Wolf.asset";
        private const int SimulationRolls = 100000;
        private const int SimulationSeed = 9009;

        [MenuItem("Project Genesis/Sprint 009/Validate Wolf Loot Table")]
        public static void ValidateWolfLootTable()
        {
            LootTableDefinition lootTable = AssetDatabase.LoadAssetAtPath<LootTableDefinition>(WolfLootTablePath);
            Require(lootTable != null, $"Missing asset at {WolfLootTablePath}.");
            Require(lootTable.TryValidate(out string validationError), validationError);
            Require(lootTable.Entries.Count == 1, "The prototype wolf table should contain one item.");

            LootTableEntry swordEntry = lootTable.Entries[0];
            Require(swordEntry.Item.ItemId == "weapon.rusty_sword", "Wolf table does not contain the rusty sword.");
            Require(Mathf.Approximately(swordEntry.DropChance, 0.1f), "Rusty sword chance is not 10%.");
            Require(Mathf.Approximately(lootTable.TotalDropChance, 0.1f), "Wolf total regular-loot chance is not 10%.");
            Require(lootTable.TryRoll(0f, out LootTableEntry lowRoll) && lowRoll == swordEntry, "A low roll did not drop the sword.");
            Require(!lootTable.TryRoll(0.1f, out _), "The no-drop range does not begin at 10%.");
            Require(!lootTable.TryRoll(0.999f, out _), "A high roll unexpectedly produced loot.");
            ValidateGuaranteedTestRoll(swordEntry.Item);

            System.Random random = new(SimulationSeed);
            int drops = 0;
            for (int index = 0; index < SimulationRolls; index++)
            {
                if (lootTable.TryRoll((float)random.NextDouble(), out _))
                {
                    drops++;
                }
            }

            float observedRate = drops / (float)SimulationRolls;
            Require(
                Mathf.Abs(observedRate - 0.1f) <= 0.005f,
                $"Fixed-seed simulation produced {observedRate:P2}, outside the 9.5%-10.5% range.");

            Debug.Log($"Sprint 009 wolf loot validation passed: {drops} drops from {SimulationRolls} rolls ({observedRate:P2}).");
        }

        private static void ValidateGuaranteedTestRoll(ItemDefinition item)
        {
            LootTableDefinition guaranteedTable = ScriptableObject.CreateInstance<LootTableDefinition>();
            try
            {
                guaranteedTable.ConfigureSingleItem(item, 1f);
                Require(
                    guaranteedTable.TryRoll(1f, out LootTableEntry entry) && entry.Item == item,
                    "A temporary 100% test chance did not guarantee the item.");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(guaranteedTable);
            }
        }

        private static void Require(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException($"Sprint 009 validation failed: {message}");
            }
        }
    }
}
