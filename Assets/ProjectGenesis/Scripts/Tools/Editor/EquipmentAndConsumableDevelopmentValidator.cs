using System;
using System.Collections.Generic;
using System.Linq;
using ProjectGenesis.Data;
using ProjectGenesis.Gameplay;
using ProjectGenesis.Saving;
using ProjectGenesis.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ProjectGenesis.Tools.Editor
{
    public static class EquipmentAndConsumableDevelopmentValidator
    {
        private const string ScenePath =
            "Assets/ProjectGenesis/Scenes/StarterVillage.unity";
        private const string PlayerPrefabPath =
            "Assets/ProjectGenesis/Prefabs/Characters/PF_Player_Prototype.prefab";
        private const string SwordPath =
            "Assets/ProjectGenesis/Data/Items/ITM_RustySword.asset";
        private const string AxePath =
            "Assets/ProjectGenesis/Data/Items/ITM_WornAxe.asset";
        private const string ArmorPath =
            "Assets/ProjectGenesis/Data/Items/ITM_WornLeatherArmor.asset";
        private const string PotionPath =
            "Assets/ProjectGenesis/Data/Items/ITM_MinorHealingPotion.asset";
        private const string WolfLootPath =
            "Assets/ProjectGenesis/Data/LootTables/LT_Wolf.asset";
        private const string BoarLootPath =
            "Assets/ProjectGenesis/Data/LootTables/LT_Boar.asset";

        [MenuItem("Project Genesis/Sprint 021/Validate Equipment And Consumable")]
        public static void ValidateEquipmentAndConsumable()
        {
            ItemDefinition sword = LoadRequiredAsset<ItemDefinition>(SwordPath);
            ItemDefinition axe = LoadRequiredAsset<ItemDefinition>(AxePath);
            ItemDefinition armor = LoadRequiredAsset<ItemDefinition>(ArmorPath);
            ItemDefinition potion = LoadRequiredAsset<ItemDefinition>(PotionPath);
            LootTableDefinition wolfLoot =
                LoadRequiredAsset<LootTableDefinition>(WolfLootPath);
            LootTableDefinition boarLoot =
                LoadRequiredAsset<LootTableDefinition>(BoarLootPath);

            ValidateItemAndLootData(sword, axe, armor, potion, wolfLoot, boarLoot);
            ValidateEquipmentAndUseRules(sword, armor, potion);
            ValidateProfileCompatibility(sword, armor, potion);
            ValidatePrefab(sword, axe, armor, potion);
            ValidateScene();
            Debug.Log("Sprint 021 equipment and consumable validation passed.");
        }

        [MenuItem("Project Genesis/Sprint 021/Validate Relevant Regression Suite")]
        public static void ValidateRelevantRegressionSuite()
        {
            ValidateEquipmentAndConsumable();
            InventoryRearrangementDevelopmentValidator.ValidateInventoryRearrangement();
            ItemInstancesDevelopmentValidator.ValidateItemInstances();
            CharacterStatsDevelopmentValidator.ValidateCharacterStats();
            CharacterEntryDevelopmentValidator.ValidateCharacterEntry();
            CharacterIdentityDevelopmentValidator.ValidateCharacterIdentity();
            SkillsFoundationDevelopmentValidator.ValidateSkillsFoundation();
            EnemyVarietyDevelopmentValidator.ValidateEnemyVariety();
            EnemyExperienceDevelopmentValidator.ValidateEnemyExperience();
            PlayerDeathPenaltyDevelopmentValidator.ValidatePlayerDeathPenalty();
            EnemyTerritoryDevelopmentValidator.ValidateEnemyTerritory();
            CombatRecoveryDevelopmentValidator.ValidateCombatRecovery();
            LootTableDevelopmentValidator.ValidateWolfLootTable();
            QuestSystemDevelopmentValidator.ValidateQuestStateRules();
            Debug.Log("Sprint 021 relevant regression suite passed.");
        }

        [MenuItem("Project Genesis/Sprint 021/Prepare Play Mode Test")]
        public static void PreparePlayModeTest()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("Enter Play Mode before preparing the Sprint 021 test.");
                return;
            }

            PlayerInventory inventory =
                UnityEngine.Object.FindFirstObjectByType<PlayerInventory>();
            Health health = UnityEngine.Object.FindFirstObjectByType<PlayerController>()
                ?.GetComponent<Health>();
            ItemDefinition armor = AssetDatabase.LoadAssetAtPath<ItemDefinition>(ArmorPath);
            ItemDefinition potion = AssetDatabase.LoadAssetAtPath<ItemDefinition>(PotionPath);
            if (inventory == null || health == null || armor == null || potion == null)
            {
                Debug.LogWarning("Could not find the player or Sprint 021 item data.");
                return;
            }

            int addedCount = 0;
            addedCount += inventory.TryAdd(armor) ? 1 : 0;
            addedCount += inventory.TryAdd(potion) ? 1 : 0;
            addedCount += inventory.TryAdd(potion) ? 1 : 0;
            int safeDamage = Mathf.Min(25, Mathf.Max(0, health.CurrentHealth - 1));
            if (safeDamage > 0)
            {
                health.TakeDamage(safeDamage);
            }

            Debug.Log(
                $"Sprint 021 test prepared: added {addedCount} items and dealt " +
                $"{safeDamage} safe damage.");
        }

        private static void ValidateItemAndLootData(
            ItemDefinition sword,
            ItemDefinition axe,
            ItemDefinition armor,
            ItemDefinition potion,
            LootTableDefinition wolfLoot,
            LootTableDefinition boarLoot)
        {
            Require(sword.ItemType == ItemType.Weapon && sword.AttackBonus == 4 &&
                    sword.DefenseBonus == 0 && sword.HealingAmount == 0,
                "Rusty Sword category values are invalid.");
            Require(axe.ItemType == ItemType.Weapon && axe.AttackBonus == 7,
                "Worn Axe category values are invalid.");
            Require(armor.ItemId == "armor.worn_leather" &&
                    armor.DisplayName == "Потёртая кожаная броня" &&
                    armor.ItemType == ItemType.Armor && armor.DefenseBonus == 3 &&
                    armor.AttackBonus == 0 && armor.HealingAmount == 0,
                "Worn Leather Armor must be a stable +3 defense item.");
            Require(potion.ItemId == "consumable.minor_healing_potion" &&
                    potion.DisplayName == "Малое лечебное зелье" &&
                    potion.ItemType == ItemType.Consumable && potion.HealingAmount == 30 &&
                    potion.AttackBonus == 0 && potion.DefenseBonus == 0,
                "Minor Healing Potion must be a stable 30-health consumable.");

            Require(wolfLoot.TryValidate(out string wolfError), wolfError);
            Require(boarLoot.TryValidate(out string boarError), boarError);
            Require(HasEntry(wolfLoot, sword, 0.1f) &&
                    HasEntry(wolfLoot, potion, 0.2f) &&
                    Mathf.Approximately(wolfLoot.TotalDropChance, 0.3f),
                "Wolf loot must contain the sword and potion at 10% and 20%.");
            Require(HasEntry(boarLoot, axe, 0.2f) &&
                    HasEntry(boarLoot, armor, 0.15f) &&
                    Mathf.Approximately(boarLoot.TotalDropChance, 0.35f),
                "Boar loot must contain the axe and armor at 20% and 15%.");
        }

        private static void ValidateEquipmentAndUseRules(
            ItemDefinition sword,
            ItemDefinition armor,
            ItemDefinition potion)
        {
            GameObject probe = new("EquipmentAndConsumableValidation");
            try
            {
                Health health = probe.AddComponent<Health>();
                health.Configure(100);
                PlayerInventory inventory = probe.AddComponent<PlayerInventory>();
                CombatStats stats = probe.AddComponent<CombatStats>();
                stats.Configure(12, 3, 1.35f, 0.8f);
                PlayerEquipment equipment = probe.AddComponent<PlayerEquipment>();
                PlayerItemUseController itemUse =
                    probe.AddComponent<PlayerItemUseController>();

                ItemInstance armorInstance = ItemInstance.Create(armor, "armor-instance");
                ItemInstance firstPotion = ItemInstance.Create(potion, "potion-first");
                ItemInstance secondPotion = ItemInstance.Create(potion, "potion-second");
                ItemInstance swordInstance = ItemInstance.Create(sword, "sword-instance");
                Require(inventory.TryAddInstance(armorInstance) &&
                        inventory.TryAddInstance(firstPotion) &&
                        inventory.TryAddInstance(secondPotion) &&
                        inventory.TryAddInstance(swordInstance),
                    "Validation items must enter the inventory.");

                Require(!equipment.EquipMainHand(armorInstance) &&
                        !equipment.EquipBody(swordInstance),
                    "Equipment slots must reject the wrong item category.");
                Require(equipment.EquipBody(armorInstance) &&
                        equipment.Body == armorInstance &&
                        stats.BaseDefense == 3 &&
                        stats.EquipmentDefenseBonus == 3 && stats.Defense == 6,
                    "Body armor must apply exactly +3 equipment defense.");
                Require(!inventory.Contains(armorInstance) &&
                        equipment.Body == armorInstance && stats.Defense == 6,
                    "Equipped armor must leave the bag and preserve the exact body instance.");

                Require(!itemUse.TryUse(firstPotion, out ItemUseResult fullResult) &&
                        fullResult == ItemUseResult.FullHealth &&
                        inventory.Contains(firstPotion),
                    "Full health must reject the potion without consuming it.");
                Require(!itemUse.TryUse(swordInstance, out ItemUseResult weaponResult) &&
                        weaponResult == ItemUseResult.NotConsumable &&
                        inventory.Contains(swordInstance),
                    "A weapon must not be usable as a consumable.");

                Require(health.TakeDamage(50) &&
                        itemUse.TryUse(firstPotion, out ItemUseResult successResult) &&
                        successResult == ItemUseResult.Success &&
                        health.CurrentHealth == 80 &&
                        !inventory.Contains(firstPotion) &&
                        inventory.Contains(secondPotion),
                    "Successful use must heal 30 and remove only the chosen instance.");
                Require(health.TakeDamage(100) && health.IsDead &&
                        !itemUse.TryUse(secondPotion, out ItemUseResult deadResult) &&
                        deadResult == ItemUseResult.Dead &&
                        inventory.Contains(secondPotion),
                    "Dead players must not consume healing potions.");

                equipment.UnequipBody();
                Require(equipment.Body == null && stats.EquipmentDefenseBonus == 0 &&
                        stats.Defense == 3,
                    "Removing body armor must restore base defense.");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(probe);
            }
        }

        private static void ValidateProfileCompatibility(
            ItemDefinition sword,
            ItemDefinition armor,
            ItemDefinition potion)
        {
            Require(PlayerProfileData.CurrentVersion == 7,
                "Current profile version must be 7.");
            for (int version = 1; version <= 7; version++)
            {
                Require(LocalJsonPlayerPersistence.IsSupportedVersion(version),
                    $"Profile version {version} must remain supported.");
            }

            Require(!LocalJsonPlayerPersistence.IsSupportedVersion(8),
                "Unknown future profile versions must be rejected.");

            PlayerProfileData versionFive = new()
            {
                Version = 5,
                InventoryItems = new List<ItemInstanceData>
                {
                    new() { InstanceId = "v5-armor", ItemId = armor.ItemId, SlotIndex = 3 }
                }
            };
            List<ItemInstance> migrated = PlayerPersistenceController.BuildInventorySlots(
                versionFive,
                itemId => ResolveItem(itemId, sword, armor, potion),
                8);
            Require(migrated[3]?.InstanceId == "v5-armor" &&
                    string.IsNullOrEmpty(PlayerPersistenceController.ResolveBodyInstanceId(
                        versionFive,
                        migrated)),
                "Version-5 positions must migrate with an empty body slot.");

            PlayerProfileData versionSix = new()
            {
                Version = 6,
                InventoryItems = new List<ItemInstanceData>
                {
                    new() { InstanceId = "v6-sword", ItemId = sword.ItemId, SlotIndex = 1 },
                    new() { InstanceId = "v6-armor", ItemId = armor.ItemId, SlotIndex = 4 },
                    new() { InstanceId = "v6-potion", ItemId = potion.ItemId, SlotIndex = 7 }
                },
                MainHandInstanceId = "v6-sword",
                BodyInstanceId = "v6-armor"
            };
            List<ItemInstance> restored = PlayerPersistenceController.BuildInventorySlots(
                versionSix,
                itemId => ResolveItem(itemId, sword, armor, potion),
                8);
            Require(restored[1]?.InstanceId == "v6-sword" &&
                    restored[4]?.InstanceId == "v6-armor" &&
                    restored[7]?.InstanceId == "v6-potion" &&
                    PlayerPersistenceController.ResolveBodyInstanceId(
                        versionSix,
                        restored) == "v6-armor",
                "Version-6 profile must restore positions and exact body equipment.");
        }

        private static void ValidatePrefab(
            ItemDefinition sword,
            ItemDefinition axe,
            ItemDefinition armor,
            ItemDefinition potion)
        {
            GameObject player = LoadRequiredAsset<GameObject>(PlayerPrefabPath);
            PlayerPersistenceController persistence =
                player.GetComponent<PlayerPersistenceController>();
            Require(player.GetComponent<PlayerEquipment>() != null &&
                    player.GetComponent<PlayerItemUseController>() != null,
                "Player prefab must contain equipment and item-use components.");
            Require(persistence != null && persistence.ItemCatalog.Count == 4 &&
                    persistence.ItemCatalog.Contains(sword) &&
                    persistence.ItemCatalog.Contains(axe) &&
                    persistence.ItemCatalog.Contains(armor) &&
                    persistence.ItemCatalog.Contains(potion),
                "Persistence catalog must contain all four item definitions.");
        }

        private static void ValidateScene()
        {
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            InventoryView view = UnityEngine.Object.FindFirstObjectByType<InventoryView>();
            Require(view != null && view.WindowRoot != null && view.SlotCount == 8 &&
                    view.ItemUseController != null,
                "Inventory view must keep eight positions and reference item use.");
            Require(view.WindowRoot.transform.Find("Text_ItemFeedback") != null,
                "Inventory view must contain action feedback text.");
            Require(UnityEngine.Object.FindObjectsByType<EnemySpawner>(
                    FindObjectsSortMode.None).Length >= 3,
                "Starter scene must preserve its three regular enemy spawners.");
        }

        private static bool HasEntry(
            LootTableDefinition lootTable,
            ItemDefinition item,
            float chance)
        {
            return lootTable.Entries.Any(entry =>
                entry != null && entry.Item == item &&
                Mathf.Approximately(entry.DropChance, chance));
        }

        private static ItemDefinition ResolveItem(
            string itemId,
            ItemDefinition sword,
            ItemDefinition armor,
            ItemDefinition potion)
        {
            if (itemId == sword.ItemId)
            {
                return sword;
            }

            if (itemId == armor.ItemId)
            {
                return armor;
            }

            return itemId == potion.ItemId ? potion : null;
        }

        private static T LoadRequiredAsset<T>(string path)
            where T : UnityEngine.Object
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            Require(asset != null, $"Required asset was not found at '{path}'.");
            return asset;
        }

        private static void Require(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException(
                    $"Sprint 021 validation failed: {message}");
            }
        }
    }
}
