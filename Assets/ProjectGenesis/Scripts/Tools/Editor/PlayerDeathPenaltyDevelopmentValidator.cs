using System;
using ProjectGenesis.Gameplay;
using UnityEditor;
using UnityEngine;

namespace ProjectGenesis.Tools.Editor
{
    public static class PlayerDeathPenaltyDevelopmentValidator
    {
        private const string PlayerPrefabPath =
            "Assets/ProjectGenesis/Prefabs/Characters/PF_Player_Prototype.prefab";

        [MenuItem("Project Genesis/Sprint 012/Validate Player Death Penalty")]
        public static void ValidatePlayerDeathPenalty()
        {
            ValidatePrefabDefaults();
            ValidateProgressionBoundaries();
            Debug.Log("Sprint 012 player death penalty validation passed.");
        }

        private static void ValidatePrefabDefaults()
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath);
            Require(prefab != null, "Player prefab was not found.");

            PlayerProgression progression = prefab.GetComponent<PlayerProgression>();
            Require(progression != null, "Player prefab is missing PlayerProgression.");
            Require(Mathf.Approximately(progression.DeathExperienceLossRate, 0.1f),
                "Player death experience loss rate must be 10%.");
            Require(progression.MinimumDeathExperienceLoss == 10,
                "Player minimum death experience loss must be 10.");
        }

        private static void ValidateProgressionBoundaries()
        {
            GameObject probe = new("PlayerDeathPenaltyValidation");

            try
            {
                probe.AddComponent<Health>();
                probe.AddComponent<CombatStats>();
                PlayerProgression progression = probe.AddComponent<PlayerProgression>();
                progression.ConfigureDeathPenalty(0.1f, 10);

                progression.RestoreState(2, 5);
                Require(progression.ApplyDeathPenalty() == 15,
                    "Level-boundary death loss must remove the calculated 15 experience.");
                Require(progression.Level == 1 && progression.CurrentExperience == 90,
                    "Death loss did not cross from level 2 into level 1 correctly.");

                progression.RestoreState(3, 20);
                Require(progression.ApplyDeathPenalty() == 20,
                    "A 10% level-3 penalty must remove 20 experience.");
                Require(progression.Level == 3 && progression.CurrentExperience == 0,
                    "Reaching exactly zero experience must preserve the current level.");
                Require(progression.ApplyDeathPenalty() == 20,
                    "A second level-3 penalty must still remove 20 experience.");
                Require(progression.Level == 2 && progression.CurrentExperience == 130,
                    "Death loss did not continue into the previous level correctly.");

                progression.RestoreState(1, 5);
                Require(progression.ApplyDeathPenalty() == 5,
                    "The level-1 floor must remove only available experience.");
                Require(progression.Level == 1 && progression.CurrentExperience == 0,
                    "Death loss must never reduce the player below level 1.");

                progression.ConfigureDeathPenalty(0f, 0);
                progression.RestoreState(2, 25);
                Require(progression.ApplyDeathPenalty() == 0,
                    "A disabled death penalty must not remove experience.");
                Require(progression.Level == 2 && progression.CurrentExperience == 25,
                    "A disabled death penalty changed progression state.");
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
                throw new InvalidOperationException($"Sprint 012 validation failed: {message}");
            }
        }
    }
}
