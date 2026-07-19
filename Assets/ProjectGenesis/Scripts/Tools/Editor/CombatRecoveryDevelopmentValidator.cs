using ProjectGenesis.Gameplay;
using UnityEditor;
using UnityEngine;

namespace ProjectGenesis.Tools.Editor
{
    public static class CombatRecoveryDevelopmentValidator
    {
        private const string PlayerPrefabPath = "Assets/ProjectGenesis/Prefabs/Characters/PF_Player_Prototype.prefab";
        private const string WolfPrefabPath = "Assets/ProjectGenesis/Prefabs/Enemies/PF_Enemy_Wolf.prefab";

        [MenuItem("Project Genesis/Sprint 010/Validate Combat Recovery")]
        public static void ValidateCombatRecovery()
        {
            ValidateHealthBoundaries();
            ValidateRecoverySettings(PlayerPrefabPath, 8f, 2, 1f, true);
            ValidateRecoverySettings(WolfPrefabPath, 5f, 3, 1f, false);

            EnemyBrain wolf = LoadRequiredPrefab(WolfPrefabPath).GetComponent<EnemyBrain>();
            Require(wolf != null, "Wolf prefab is missing EnemyBrain.");
            Require(Mathf.Approximately(wolf.LeashRadius, 6f), "Wolf leash radius must be 6 metres.");
            Require(Mathf.Approximately(wolf.CorpseLifetime, 6f), "Wolf corpse lifetime must remain 6 seconds.");

            Debug.Log("Sprint 010 combat recovery validation passed.");
        }

        private static void ValidateHealthBoundaries()
        {
            GameObject probe = new("CombatRecoveryValidationProbe");

            try
            {
                Health health = probe.AddComponent<Health>();
                health.Configure(10);

                Require(health.TakeDamage(4) && health.CurrentHealth == 6, "Health damage validation failed.");
                Require(health.Heal(3) && health.CurrentHealth == 9, "Partial healing validation failed.");
                Require(health.Heal(50) && health.CurrentHealth == 10, "Healing must clamp to maximum health.");
                Require(health.TakeDamage(10) && health.IsDead, "Death boundary validation failed.");
                Require(!health.Heal(1) && health.CurrentHealth == 0, "Regeneration must not revive dead characters.");
            }
            finally
            {
                Object.DestroyImmediate(probe);
            }
        }

        private static void ValidateRecoverySettings(
            string prefabPath,
            float expectedDelay,
            int expectedAmount,
            float expectedInterval,
            bool expectedStartsEnabled)
        {
            HealthRegeneration regeneration = LoadRequiredPrefab(prefabPath).GetComponent<HealthRegeneration>();
            Require(regeneration != null, $"Prefab '{prefabPath}' is missing HealthRegeneration.");
            Require(Mathf.Approximately(regeneration.RecoveryDelay, expectedDelay),
                $"Prefab '{prefabPath}' has an unexpected recovery delay.");
            Require(regeneration.HealAmount == expectedAmount,
                $"Prefab '{prefabPath}' has an unexpected heal amount.");
            Require(Mathf.Approximately(regeneration.TickInterval, expectedInterval),
                $"Prefab '{prefabPath}' has an unexpected recovery interval.");
            Require(regeneration.StartsEnabled == expectedStartsEnabled,
                $"Prefab '{prefabPath}' has an unexpected start-enabled value.");
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
                throw new System.InvalidOperationException(message);
            }
        }
    }
}
