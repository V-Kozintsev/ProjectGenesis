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
    public static class InventoryRearrangementDevelopmentValidator
    {
        private const string ScenePath = "Assets/ProjectGenesis/Scenes/StarterVillage.unity";
        private const string PlayerPrefabPath =
            "Assets/ProjectGenesis/Prefabs/Characters/PF_Player_Prototype.prefab";
        private const string BoarPrefabPath =
            "Assets/ProjectGenesis/Prefabs/Enemies/PF_Enemy_Boar.prefab";
        private const string SwordPath =
            "Assets/ProjectGenesis/Data/Items/ITM_RustySword.asset";
        private const string AxePath =
            "Assets/ProjectGenesis/Data/Items/ITM_WornAxe.asset";
        private const string BoarLootTablePath =
            "Assets/ProjectGenesis/Data/LootTables/LT_Boar.asset";

        [MenuItem("Project Genesis/Sprint 020/Validate Inventory Rearrangement")]
        public static void ValidateInventoryRearrangement()
        {
            ItemDefinition sword = LoadRequiredAsset<ItemDefinition>(SwordPath);
            ItemDefinition axe = LoadRequiredAsset<ItemDefinition>(AxePath);
            LootTableDefinition boarLoot =
                LoadRequiredAsset<LootTableDefinition>(BoarLootTablePath);

            ValidateItemAndLootData(sword, axe, boarLoot);
            ValidateMovementAndEquipment(sword, axe);
            ValidateProfilePositions(sword, axe);
            ValidatePrefabs(sword, axe, boarLoot);
            ValidateScene();
            Debug.Log("Sprint 020 inventory rearrangement validation passed.");
        }

        [MenuItem("Project Genesis/Sprint 020/Add Sword And Axe In Play Mode")]
        public static void AddSwordAndAxeInPlayMode()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("Enter Play Mode before adding test weapons.");
                return;
            }

            PlayerInventory inventory =
                UnityEngine.Object.FindFirstObjectByType<PlayerInventory>();
            ItemDefinition sword = AssetDatabase.LoadAssetAtPath<ItemDefinition>(SwordPath);
            ItemDefinition axe = AssetDatabase.LoadAssetAtPath<ItemDefinition>(AxePath);
            if (inventory == null || sword == null || axe == null)
            {
                Debug.LogWarning("Could not find the inventory or weapon definitions.");
                return;
            }

            int addedCount = 0;
            addedCount += inventory.TryAdd(sword) ? 1 : 0;
            addedCount += inventory.TryAdd(axe) ? 1 : 0;
            Debug.Log($"Added {addedCount} test weapons for Sprint 020 Play Mode testing.");
        }

        private static void ValidateItemAndLootData(
            ItemDefinition sword,
            ItemDefinition axe,
            LootTableDefinition boarLoot)
        {
            Require(sword.ItemId == "weapon.rusty_sword" && sword.AttackBonus == 4,
                "Rusty Sword data changed unexpectedly.");
            Require(axe.ItemId == "weapon.worn_axe" &&
                    axe.DisplayName == "Потёртый топор" &&
                    axe.ItemType == ItemType.Weapon && axe.AttackBonus == 7,
                "Worn Axe must be a +7 weapon with stable authored data.");
            Require(boarLoot.TryValidate(out string lootError),
                $"Boar loot table is invalid: {lootError}");
            Require(boarLoot.Entries.Count >= 1 &&
                    boarLoot.Entries[0].Item == axe &&
                    Mathf.Approximately(boarLoot.Entries[0].DropChance, 0.2f),
                "Boar loot must contain the Worn Axe at 20% probability.");
        }

        private static void ValidateMovementAndEquipment(
            ItemDefinition sword,
            ItemDefinition axe)
        {
            GameObject probe = new("InventoryRearrangementValidation");
            try
            {
                PlayerInventory inventory = probe.AddComponent<PlayerInventory>();
                CombatStats stats = probe.AddComponent<CombatStats>();
                stats.Configure(12, 3, 1.35f, 0.8f);
                PlayerEquipment equipment = probe.AddComponent<PlayerEquipment>();

                ItemInstance swordInstance = ItemInstance.Create(sword, "move-sword");
                ItemInstance axeInstance = ItemInstance.Create(axe, "move-axe");
                Require(inventory.TryAddInstance(swordInstance) &&
                        inventory.TryAddInstance(axeInstance),
                    "Sword and axe must enter consecutive empty slots.");
                Require(inventory.Count == 2 &&
                        inventory.GetItemAt(0) == swordInstance &&
                        inventory.GetItemAt(1) == axeInstance &&
                        inventory.GetItemAt(2) == null,
                    "Inventory must expose occupied and empty fixed positions.");

                Require(inventory.TryMoveOrSwap(0, 6),
                    "An item must move into a distant empty slot.");
                Require(inventory.GetItemAt(0) == null &&
                        inventory.GetItemAt(6) == swordInstance &&
                        inventory.Count == 2,
                    "Moving must preserve the exact instance and occupied count.");

                Require(equipment.EquipMainHand(axeInstance) &&
                        stats.EquipmentAttackBonus == 7,
                    "Worn Axe must apply its +7 attack bonus.");
                Require(inventory.TryMoveOrSwap(1, 6),
                    "Two occupied slots must swap.");
                Require(inventory.GetItemAt(1) == swordInstance &&
                        inventory.GetItemAt(6) == axeInstance,
                    "Swap must exchange the exact sword and axe instances.");
                Require(equipment.MainHand == axeInstance &&
                        stats.EquipmentAttackBonus == 7,
                    "Moving an equipped axe must preserve equipment and its bonus.");

                Require(equipment.EquipMainHand(swordInstance) &&
                        stats.EquipmentAttackBonus == 4,
                    "Selecting the sword must replace the axe bonus with +4.");
                Require(!inventory.TryMoveOrSwap(0, 3) &&
                        !inventory.TryMoveOrSwap(-1, 3) &&
                        !inventory.TryMoveOrSwap(1, 1),
                    "Empty, invalid, and same-slot moves must be rejected.");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(probe);
            }
        }

        private static void ValidateProfilePositions(
            ItemDefinition sword,
            ItemDefinition axe)
        {
            Require(PlayerProfileData.CurrentVersion == 6,
                "Current profile version must be 6.");
            for (int version = 1; version <= 6; version++)
            {
                Require(LocalJsonPlayerPersistence.IsSupportedVersion(version),
                    $"Profile version {version} must remain supported.");
            }

            PlayerProfileData versionFour = new()
            {
                Version = 4,
                InventoryItems = new List<ItemInstanceData>
                {
                    new() { InstanceId = "v4-sword", ItemId = sword.ItemId },
                    new() { InstanceId = "v4-axe", ItemId = axe.ItemId }
                }
            };
            List<ItemInstance> migrated = PlayerPersistenceController.BuildInventorySlots(
                versionFour,
                itemId => ResolveItem(itemId, sword, axe),
                8);
            Require(migrated.Count == 8 &&
                    migrated[0]?.InstanceId == "v4-sword" &&
                    migrated[1]?.InstanceId == "v4-axe" &&
                    migrated[2] == null,
                "Version-4 item order must migrate into consecutive fixed positions.");

            PlayerProfileData versionFive = new()
            {
                Version = 5,
                InventoryItems = new List<ItemInstanceData>
                {
                    new() { InstanceId = "v5-sword", ItemId = sword.ItemId, SlotIndex = 5 },
                    new() { InstanceId = "v5-axe", ItemId = axe.ItemId, SlotIndex = 2 }
                },
                MainHandInstanceId = "v5-axe"
            };
            List<ItemInstance> restored = PlayerPersistenceController.BuildInventorySlots(
                versionFive,
                itemId => ResolveItem(itemId, sword, axe),
                8);
            Require(restored[0] == null && restored[1] == null &&
                    restored[2]?.InstanceId == "v5-axe" &&
                    restored[5]?.InstanceId == "v5-sword",
                "Version-5 restoration must preserve empty gaps and exact positions.");
            Require(PlayerPersistenceController.ResolveMainHandInstanceId(
                    versionFive,
                    restored) == "v5-axe",
                "Version-5 equipment must resolve after reordered slot restoration.");
        }

        private static void ValidatePrefabs(
            ItemDefinition sword,
            ItemDefinition axe,
            LootTableDefinition boarLoot)
        {
            GameObject player = LoadRequiredAsset<GameObject>(PlayerPrefabPath);
            PlayerInventory inventory = player.GetComponent<PlayerInventory>();
            PlayerPersistenceController persistence =
                player.GetComponent<PlayerPersistenceController>();
            Require(inventory != null && inventory.Capacity == 8,
                "Player prefab must retain eight fixed inventory positions.");
            Require(persistence != null && persistence.ItemCatalog.Count >= 2 &&
                    persistence.ItemCatalog.Contains(sword) &&
                    persistence.ItemCatalog.Contains(axe),
                "Persistence catalog must contain both weapon definitions.");

            GameObject boar = LoadRequiredAsset<GameObject>(BoarPrefabPath);
            EnemyLootDrop boarDrop = boar.GetComponent<EnemyLootDrop>();
            Require(boarDrop != null && boarDrop.LootTable == boarLoot,
                "Boar prefab must use the authored Worn Axe loot table.");
        }

        private static void ValidateScene()
        {
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            InventoryView view = UnityEngine.Object.FindFirstObjectByType<InventoryView>();
            Require(view != null && view.WindowRoot != null && view.SlotCount == 8,
                "Starter scene must preserve the eight-slot inventory view.");

            InventorySlotDragHandler[] dragHandlers =
                UnityEngine.Object.FindObjectsByType<InventorySlotDragHandler>(
                    FindObjectsInactive.Include,
                    FindObjectsSortMode.None);
            Require(dragHandlers.Length == 8,
                "Every inventory position must have one drag handler.");
            Require(dragHandlers.All(handler => handler.InventoryView == view) &&
                    dragHandlers.Select(handler => handler.SlotIndex).Distinct().Count() == 8 &&
                    dragHandlers.All(handler => handler.SlotIndex >= 0 && handler.SlotIndex < 8),
                "Inventory drag handlers must reference one view and unique slot indices.");
            Require(UnityEngine.Object.FindObjectsByType<EnemySpawner>(
                    FindObjectsSortMode.None).Length == 3,
                "Starter scene must preserve all three enemy spawners.");
        }

        private static ItemDefinition ResolveItem(
            string itemId,
            ItemDefinition sword,
            ItemDefinition axe)
        {
            if (itemId == sword.ItemId)
            {
                return sword;
            }

            return itemId == axe.ItemId ? axe : null;
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
                    $"Sprint 020 validation failed: {message}");
            }
        }
    }
}
