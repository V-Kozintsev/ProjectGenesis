using System;
using ProjectGenesis.Gameplay;
using ProjectGenesis.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace ProjectGenesis.Tools.Editor
{
    public static class DeathStateDevelopmentValidator
    {
        private const string ScenePath =
            "Assets/ProjectGenesis/Scenes/StarterVillage.unity";
        private const string PlayerPrefabPath =
            "Assets/ProjectGenesis/Prefabs/Characters/PF_Player_Prototype.prefab";

        [MenuItem("Project Genesis/Sprint 022/Validate Death State")]
        public static void ValidateDeathState()
        {
            ValidateDeathLifecycle();
            ValidatePrefab();
            ValidateScene();
            Debug.Log("Sprint 022 death state validation passed.");
        }

        [MenuItem("Project Genesis/Sprint 022/Validate Relevant Regression Suite")]
        public static void ValidateRelevantRegressionSuite()
        {
            ValidateDeathState();
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
            Debug.Log("Sprint 022 relevant regression suite passed.");
        }

        private static void ValidateDeathLifecycle()
        {
            GameObject player = new("DeathStateValidationPlayer");
            GameObject spawn = new("DeathStateValidationSpawn");
            GameObject window = new("DeathStateValidationWindow");
            GameObject lossObject = new("DeathStateValidationLossText");
            GameObject buttonObject = new("DeathStateValidationButton");

            try
            {
                spawn.transform.position = new Vector3(10f, 0f, 3f);
                player.transform.position = new Vector3(3f, 0f, -2f);

                player.AddComponent<NavMeshAgent>().enabled = false;
                Health health = player.AddComponent<Health>();
                health.Configure(100);
                player.AddComponent<CombatStats>();
                HealthRegeneration regeneration = player.AddComponent<HealthRegeneration>();
                PlayerProgression progression = player.AddComponent<PlayerProgression>();
                progression.ConfigureDeathPenalty(0.1f, 10);
                progression.RestoreState(2, 5);
                PlayerCombatController combat = player.AddComponent<PlayerCombatController>();
                PlayerDeathController death = player.AddComponent<PlayerDeathController>();

                Text lossText = lossObject.AddComponent<Text>();
                Button button = buttonObject.AddComponent<Button>();
                DeathRespawnView view = player.AddComponent<DeathRespawnView>();
                view.Initialize(window, lossText, button);
                death.SetDeathView(view);
                death.SetRespawnPoint(spawn.transform);

                Vector3 deathPosition = player.transform.position;
                Require(health.TakeDamage(100), "Fatal damage must be accepted.");
                Require(health.IsDead && death.IsDeathStateActive && combat.IsInputLocked,
                    "Fatal damage must enter a locked death state.");
                Require(death.LastExperienceLoss == 15 &&
                        progression.Level == 1 && progression.CurrentExperience == 90,
                    "Death penalty must be applied exactly once on death.");
                Require(player.transform.position == deathPosition,
                    "Player must remain at the death position before confirmation.");
                Require(view.IsVisible && lossText.text.Contains("15"),
                    "Death view must show the current experience loss.");
                Require(!regeneration.IsRegenerationAllowed,
                    "Regeneration must be disabled while dead.");

                death.RespawnAtVillage();
                Require(!death.IsDeathStateActive && !health.IsDead &&
                        health.CurrentHealth == health.MaximumHealth,
                    "Village resurrection must restore full health and clear death state.");
                Require(player.transform.position == spawn.transform.position,
                    "Village resurrection must move the player to the configured spawn point.");
                Require(!view.IsVisible && regeneration.IsRegenerationAllowed,
                    "Death view must hide and regeneration must resume after resurrection.");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(player);
                UnityEngine.Object.DestroyImmediate(spawn);
                UnityEngine.Object.DestroyImmediate(window);
                UnityEngine.Object.DestroyImmediate(lossObject);
                UnityEngine.Object.DestroyImmediate(buttonObject);
            }
        }

        private static void ValidatePrefab()
        {
            GameObject player = AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath);
            Require(player != null, "Player prefab was not found.");
            Require(player.GetComponent<PlayerDeathController>() != null,
                "Player prefab must contain PlayerDeathController.");
            Require(player.GetComponent<PlayerCombatController>() != null,
                "Player prefab must keep PlayerCombatController.");
        }

        private static void ValidateScene()
        {
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            PlayerDeathController death =
                UnityEngine.Object.FindFirstObjectByType<PlayerDeathController>();
            DeathRespawnView view =
                UnityEngine.Object.FindFirstObjectByType<DeathRespawnView>();
            Require(death != null && death.RespawnPoint != null,
                "Scene player must have a death controller with a respawn point.");
            Require(view != null, "Scene must contain a death respawn view.");
        }

        private static void Require(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException(
                    $"Sprint 022 validation failed: {message}");
            }
        }
    }
}
