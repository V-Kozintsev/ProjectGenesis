using System;
using ProjectGenesis.Gameplay;
using UnityEditor;
using UnityEngine;

namespace ProjectGenesis.Tools.Editor
{
    public static class EnemyExperienceDevelopmentValidator
    {
        private const string PlayerPrefabPath =
            "Assets/ProjectGenesis/Prefabs/Characters/PF_Player_Prototype.prefab";
        private const string WolfPrefabPath =
            "Assets/ProjectGenesis/Prefabs/Enemies/PF_Enemy_Wolf.prefab";

        [MenuItem("Project Genesis/Sprint 013/Validate Enemy Experience")]
        public static void ValidateEnemyExperience()
        {
            ValidatePrefabDefaults();
            ValidateRewardScaling();
            Debug.Log("Sprint 013 enemy experience validation passed.");
        }

        private static void ValidatePrefabDefaults()
        {
            GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath);
            Require(playerPrefab != null, "Player prefab was not found.");
            PlayerProgression progression = playerPrefab.GetComponent<PlayerProgression>();
            Require(progression != null, "Player prefab is missing PlayerProgression.");
            Require(Mathf.Approximately(progression.LowerLevelPenaltyPerLevel, 0.25f),
                "Lower-level experience penalty must be 25% per level.");
            Require(Mathf.Approximately(progression.MinimumEnemyRewardMultiplier, 0.1f),
                "Minimum enemy reward multiplier must be 10%.");
            Require(Mathf.Approximately(progression.HigherLevelBonusPerLevel, 0.1f),
                "Higher-level experience bonus must be 10% per level.");
            Require(Mathf.Approximately(progression.MaximumEnemyRewardMultiplier, 1.5f),
                "Maximum enemy reward multiplier must be 150%.");

            GameObject wolfPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(WolfPrefabPath);
            Require(wolfPrefab != null, "Wolf prefab was not found.");
            EnemyBrain wolf = wolfPrefab.GetComponent<EnemyBrain>();
            Require(wolf != null, "Wolf prefab is missing EnemyBrain.");
            Require(wolf.DisplayName == "Молодой волк", "Wolf display name is unexpected.");
            Require(wolf.EnemyLevel == 1, "Young wolf must be level 1.");
            Require(wolf.ExperienceReward == 20, "Young wolf base experience must be 20.");
        }

        private static void ValidateRewardScaling()
        {
            GameObject probe = new("EnemyExperienceValidation");

            try
            {
                probe.AddComponent<Health>();
                probe.AddComponent<CombatStats>();
                PlayerProgression progression = probe.AddComponent<PlayerProgression>();
                progression.ConfigureEnemyExperienceScaling(0.25f, 0.1f, 0.1f, 1.5f);

                progression.RestoreState(1, 0);
                Require(progression.CalculateEnemyExperience(1, 20) == 20,
                    "An equal-level wolf must award 100% experience.");

                progression.RestoreState(2, 0);
                Require(progression.CalculateEnemyExperience(1, 20) == 15,
                    "A wolf one level below must award 75% experience.");

                progression.RestoreState(3, 0);
                Require(progression.CalculateEnemyExperience(1, 20) == 10,
                    "A wolf two levels below must award 50% experience.");

                progression.RestoreState(4, 0);
                Require(progression.CalculateEnemyExperience(1, 20) == 5,
                    "A wolf three levels below must award 25% experience.");

                progression.RestoreState(5, 0);
                Require(progression.CalculateEnemyExperience(1, 20) == 2,
                    "A much weaker wolf must keep the 10% minimum reward.");

                progression.RestoreState(1, 0);
                Require(progression.CalculateEnemyExperience(2, 20) == 22,
                    "An enemy one level above must award 110% experience.");
                Require(progression.CalculateEnemyExperience(8, 20) == 30,
                    "Higher-level rewards must stop at the 150% cap.");
                Require(progression.CalculateEnemyExperience(1, 0) == 0,
                    "A zero base reward must remain zero.");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(probe);
            }
        }

        private static void Require(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException($"Sprint 013 validation failed: {message}");
            }
        }
    }
}
