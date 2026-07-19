using ProjectGenesis.Gameplay;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ProjectGenesis.Tools.Editor
{
    public static class EnemyTerritoryDevelopmentValidator
    {
        private const string ScenePath = "Assets/ProjectGenesis/Scenes/StarterVillage.unity";
        private const string WolfPrefabPath = "Assets/ProjectGenesis/Prefabs/Enemies/PF_Enemy_Wolf.prefab";

        [MenuItem("Project Genesis/Sprint 011/Validate Enemy Territory")]
        public static void ValidateEnemyTerritory()
        {
            ValidateWolfPrefab();
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

            EnemyTerritory[] territories =
                Object.FindObjectsByType<EnemyTerritory>(FindObjectsSortMode.None);
            Require(territories.Length == 1, "Starter Village must contain exactly one enemy territory.");

            EnemyTerritory territory = territories[0];
            Require(territory.name == "Zone_NorthCombat", "The combat territory has an unexpected name.");
            Require(Approximately(territory.Size, new Vector2(15.6f, 9.2f)),
                "The combat territory has unexpected dimensions.");
            Require(Mathf.Approximately(territory.EdgePadding, 0.2f),
                "The combat territory must keep 0.2 metres of inner padding.");
            Require(territory.Contains(territory.transform.position),
                "The combat territory must contain its own center.");
            Require(!territory.Contains(new Vector3(0f, 0f, 8.4f)),
                "The northern village gate must remain outside the hostile territory.");

            EnemySpawner[] spawners =
                Object.FindObjectsByType<EnemySpawner>(FindObjectsSortMode.None);
            Require(spawners.Length == 3, "Starter Village must contain exactly three enemy spawners.");

            foreach (EnemySpawner spawner in spawners)
            {
                Require(spawner.Territory == territory,
                    $"Spawner '{spawner.name}' is not assigned to the northern combat territory.");
                Require(territory.Contains(spawner.transform.position),
                    $"Spawner '{spawner.name}' is outside its assigned territory.");
                Require(Mathf.Approximately(spawner.RespawnDelay, 12f),
                    $"Spawner '{spawner.name}' has an unexpected respawn delay.");
            }

            Debug.Log("Sprint 011 enemy territory validation passed.");
        }

        private static void ValidateWolfPrefab()
        {
            GameObject wolfPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(WolfPrefabPath);
            Require(wolfPrefab != null, $"Wolf prefab was not found at '{WolfPrefabPath}'.");

            EnemyBrain brain = wolfPrefab.GetComponent<EnemyBrain>();
            Require(brain != null, "Wolf prefab is missing EnemyBrain.");
            Require(Mathf.Approximately(brain.DetectionRadius, 2.2f),
                "Wolf detection radius must be 2.2 metres.");
            Require(Mathf.Approximately(brain.LeashRadius, 6f),
                "Wolf leash radius must be 6 metres.");
            Require(Mathf.Approximately(brain.RoamingRadius, 2.4f),
                "Wolf roaming radius must be 2.4 metres.");
            Require(Mathf.Approximately(brain.MinimumIdleDelay, 0.5f),
                "Wolf minimum idle delay must be 0.5 seconds.");
            Require(Mathf.Approximately(brain.MaximumIdleDelay, 1.5f),
                "Wolf maximum idle delay must be 1.5 seconds.");
            Require(brain.RoamingRadius < brain.LeashRadius,
                "Wolf roaming radius must remain smaller than its leash radius.");
        }

        private static bool Approximately(Vector2 first, Vector2 second)
        {
            return Mathf.Approximately(first.x, second.x) &&
                   Mathf.Approximately(first.y, second.y);
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
