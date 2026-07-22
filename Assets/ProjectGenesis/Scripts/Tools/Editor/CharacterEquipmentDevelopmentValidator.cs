using System;
using System.Collections.Generic;
using ProjectGenesis.Data;
using ProjectGenesis.Gameplay;
using ProjectGenesis.Saving;
using ProjectGenesis.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ProjectGenesis.Tools.Editor
{
    public static class CharacterEquipmentDevelopmentValidator
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

        [MenuItem("Project Genesis/Sprint 023/Validate Character Equipment")]
        public static void ValidateCharacterEquipment()
        {
            ItemDefinition sword = LoadRequiredAsset<ItemDefinition>(SwordPath);
            ItemDefinition axe = LoadRequiredAsset<ItemDefinition>(AxePath);
            ItemDefinition armor = LoadRequiredAsset<ItemDefinition>(ArmorPath);
            ItemDefinition potion = LoadRequiredAsset<ItemDefinition>(PotionPath);

            ValidateEquipmentTransfer(sword, axe, armor, potion);
            ValidateItemRemoval(sword);
            ValidateProfileMigration(sword, armor);
            ValidatePlayerPrefab();
            ValidateScene();
            Debug.Log("Sprint 023 character equipment validation passed.");
        }

        [MenuItem("Project Genesis/Sprint 023/Validate Relevant Regression Suite")]
        public static void ValidateRelevantRegressionSuite()
        {
            ValidateCharacterEquipment();
            DeathStateDevelopmentValidator.ValidateDeathState();
            EquipmentAndConsumableDevelopmentValidator.ValidateEquipmentAndConsumable();
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
            Debug.Log("Sprint 023 relevant regression suite passed.");
        }

        [MenuItem("Project Genesis/Sprint 023/Prepare Play Mode Test")]
        public static void PreparePlayModeTest()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("Enter Play Mode before preparing the Sprint 023 test.");
                return;
            }

            PlayerInventory inventory =
                UnityEngine.Object.FindFirstObjectByType<PlayerInventory>();
            if (inventory == null)
            {
                Debug.LogWarning("Could not find the player inventory.");
                return;
            }

            int added = 0;
            added += inventory.TryAdd(LoadRequiredAsset<ItemDefinition>(SwordPath)) ? 1 : 0;
            added += inventory.TryAdd(LoadRequiredAsset<ItemDefinition>(AxePath)) ? 1 : 0;
            added += inventory.TryAdd(LoadRequiredAsset<ItemDefinition>(ArmorPath)) ? 1 : 0;
            Debug.Log($"Sprint 023 test prepared: added {added} equipment items.");
        }

        private static void ValidateEquipmentTransfer(
            ItemDefinition sword,
            ItemDefinition axe,
            ItemDefinition armor,
            ItemDefinition potion)
        {
            GameObject probe = new("CharacterEquipmentValidation");
            try
            {
                PlayerInventory inventory = probe.AddComponent<PlayerInventory>();
                CombatStats stats = probe.AddComponent<CombatStats>();
                stats.Configure(12, 3, 1.35f, 0.8f);
                PlayerEquipment equipment = probe.AddComponent<PlayerEquipment>();

                ItemInstance swordInstance = ItemInstance.Create(sword, "sprint23-sword");
                ItemInstance axeInstance = ItemInstance.Create(axe, "sprint23-axe");
                ItemInstance armorInstance = ItemInstance.Create(armor, "sprint23-armor");
                Require(inventory.TryAddInstance(swordInstance) &&
                        inventory.TryAddInstance(axeInstance) &&
                        inventory.TryAddInstance(armorInstance),
                    "Validation equipment must enter the bag.");
                Require(!equipment.EquipMainHand(armorInstance) &&
                        !equipment.EquipBody(swordInstance),
                    "Equipment slots must reject incompatible categories.");

                Require(equipment.EquipMainHand(swordInstance) &&
                        equipment.MainHand == swordInstance &&
                        !inventory.Contains(swordInstance) && inventory.Count == 2 &&
                        stats.EquipmentAttackBonus == sword.AttackBonus,
                    "Equipping must transfer the exact weapon out of the bag.");
                int axeBagSlot = inventory.GetSlotIndex(axeInstance.InstanceId);
                Require(equipment.EquipMainHand(axeInstance) &&
                        equipment.MainHand == axeInstance &&
                        inventory.GetItemAt(axeBagSlot) == swordInstance &&
                        stats.EquipmentAttackBonus == axe.AttackBonus,
                    "Replacing a weapon must return the previous exact instance to the released slot.");
                Require(equipment.EquipBody(armorInstance) &&
                        equipment.Body == armorInstance &&
                        !inventory.Contains(armorInstance) &&
                        stats.EquipmentDefenseBonus == armor.DefenseBonus,
                    "Equipping must transfer exact armor and apply its defense.");

                while (inventory.HasFreeSlot)
                {
                    Require(inventory.TryAdd(potion),
                        "The validation bag must be fillable with potions.");
                }

                Require(!equipment.UnequipBody() &&
                        equipment.Body == armorInstance &&
                        !inventory.Contains(armorInstance),
                    "A full bag must refuse unequip without losing armor.");
                Require(inventory.TryRemoveInstance(inventory.GetItemAt(0)) &&
                        equipment.UnequipBody() && equipment.Body == null &&
                        inventory.Contains(armorInstance) &&
                        stats.EquipmentDefenseBonus == 0,
                    "Unequipping with space must return the exact armor to the bag.");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(probe);
            }
        }

        private static void ValidateProfileMigration(
            ItemDefinition sword,
            ItemDefinition armor)
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

            PlayerProfileData versionSix = new()
            {
                Version = 6,
                InventoryItems = new List<ItemInstanceData>
                {
                    new() { InstanceId = "legacy-sword", ItemId = sword.ItemId, SlotIndex = 2 },
                    new() { InstanceId = "legacy-armor", ItemId = armor.ItemId, SlotIndex = 5 }
                },
                MainHandInstanceId = "legacy-sword",
                BodyInstanceId = "legacy-armor"
            };
            List<ItemInstance> migratedSlots = PlayerPersistenceController.BuildInventorySlots(
                versionSix,
                itemId => ResolveItem(itemId, sword, armor),
                8);

            GameObject probe = new("CharacterEquipmentMigrationValidation");
            try
            {
                PlayerInventory inventory = probe.AddComponent<PlayerInventory>();
                CombatStats stats = probe.AddComponent<CombatStats>();
                stats.Configure(12, 3, 1.35f, 0.8f);
                PlayerEquipment equipment = probe.AddComponent<PlayerEquipment>();
                inventory.RestoreSlots(migratedSlots);
                equipment.RestoreMainHand(
                    PlayerPersistenceController.ResolveMainHandInstanceId(
                        versionSix,
                        migratedSlots));
                equipment.RestoreBody(
                    PlayerPersistenceController.ResolveBodyInstanceId(
                        versionSix,
                        migratedSlots));
                Require(equipment.MainHand?.InstanceId == "legacy-sword" &&
                        equipment.Body?.InstanceId == "legacy-armor" &&
                        inventory.Count == 0,
                    "Version-6 equipped instances must migrate out of old bag positions.");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(probe);
            }

            ItemInstance restoredWeapon = PlayerPersistenceController.BuildEquippedInstance(
                new ItemInstanceData
                {
                    InstanceId = "v7-sword",
                    ItemId = sword.ItemId
                },
                itemId => ResolveItem(itemId, sword, armor),
                ItemType.Weapon);
            Require(restoredWeapon?.InstanceId == "v7-sword" &&
                    restoredWeapon.Definition == sword,
                "Version-7 equipment must restore the exact saved instance.");
            Require(PlayerPersistenceController.BuildEquippedInstance(
                    new ItemInstanceData
                    {
                        InstanceId = "wrong-slot",
                        ItemId = armor.ItemId
                    },
                    itemId => ResolveItem(itemId, sword, armor),
                    ItemType.Weapon) == null,
                "Version-7 equipment restore must reject the wrong category.");
        }

        private static void ValidateItemRemoval(ItemDefinition sword)
        {
            GameObject probe = new("ItemRemovalValidation");
            try
            {
                Health health = probe.AddComponent<Health>();
                health.Configure(100);
                PlayerInventory inventory = probe.AddComponent<PlayerInventory>();
                PlayerItemDropController dropController =
                    probe.AddComponent<PlayerItemDropController>();
                ItemInstance swordInstance = ItemInstance.Create(
                    sword,
                    "drop-and-destroy-sword");
                Require(inventory.TryAddInstance(swordInstance),
                    "The removal test item must enter the bag.");

                Require(dropController.TryDrop(swordInstance, out WorldLootPickup pickup) &&
                        pickup != null && pickup.Instance == swordInstance &&
                        !inventory.Contains(swordInstance),
                    "Dropping must move the exact instance from the bag into the world.");
                Require(pickup.TryCollect(inventory) &&
                        inventory.FindByInstanceId(swordInstance.InstanceId) == swordInstance,
                    "Collecting a dropped item must restore the same exact instance.");
                Require(dropController.TryDestroy(swordInstance) && inventory.Count == 0,
                    "The confirmed trash action must permanently remove the selected bag item.");

                Require(inventory.TryAddInstance(swordInstance) &&
                        health.TakeDamage(100) && health.IsDead &&
                        !dropController.TryDestroy(swordInstance) &&
                        !dropController.TryDrop(swordInstance, out _),
                    "Dropping and destroying items must be unavailable while dead.");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(probe);
            }
        }

        private static void ValidatePlayerPrefab()
        {
            GameObject player = LoadRequiredAsset<GameObject>(PlayerPrefabPath);
            PlayerItemDropController dropController =
                player.GetComponent<PlayerItemDropController>();
            Require(dropController != null && dropController.PickupMaterial != null,
                "Player prefab must contain a configured item-drop controller.");
        }

        private static void ValidateScene()
        {
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            CharacterEquipmentView view =
                UnityEngine.Object.FindFirstObjectByType<CharacterEquipmentView>();
            InventoryView inventoryView =
                UnityEngine.Object.FindFirstObjectByType<InventoryView>();
            Require(view != null && inventoryView != null && view.WindowRoot != null &&
                    view.MainHandSlotButton != null && view.BodySlotButton != null &&
                    view.MainHandUnequipButton != null &&
                    view.BodyUnequipButton != null &&
                    view.Inventory != null && view.Equipment != null,
                "Starter scene must contain a wired character equipment view.");
            Require(view.WindowRoot == inventoryView.WindowRoot &&
                    view.WindowRoot.name == "InventoryWindow" &&
                    FindChild(view.WindowRoot.transform, "Text_EquipmentComparison") != null &&
                    FindChild(view.WindowRoot.transform, "Button_EquipmentAction") != null &&
                    FindChild(view.WindowRoot.transform, "Button_DropItem") != null &&
                    FindChild(view.WindowRoot.transform, "Button_DestroyItem") != null &&
                    FindChild(inventoryView.transform, "InventoryDestroyConfirmation") != null &&
                    inventoryView.ItemDropController != null,
                "Inventory window must contain equipment, drop, and trash actions.");
            InventoryTrashDropTarget[] trashTargets =
                UnityEngine.Object.FindObjectsByType<InventoryTrashDropTarget>(
                    FindObjectsInactive.Include,
                    FindObjectsSortMode.None);
            Require(trashTargets.Length == 1 &&
                    trashTargets[0].InventoryView == inventoryView,
                "Trash drop target must reference the inventory view.");
        }

        private static ItemDefinition ResolveItem(
            string itemId,
            ItemDefinition sword,
            ItemDefinition armor)
        {
            if (itemId == sword.ItemId)
            {
                return sword;
            }

            return itemId == armor.ItemId ? armor : null;
        }

        private static Transform FindChild(Transform root, string childName)
        {
            foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
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
                    $"Sprint 023 validation failed: {message}");
            }
        }
    }
}
