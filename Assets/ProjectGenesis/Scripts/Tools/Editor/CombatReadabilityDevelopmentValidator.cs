using System;
using System.Reflection;
using ProjectGenesis.Gameplay;
using ProjectGenesis.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectGenesis.Tools.Editor
{
    public static class CombatReadabilityDevelopmentValidator
    {
        private const string ScenePath =
            "Assets/ProjectGenesis/Scenes/StarterVillage.unity";
        private const string WolfPrefabPath =
            "Assets/ProjectGenesis/Prefabs/Enemies/PF_Enemy_Wolf.prefab";
        private const string AlphaPrefabPath =
            "Assets/ProjectGenesis/Prefabs/Enemies/PF_Enemy_WolfAlpha.prefab";

        [MenuItem("Project Genesis/Sprint 029/Validate Combat Readability")]
        public static void ValidateCombatReadability()
        {
            ValidateSceneHudWiring();
            ValidateStatusTextRuntime();
            Debug.Log("Sprint 029 combat readability validation passed.");
        }

        [MenuItem("Project Genesis/Sprint 029/Validate Relevant Regression Suite")]
        public static void ValidateRelevantRegressionSuite()
        {
            ValidateCombatReadability();
            EliteEncounterDevelopmentValidator.ValidateRelevantRegressionSuite();
        }

        private static void ValidateSceneHudWiring()
        {
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            CombatHudView hud = UnityEngine.Object.FindFirstObjectByType<CombatHudView>(
                FindObjectsInactive.Include);
            GameObject targetPanel = FindGameObject("CombatHud_Target");
            Text statusText = FindText("Text_TargetStatus");

            Require(hud != null, "Combat HUD view is missing.");
            Require(targetPanel != null, "Selected-target panel is missing.");
            Require(statusText != null &&
                    statusText.transform.IsChildOf(targetPanel.transform) &&
                    statusText.GetComponent<RectTransform>().sizeDelta.y >= 20f,
                "Selected-target status text must exist inside the target panel.");
            Require(targetPanel.GetComponent<RectTransform>().sizeDelta.y >= 128f,
                "Selected-target panel must leave room for the status line.");
        }

        private static void ValidateStatusTextRuntime()
        {
            MethodInfo buildWarning = typeof(CombatHudView).GetMethod(
                "BuildEnemyWarning",
                BindingFlags.Static | BindingFlags.NonPublic);
            Require(buildWarning != null, "Combat HUD warning builder is missing.");

            GameObject wolfPrefab =
                AssetDatabase.LoadAssetAtPath<GameObject>(WolfPrefabPath);
            GameObject alphaPrefab =
                AssetDatabase.LoadAssetAtPath<GameObject>(AlphaPrefabPath);
            Require(wolfPrefab != null && alphaPrefab != null,
                "Enemy prefabs required for combat readability validation are missing.");

            string wolfStatus = (string)buildWarning.Invoke(
                null,
                new object[] { wolfPrefab.GetComponent<EnemyBrain>() });
            Require(string.IsNullOrEmpty(wolfStatus),
                "Regular enemies must not show a permanent explanatory target line.");

            GameObject alpha = UnityEngine.Object.Instantiate(alphaPrefab);
            GameObject target = new("CombatReadabilityTarget");
            try
            {
                TelegraphedEnemyAttack special =
                    alpha.GetComponent<TelegraphedEnemyAttack>();
                Health targetHealth = target.AddComponent<Health>();
                targetHealth.Configure(100);
                CombatStats targetStats = target.AddComponent<CombatStats>();
                targetStats.Configure(10, 3, 1f, 1f);
                LocalMessageStream messages = target.AddComponent<LocalMessageStream>();

                special.ResetCycle(0f);
                special.UpdateAttack(
                    2.5f,
                    1f,
                    targetHealth,
                    targetStats,
                    messages,
                    "Вожак стаи");

                string alphaStatus = (string)buildWarning.Invoke(
                    null,
                    new object[] { alpha.GetComponent<EnemyBrain>() });
                Require(alphaStatus.Contains("Опасно") &&
                        alphaStatus.Contains("Мощный укус") &&
                        alphaStatus.Contains("Отойдите"),
                    "Elite windup status must name the attack and tell the player to move.");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(target);
                UnityEngine.Object.DestroyImmediate(alpha);
            }
        }

        private static Text FindText(string objectName)
        {
            Text[] texts = UnityEngine.Object.FindObjectsByType<Text>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);
            foreach (Text text in texts)
            {
                if (text.name == objectName)
                {
                    return text;
                }
            }

            return null;
        }

        private static GameObject FindGameObject(string objectName)
        {
            Transform[] transforms = UnityEngine.Object.FindObjectsByType<Transform>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);
            foreach (Transform transform in transforms)
            {
                if (transform.name == objectName)
                {
                    return transform.gameObject;
                }
            }

            return null;
        }

        private static void Require(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException(
                    $"Sprint 029 validation failed: {message}");
            }
        }
    }
}
