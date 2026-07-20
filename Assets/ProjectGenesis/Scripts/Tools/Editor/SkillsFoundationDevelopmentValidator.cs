using System;
using System.Linq;
using ProjectGenesis.Data;
using ProjectGenesis.Gameplay;
using ProjectGenesis.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ProjectGenesis.Tools.Editor
{
    public static class SkillsFoundationDevelopmentValidator
    {
        private const string ScenePath = "Assets/ProjectGenesis/Scenes/StarterVillage.unity";
        private const string PlayerPrefabPath =
            "Assets/ProjectGenesis/Prefabs/Characters/PF_Player_Prototype.prefab";
        private const string HeavyStrikePath =
            "Assets/ProjectGenesis/Data/Skills/SO_Skill_HeavyStrike.asset";

        [MenuItem("Project Genesis/Sprint 015/Validate Skills Foundation")]
        public static void ValidateSkillsFoundation()
        {
            SkillDefinition heavyStrike = LoadRequiredAsset<SkillDefinition>(HeavyStrikePath);
            ValidateHeavyStrike(heavyStrike);
            ValidatePlayerPrefab(heavyStrike);
            ValidateScene();
            Debug.Log("Sprint 015 skills foundation validation passed.");
        }

        private static void ValidateHeavyStrike(SkillDefinition skill)
        {
            Require(skill.IsValid, "Heavy Strike skill data is invalid.");
            Require(skill.SkillId == "warrior.heavy_strike", "Heavy Strike id is unexpected.");
            Require(skill.DisplayName == "Heavy Strike", "Heavy Strike display name is unexpected.");
            Require(skill.ClassRequirement == SkillClassRequirement.Warrior,
                "Heavy Strike must be authored as a warrior skill.");
            Require(skill.TargetType == SkillTargetType.Enemy, "Heavy Strike must target enemies.");
            Require(skill.Damage == 22, "Heavy Strike damage must be 22.");
            Require(Mathf.Approximately(skill.Range, 1.55f), "Heavy Strike range must be 1.55.");
            Require(Mathf.Approximately(skill.Cooldown, 4f), "Heavy Strike cooldown must be 4 seconds.");
        }

        private static void ValidatePlayerPrefab(SkillDefinition heavyStrike)
        {
            GameObject playerPrefab = LoadRequiredAsset<GameObject>(PlayerPrefabPath);
            Require(playerPrefab.GetComponent<PlayerCombatController>() != null,
                "Player prefab must keep PlayerCombatController for basic autoattack.");
            Require(playerPrefab.GetComponent<PlayerController>() != null,
                "Player prefab must keep PlayerController for click and WASD movement.");

            PlayerSkillController skillController = playerPrefab.GetComponent<PlayerSkillController>();
            Require(skillController != null, "Player prefab is missing PlayerSkillController.");
            Require(skillController.SlotCount == 1, "Player must start with exactly one skill hotbar slot.");
            Require(skillController.GetSkill(0) == heavyStrike,
                "Player skill slot 1 must reference Heavy Strike.");
        }

        private static void ValidateScene()
        {
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

            SkillHotbarView[] hotbars =
                UnityEngine.Object.FindObjectsByType<SkillHotbarView>(FindObjectsSortMode.None);
            Require(hotbars.Length == 1, "Starter scene must contain exactly one skill hotbar.");

            EnemySpawner[] spawners =
                UnityEngine.Object.FindObjectsByType<EnemySpawner>(FindObjectsSortMode.None);
            Require(spawners.Length == 3, "Starter combat area must keep exactly three spawners.");
            Require(spawners.Count(spawner => spawner.EnemyPrefab != null) == 3,
                "All enemy spawners must keep valid enemy prefabs.");
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
                throw new InvalidOperationException($"Sprint 015 validation failed: {message}");
            }
        }
    }
}
