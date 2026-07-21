using System;
using System.Linq;
using ProjectGenesis.Gameplay;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ProjectGenesis.Tools.Editor
{
    public static class EnemyVarietyDevelopmentValidator
    {
        private const string ScenePath = "Assets/ProjectGenesis/Scenes/StarterVillage.unity";
        private const string WolfPrefabPath =
            "Assets/ProjectGenesis/Prefabs/Enemies/PF_Enemy_Wolf.prefab";
        private const string BoarPrefabPath =
            "Assets/ProjectGenesis/Prefabs/Enemies/PF_Enemy_Boar.prefab";

        [MenuItem("Project Genesis/Sprint 014/Validate Enemy Variety")]
        public static void ValidateEnemyVariety()
        {
            GameObject wolfPrefab = LoadRequiredPrefab(WolfPrefabPath);
            GameObject boarPrefab = LoadRequiredPrefab(BoarPrefabPath);
            ValidateBoarPrefab(boarPrefab);
            ValidateSceneSpawners(wolfPrefab, boarPrefab);
            Debug.Log("Sprint 014 enemy variety validation passed.");
        }

        private static void ValidateBoarPrefab(GameObject boarPrefab)
        {
            EnemyBrain brain = boarPrefab.GetComponent<EnemyBrain>();
            Require(brain != null, "Boar prefab is missing EnemyBrain.");
            Require(brain.DisplayName == "Лесной кабан", "Boar display name is unexpected.");
            Require(brain.EnemyLevel == 2, "Forest boar must be level 2.");
            Require(brain.ExperienceReward == 30, "Forest boar base experience must be 30.");
            Require(brain.QuestTargetId == "boar", "Forest boar quest target id is unexpected.");

            Health health = boarPrefab.GetComponent<Health>();
            Require(health != null && health.MaximumHealth == 60,
                "Forest boar must have 60 maximum health.");

            CombatStats stats = boarPrefab.GetComponent<CombatStats>();
            Require(stats != null, "Boar prefab is missing CombatStats.");
            Require(stats.BaseAttackPower == 10 && stats.Defense == 2,
                "Forest boar combat stats are unexpected.");
            EnemyLootDrop lootDrop = boarPrefab.GetComponent<EnemyLootDrop>();
            Require(lootDrop != null && lootDrop.LootTable != null,
                "Forest boar must have its own regular loot table.");
            Require(string.IsNullOrEmpty(lootDrop.QuestObjectiveTargetId) &&
                    Mathf.Approximately(lootDrop.QuestItemDropChance, 0f),
                "Forest boar must not use the wolf quest-trophy rules.");
        }

        private static void ValidateSceneSpawners(GameObject wolfPrefab, GameObject boarPrefab)
        {
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            EnemySpawner[] spawners =
                UnityEngine.Object.FindObjectsByType<EnemySpawner>(FindObjectsSortMode.None);

            Require(spawners.Length == 3, "Starter combat area must keep exactly three spawners.");
            Require(spawners.Count(spawner => spawner.EnemyPrefab == wolfPrefab) == 2,
                "Starter combat area must contain exactly two wolf spawners.");
            Require(spawners.Count(spawner => spawner.EnemyPrefab == boarPrefab) == 1,
                "Starter combat area must contain exactly one boar spawner.");

            EnemySpawner boarSpawner = spawners.Single(spawner => spawner.EnemyPrefab == boarPrefab);
            Require(boarSpawner.name == "BoarSpawn_North",
                "Forest boar must occupy the northern spawn point.");
            Require(boarSpawner.Territory != null &&
                    boarSpawner.Territory.Contains(boarSpawner.transform.position),
                "Forest boar spawner must remain inside the combat territory.");
        }

        private static GameObject LoadRequiredPrefab(string path)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            Require(prefab != null, $"Required prefab was not found at '{path}'.");
            return prefab;
        }

        private static void Require(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException($"Sprint 014 validation failed: {message}");
            }
        }
    }
}
