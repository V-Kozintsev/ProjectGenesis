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
    public static class ItemInstancesDevelopmentValidator
    {
        private const string ScenePath = "Assets/ProjectGenesis/Scenes/StarterVillage.unity";
        private const string PlayerPrefabPath =
            "Assets/ProjectGenesis/Prefabs/Characters/PF_Player_Prototype.prefab";
        private const string SwordPath =
            "Assets/ProjectGenesis/Data/Items/ITM_RustySword.asset";

        [MenuItem("Project Genesis/Sprint 019/Validate Item Instances")]
        public static void ValidateItemInstances()
        {
            ItemDefinition sword = LoadRequiredAsset<ItemDefinition>(SwordPath);
            ValidateRuntimeInstances(sword);
            ValidateProfileMigration(sword);
            ValidatePlayerPrefab(sword);
            ValidateScene();
            Debug.Log("Sprint 019 item instances validation passed.");
        }

        [MenuItem("Project Genesis/Sprint 019/Add Two Rusty Swords In Play Mode")]
        public static void AddTwoRustySwordsInPlayMode()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("Enter Play Mode before adding test item instances.");
                return;
            }

            PlayerInventory inventory =
                UnityEngine.Object.FindFirstObjectByType<PlayerInventory>();
            ItemDefinition sword = AssetDatabase.LoadAssetAtPath<ItemDefinition>(SwordPath);
            if (inventory == null || sword == null)
            {
                Debug.LogWarning("Could not find the player inventory or Rusty Sword data.");
                return;
            }

            int addedCount = 0;
            for (int index = 0; index < 2; index++)
            {
                if (inventory.TryAdd(sword))
                {
                    addedCount++;
                }
            }

            Debug.Log($"Added {addedCount} Rusty Sword item instances for Play Mode testing.");
        }

        private static void ValidateRuntimeInstances(ItemDefinition sword)
        {
            ItemInstance generatedFirst = ItemInstance.Create(sword);
            ItemInstance generatedSecond = ItemInstance.Create(sword);
            Require(generatedFirst.IsValid && generatedSecond.IsValid,
                "Generated item instances must be valid.");
            Require(generatedFirst.InstanceId != generatedSecond.InstanceId,
                "Two drops of one definition must receive unique instance ids.");

            GameObject probe = new("ItemInstancesValidation");
            try
            {
                PlayerInventory inventory = probe.AddComponent<PlayerInventory>();
                CombatStats stats = probe.AddComponent<CombatStats>();
                stats.Configure(12, 3, 1.35f, 0.8f);
                PlayerEquipment equipment = probe.AddComponent<PlayerEquipment>();

                ItemInstance first = ItemInstance.Create(sword, "sword-copy-a");
                ItemInstance second = ItemInstance.Create(sword, "sword-copy-b");
                ItemInstance duplicate = ItemInstance.Create(sword, "sword-copy-b");

                Require(inventory.TryAddInstance(first),
                    "The first sword instance must enter the inventory.");
                Require(inventory.TryAddInstance(second),
                    "A second sword with another id must enter the inventory.");
                Require(!inventory.TryAddInstance(duplicate),
                    "A duplicate instance id must be rejected.");
                Require(inventory.Count == 2,
                    "Duplicate rejection must preserve two inventory entries.");
                Require(equipment.EquipMainHand(second),
                    "The selected second sword must be equippable.");
                Require(equipment.MainHand == second,
                    "Equipment must retain the exact selected instance.");
                Require(!inventory.Contains(second) && inventory.Count == 1,
                    "The equipped instance must leave bag storage.");
                Require(stats.EquipmentAttackBonus == sword.AttackBonus,
                    "The exact equipped instance must apply its definition bonus.");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(probe);
            }
        }

        private static void ValidateProfileMigration(ItemDefinition sword)
        {
            Require(PlayerProfileData.CurrentVersion == 7,
                "Current local profile version must be 7.");
            for (int version = 1; version <= 7; version++)
            {
                Require(LocalJsonPlayerPersistence.IsSupportedVersion(version),
                    $"Profile version {version} must remain supported.");
            }

            Require(!LocalJsonPlayerPersistence.IsSupportedVersion(8),
                "Unknown future profile versions must be rejected.");

            PlayerProfileData legacy = new()
            {
                Version = 3,
                InventoryItemIds = new List<string> { sword.ItemId, sword.ItemId },
                MainHandItemId = sword.ItemId
            };
            List<ItemInstance> migrated = PlayerPersistenceController.BuildInventoryInstances(
                legacy,
                itemId => itemId == sword.ItemId ? sword : null);
            Require(migrated.Count == 2,
                "Both legacy copies must migrate to item instances.");
            Require(migrated.All(item => item.IsValid) &&
                    migrated.Select(item => item.InstanceId).Distinct().Count() == 2,
                "Legacy copies must receive distinct valid ids.");
            Require(PlayerPersistenceController.ResolveMainHandInstanceId(legacy, migrated) ==
                    migrated[0].InstanceId,
                "Legacy equipment must resolve to the first matching migrated copy.");

            PlayerProfileData current = new()
            {
                Version = 4,
                InventoryItems = new List<ItemInstanceData>
                {
                    new() { InstanceId = "saved-sword-a", ItemId = sword.ItemId },
                    new() { InstanceId = "saved-sword-b", ItemId = sword.ItemId }
                },
                MainHandInstanceId = "saved-sword-b"
            };
            List<ItemInstance> restored = PlayerPersistenceController.BuildInventoryInstances(
                current,
                itemId => itemId == sword.ItemId ? sword : null);
            Require(restored.Count == 2 &&
                    restored[0].InstanceId == "saved-sword-a" &&
                    restored[1].InstanceId == "saved-sword-b",
                "Version-4 instance ids must survive restoration exactly.");
            Require(PlayerPersistenceController.ResolveMainHandInstanceId(current, restored) ==
                    "saved-sword-b",
                "Version-4 equipment must restore the exact saved instance.");
        }

        private static void ValidatePlayerPrefab(ItemDefinition sword)
        {
            GameObject player = LoadRequiredAsset<GameObject>(PlayerPrefabPath);
            PlayerInventory inventory = player.GetComponent<PlayerInventory>();
            PlayerEquipment equipment = player.GetComponent<PlayerEquipment>();
            PlayerPersistenceController persistence =
                player.GetComponent<PlayerPersistenceController>();

            Require(inventory != null && inventory.Capacity == 8,
                "Player inventory must retain eight slots.");
            Require(equipment != null,
                "Player prefab must retain equipment.");
            Require(equipment.MainHand == null,
                "Player prefab must begin without a valid equipped item instance.");
            Require(persistence != null && persistence.ItemCatalog.Contains(sword),
                "Persistence item catalog must retain the Rusty Sword definition.");
        }

        private static void ValidateScene()
        {
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

            InventoryView[] inventoryViews =
                UnityEngine.Object.FindObjectsByType<InventoryView>(FindObjectsSortMode.None);
            Require(inventoryViews.Length == 1,
                "Starter scene must contain exactly one inventory view.");
            InventoryView view = inventoryViews.Single();
            Require(view.WindowRoot != null && view.SlotCount == 8,
                "Inventory view must reference one window and eight slots.");

            for (int index = 1; index <= 8; index++)
            {
                Require(FindChild(view.WindowRoot.transform, $"Button_InventorySlot_{index}") != null,
                    $"Inventory slot button {index} is missing.");
            }

            Require(FindChild(view.WindowRoot.transform, "Button_ItemAction") != null,
                "Selected-item action button is missing.");
            Require(UnityEngine.Object.FindObjectsByType<WorldLootPickup>(
                    FindObjectsInactive.Include,
                    FindObjectsSortMode.None).Length == 0,
                "The authored scene must not contain stale runtime loot pickups.");
            Require(UnityEngine.Object.FindObjectsByType<EnemySpawner>(
                    FindObjectsSortMode.None).Length == 3,
                "Starter scene must preserve all three enemy spawners.");
        }

        private static Transform FindChild(Transform parent, string childName)
        {
            foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
            {
                if (child.name == childName)
                {
                    return child;
                }
            }

            return null;
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
                    $"Sprint 019 validation failed: {message}");
            }
        }
    }
}
