using System;
using ProjectGenesis.Gameplay;
using ProjectGenesis.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectGenesis.Tools.Editor
{
    public static class MapFoundationDevelopmentValidator
    {
        private const string ScenePath =
            "Assets/ProjectGenesis/Scenes/StarterVillage.unity";

        [MenuItem("Project Genesis/Sprint 031/Validate Map Foundation")]
        public static void ValidateMapFoundation()
        {
            ValidateRuntimeMapping();
            ValidateSceneWiring();
            Debug.Log("Sprint 031 map foundation validation passed.");
        }

        [MenuItem("Project Genesis/Sprint 031/Validate Relevant Regression Suite")]
        public static void ValidateRelevantRegressionSuite()
        {
            ValidateMapFoundation();
            EliteQuestAndMerchantDevelopmentValidator.ValidateRelevantRegressionSuite();
            Debug.Log("Sprint 031 relevant regression suite passed.");
        }

        private static void ValidateRuntimeMapping()
        {
            GameObject player = new("MapPlayerValidation");
            GameObject root = new("MapRootValidation");
            try
            {
                WorldMapView map = root.AddComponent<WorldMapView>();
                map.Initialize(
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    player.transform,
                    null,
                    new Vector2(-8.5f, -8.5f),
                    new Vector2(18.5f, 18.5f));

                Vector2 center = map.WorldToMapPosition(
                    new Vector3(5f, 0f, 5f),
                    new Vector2(270f, 270f));
                Require(Mathf.Abs(center.x) < 0.01f && Mathf.Abs(center.y) < 0.01f,
                    "World center must map to the visual center.");

                Vector2 minimum = map.WorldToMapPosition(
                    new Vector3(-8.5f, 0f, -8.5f),
                    new Vector2(270f, 270f));
                Require(Mathf.Approximately(minimum.x, -135f) &&
                        Mathf.Approximately(minimum.y, -135f),
                    "World minimum must map to the lower-left map edge.");

                Vector2 maximum = map.WorldToMapPosition(
                    new Vector3(18.5f, 0f, 18.5f),
                    new Vector2(270f, 270f));
                Require(Mathf.Approximately(maximum.x, 135f) &&
                        Mathf.Approximately(maximum.y, 135f),
                    "World maximum must map to the upper-right map edge.");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(root);
                UnityEngine.Object.DestroyImmediate(player);
            }
        }

        private static void ValidateSceneWiring()
        {
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            WorldMapView map = UnityEngine.Object.FindFirstObjectByType<WorldMapView>(
                FindObjectsInactive.Include);
            Require(map != null, "World map view is missing from the scene.");
            RectTransform root = map.GetComponent<RectTransform>();
            Require(root != null &&
                    root.anchorMin == Vector2.zero &&
                    root.anchorMax == Vector2.one,
                "World map root must stretch across the canvas so screen-edge anchors work.");
            Require(map.MiniMapRoot != null && map.MiniMapRoot.name == "MiniMapSurface",
                "Mini-map surface must be wired.");
            Require(map.LargeMapRoot != null && map.LargeMapRoot.name == "WorldMapWindow" &&
                    !map.IsLargeMapOpen,
                "Large map window must be wired and closed by default.");
            Require(map.MiniPlayerMarker != null && map.LargePlayerMarker != null,
                "Both player map markers must be wired.");
            Require(map.MiniZoneText != null && map.LargeZoneText != null,
                "Map zone labels must be wired.");
            RectTransform miniPanel =
                GameObject.Find("MiniMapPanel")?.GetComponent<RectTransform>();
            Require(miniPanel != null &&
                    miniPanel.anchorMin == Vector2.one &&
                    miniPanel.anchorMax == Vector2.one &&
                    Mathf.Approximately(miniPanel.anchoredPosition.x, 0f),
                "Mini-map panel must be pinned to the right screen edge.");
            RectTransform miniZoneRect = map.MiniZoneText.GetComponent<RectTransform>();
            float zoneBottomFromTop =
                Mathf.Abs(miniZoneRect.anchoredPosition.y) + miniZoneRect.sizeDelta.y;
            float mapTopFromTop =
                miniPanel.sizeDelta.y -
                (map.MiniMapRoot.anchoredPosition.y + map.MiniMapRoot.sizeDelta.y);
            Require(mapTopFromTop >= zoneBottomFromTop + 8f,
                "Mini-map surface must not overlap the title or zone label.");
            Require(map.LargeMapRoot.GetComponentInChildren<DraggableWindow>(true) != null,
                "Large map window must be draggable like the other temporary windows.");
            Require(Mathf.Approximately(map.WorldMinimum.x, -8.5f) &&
                    Mathf.Approximately(map.WorldMinimum.y, -8.5f) &&
                    Mathf.Approximately(map.WorldMaximum.x, 18.5f) &&
                    Mathf.Approximately(map.WorldMaximum.y, 18.5f),
                "Map world bounds must cover the starter village, north wilds, and elite clearing.");

            Require(GameObject.Find("MapArea_StarterVillage") != null &&
                    GameObject.Find("MapArea_NorthWilds") != null &&
                    GameObject.Find("MapArea_EliteClearing") != null,
                "Map must include the village, north wilds, and elite clearing areas.");

            PlayerZoneController zoneController = UnityEngine.Object.FindFirstObjectByType<PlayerZoneController>(
                FindObjectsInactive.Include);
            Require(zoneController != null && zoneController.ZoneVolumes.Length >= 3,
                "Map depends on the authored starter zone volumes remaining present.");
        }

        private static void Require(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException(
                    $"Sprint 031 validation failed: {message}");
            }
        }
    }
}
