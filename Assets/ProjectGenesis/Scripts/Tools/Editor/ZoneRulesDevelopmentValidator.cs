using System;
using ProjectGenesis.Core;
using ProjectGenesis.Data;
using ProjectGenesis.Gameplay;
using ProjectGenesis.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ProjectGenesis.Tools.Editor
{
    public static class ZoneRulesDevelopmentValidator
    {
        private const string ScenePath =
            "Assets/ProjectGenesis/Scenes/StarterVillage.unity";
        private const string PlayerPrefabPath =
            "Assets/ProjectGenesis/Prefabs/Characters/PF_Player_Prototype.prefab";
        private const string VillageZonePath =
            "Assets/ProjectGenesis/Data/Zones/SO_Zone_StarterVillage.asset";
        private const string NorthWildsZonePath =
            "Assets/ProjectGenesis/Data/Zones/SO_Zone_NorthWilds.asset";

        [MenuItem("Project Genesis/Sprint 027/Validate Zone Rules Foundation")]
        public static void ValidateZoneRulesFoundation()
        {
            WorldZoneDefinition villageZone =
                AssetDatabase.LoadAssetAtPath<WorldZoneDefinition>(VillageZonePath);
            WorldZoneDefinition northWildsZone =
                AssetDatabase.LoadAssetAtPath<WorldZoneDefinition>(NorthWildsZonePath);

            ValidateDefinitions(villageZone, northWildsZone);
            ValidateRuntimeResolution(villageZone, northWildsZone);
            ValidateScene(villageZone, northWildsZone);
            Debug.Log("Sprint 027 zone rules validation passed.");
        }

        [MenuItem("Project Genesis/Sprint 027/Validate Relevant Regression Suite")]
        public static void ValidateRelevantRegressionSuite()
        {
            ValidateZoneRulesFoundation();
            ParallelQuestsDevelopmentValidator.ValidateRelevantRegressionSuite();
        }

        private static void ValidateDefinitions(
            WorldZoneDefinition villageZone,
            WorldZoneDefinition northWildsZone)
        {
            Require(villageZone != null && villageZone.IsValid,
                "Starter village zone asset is missing or invalid.");
            Require(northWildsZone != null && northWildsZone.IsValid,
                "North wilds zone asset is missing or invalid.");
            Require(villageZone.ZoneId == "starter-village" &&
                    villageZone.DisplayName == "Стартовая деревня" &&
                    villageZone.ZoneType == WorldZoneType.Peaceful &&
                    !villageZone.AllowsCombat &&
                    !string.IsNullOrWhiteSpace(villageZone.CombatBlockedMessage),
                "Starter village must be an authored peaceful zone with feedback.");
            Require(northWildsZone.ZoneId == "north-wilds" &&
                    northWildsZone.DisplayName == "Северные окрестности" &&
                    northWildsZone.ZoneType == WorldZoneType.Combat &&
                    northWildsZone.AllowsCombat,
                "North wilds must be an authored combat zone.");
            Require(villageZone.ZoneId != northWildsZone.ZoneId,
                "World zone ids must be unique.");

            WorldZoneDefinition invalidZone =
                ScriptableObject.CreateInstance<WorldZoneDefinition>();
            try
            {
                invalidZone.Configure(
                    string.Empty,
                    string.Empty,
                    WorldZoneType.Peaceful,
                    string.Empty,
                    string.Empty);
                Require(!invalidZone.TryValidate(out _),
                    "Invalid world-zone data must be rejected.");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(invalidZone);
            }
        }

        private static void ValidateRuntimeResolution(
            WorldZoneDefinition villageZone,
            WorldZoneDefinition northWildsZone)
        {
            GameObject safeObject = new("SafeZoneProbe");
            GameObject combatObject = new("CombatZoneProbe");
            GameObject player = new("PlayerZoneProbe");
            try
            {
                WorldZoneVolume safeVolume = safeObject.AddComponent<WorldZoneVolume>();
                safeVolume.Configure(villageZone, new Vector2(10f, 10f), 0f, 100);
                WorldZoneVolume combatVolume = combatObject.AddComponent<WorldZoneVolume>();
                combatVolume.Configure(northWildsZone, new Vector2(20f, 20f), 0f, 10);
                Require(safeVolume.TryValidate(out _) && combatVolume.TryValidate(out _),
                    "Configured world-zone volumes must validate.");

                LocalMessageStream messages = player.AddComponent<LocalMessageStream>();
                PlayerZoneController controller = player.AddComponent<PlayerZoneController>();
                controller.Configure(
                    northWildsZone,
                    new[] { combatVolume, safeVolume });

                player.transform.position = Vector3.zero;
                controller.RefreshNow(false);
                int zoneChanges = 0;
                controller.ZoneChanged += _ => zoneChanges++;
                Require(controller.CurrentZone == villageZone &&
                        !controller.IsCombatAllowed,
                    "Higher-priority peaceful volume must win an overlap.");
                Require(!controller.TryAuthorizeCombat(),
                    "Peaceful zone must refuse combat authorization.");
                Require(messages.Entries.Count == 1 &&
                        messages.Entries[0].Text == villageZone.CombatBlockedMessage,
                    "Peaceful combat refusal must publish its authored reason.");

                player.transform.position = new Vector3(7f, 0f, 0f);
                controller.RefreshNow();
                Require(controller.CurrentZone == northWildsZone &&
                        controller.IsCombatAllowed && controller.TryAuthorizeCombat(),
                    "Combat volume must allow combat outside the safe volume.");
                Require(messages.Entries[^1].Text == northWildsZone.EnterMessage,
                    "Entering the combat zone must publish its authored message.");

                player.transform.position = Vector3.zero;
                controller.RefreshNow();
                Require(controller.CurrentZone == villageZone && zoneChanges == 2,
                    "Returning to the peaceful zone must publish one zone change.");
                Require(messages.Entries[^1].Text == villageZone.EnterMessage,
                    "Returning to the village must publish its authored message.");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(player);
                UnityEngine.Object.DestroyImmediate(combatObject);
                UnityEngine.Object.DestroyImmediate(safeObject);
            }
        }

        private static void ValidateScene(
            WorldZoneDefinition villageZone,
            WorldZoneDefinition northWildsZone)
        {
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            WorldZoneVolume[] volumes =
                UnityEngine.Object.FindObjectsByType<WorldZoneVolume>(
                    FindObjectsInactive.Include,
                    FindObjectsSortMode.None);
            Require(volumes.Length >= 2,
                "Starter scene must contain its peaceful volume and at least one combat volume.");

            WorldZoneVolume[] safeVolumes = Array.FindAll(
                volumes,
                volume => volume.Definition == villageZone);
            WorldZoneVolume[] combatVolumes = Array.FindAll(
                volumes,
                volume => volume.Definition == northWildsZone);
            Require(safeVolumes.Length == 1,
                "Starter scene must contain exactly one village safe volume.");
            Require(combatVolumes.Length >= 1,
                "Starter scene must contain at least one north-wilds combat volume.");
            WorldZoneVolume safeVolume = safeVolumes[0];
            WorldZoneVolume combatVolume = Array.Find(
                combatVolumes,
                volume => volume.name == "Zone_NorthCombat");
            Require(safeVolume != null && safeVolume.name == "Zone_StarterVillageSafe" &&
                    safeVolume.Priority > (combatVolume != null ? combatVolume.Priority : int.MaxValue),
                "Starter village safe volume must exist with the higher priority.");
            Require(combatVolume != null && combatVolume.name == "Zone_NorthCombat" &&
                    combatVolume.GetComponent<EnemyTerritory>() != null,
                "North combat rules must share the existing enemy territory object.");

            PlayerSpawnPoint spawnPoint =
                UnityEngine.Object.FindFirstObjectByType<PlayerSpawnPoint>();
            Require(spawnPoint != null && safeVolume.Contains(spawnPoint.transform.position),
                "Village respawn point must remain inside the peaceful volume.");
            Require(Array.TrueForAll(
                    combatVolumes,
                    volume => !volume.Contains(spawnPoint.transform.position)),
                "Village respawn point must not be inside a north-wilds combat volume.");

            PlayerZoneController sceneController =
                UnityEngine.Object.FindFirstObjectByType<PlayerZoneController>(
                    FindObjectsInactive.Include);
            Require(sceneController != null &&
                    sceneController.DefaultZone == northWildsZone &&
                    sceneController.ZoneVolumes.Length == volumes.Length &&
                    Array.TrueForAll(
                        volumes,
                        volume => Array.IndexOf(sceneController.ZoneVolumes, volume) >= 0),
                "Scene player must reference every authored volume and a combat fallback.");

            PlayerDeathController deathController =
                sceneController.GetComponent<PlayerDeathController>();
            Require(deathController != null && deathController.RespawnPoint == spawnPoint.transform,
                "Death flow must still use the peaceful village spawn point.");

            CharacterEntryView entryView =
                UnityEngine.Object.FindFirstObjectByType<CharacterEntryView>(
                    FindObjectsInactive.Include);
            Require(entryView != null && EntryGateContains(entryView, sceneController),
                "Character entry must gate the player zone controller with gameplay.");

            GameObject playerPrefab =
                AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath);
            PlayerZoneController prefabController = playerPrefab != null
                ? playerPrefab.GetComponent<PlayerZoneController>()
                : null;
            Require(prefabController != null && prefabController.DefaultZone == northWildsZone,
                "Player prefab must keep the combat fallback zone definition.");
        }

        private static bool EntryGateContains(
            CharacterEntryView entryView,
            MonoBehaviour expectedBehaviour)
        {
            SerializedObject serializedEntry = new(entryView);
            SerializedProperty behaviours = serializedEntry.FindProperty("gameplayBehaviours");
            for (int index = 0; index < behaviours.arraySize; index++)
            {
                if (behaviours.GetArrayElementAtIndex(index).objectReferenceValue ==
                    expectedBehaviour)
                {
                    return true;
                }
            }

            return false;
        }

        private static void Require(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException(
                    $"Sprint 027 validation failed: {message}");
            }
        }
    }
}
