using System;
using System.Collections.Generic;
using ProjectGenesis.Data;
using ProjectGenesis.Gameplay;
using ProjectGenesis.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectGenesis.Tools.Editor
{
    public static class ParallelQuestsDevelopmentValidator
    {
        private const string ScenePath =
            "Assets/ProjectGenesis/Scenes/StarterVillage.unity";
        private const string WolfQuestPath =
            "Assets/ProjectGenesis/Data/Quests/SO_Quest_WolfTrophies.asset";
        private const string BoarQuestPath =
            "Assets/ProjectGenesis/Data/Quests/SO_Quest_BoarHunt.asset";
        private const string BoarPrefabPath =
            "Assets/ProjectGenesis/Prefabs/Enemies/PF_Enemy_Boar.prefab";

        [MenuItem("Project Genesis/Sprint 026/Validate Parallel Quests")]
        public static void ValidateParallelQuests()
        {
            QuestDefinition wolfQuest = LoadValidQuest(WolfQuestPath);
            QuestDefinition boarQuest = LoadValidQuest(BoarQuestPath);
            ValidateAuthoredData(wolfQuest, boarQuest);
            ValidateParallelRuntime(wolfQuest, boarQuest);
            ValidateScene(wolfQuest, boarQuest);
            Debug.Log("Sprint 026 parallel quests validation passed.");
        }

        [MenuItem("Project Genesis/Sprint 026/Validate Relevant Regression Suite")]
        public static void ValidateRelevantRegressionSuite()
        {
            ValidateParallelQuests();
            QuestDefinitionsDevelopmentValidator.ValidateRelevantRegressionSuite();
            Debug.Log("Sprint 026 relevant regression suite passed.");
        }

        private static QuestDefinition LoadValidQuest(string path)
        {
            QuestDefinition definition =
                AssetDatabase.LoadAssetAtPath<QuestDefinition>(path);
            Require(definition != null, $"Quest asset was not found at {path}.");
            Require(definition.TryValidate(out string error),
                $"Quest asset at {path} is invalid: {error}");
            return definition;
        }

        private static void ValidateAuthoredData(
            QuestDefinition wolfQuest,
            QuestDefinition boarQuest)
        {
            Require(wolfQuest.QuestId != boarQuest.QuestId,
                "Quest definitions must use unique stable ids.");
            Require(boarQuest.QuestId == "boars-near-the-road" &&
                    boarQuest.DisplayName == "Кабанья угроза",
                "Boar quest identity changed unexpectedly.");
            Require(string.IsNullOrWhiteSpace(boarQuest.PrerequisiteQuestId),
                "Boar quest must be available in parallel without a prerequisite.");
            Require(boarQuest.Objective.ObjectiveType == QuestObjectiveType.DefeatTarget &&
                    boarQuest.Objective.TargetId == "boar" &&
                    boarQuest.Objective.RequiredCount == 2,
                "Boar quest must require two defeated boars.");
            Require(boarQuest.Reward.Experience == 100,
                "Boar quest must grant 100 experience.");

            GameObject boarPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(BoarPrefabPath);
            Require(boarPrefab != null, "Boar prefab was not found.");
            EnemyBrain boar = boarPrefab.GetComponent<EnemyBrain>();
            Require(boar != null && boar.QuestTargetId == boarQuest.Objective.TargetId,
                "Boar prefab target id must advance the boar quest objective.");
        }

        private static void ValidateParallelRuntime(
            QuestDefinition wolfQuest,
            QuestDefinition boarQuest)
        {
            GameObject playerProbe = new("ParallelQuestPlayerValidation");
            GameObject restoreProbe = new("ParallelQuestRestoreValidation");
            GameObject trackerProbe = new("ParallelQuestTrackerValidation");
            try
            {
                PlayerProgression progression = playerProbe.AddComponent<PlayerProgression>();
                QuestLog questLog = playerProbe.AddComponent<QuestLog>();
                Require(questLog.TryAcceptQuest(wolfQuest, "Староста деревни"),
                    "Wolf quest could not be accepted.");
                Require(questLog.TryAcceptQuest(boarQuest, "Капитан стражи"),
                    "Boar quest could not be accepted while the wolf quest was active.");
                Require(questLog.GetQuestState(wolfQuest.QuestId) == QuestState.Active &&
                        questLog.GetQuestState(boarQuest.QuestId) == QuestState.Active,
                    "Both quests must remain active at the same time.");

                Require(questLog.ReportObjectiveProgress(wolfQuest.Objective.TargetId),
                    "Wolf objective did not advance.");
                questLog.ReportEnemyDefeated(boarQuest.Objective.TargetId);
                Require(questLog.GetQuestProgress(wolfQuest.QuestId).CurrentCount == 1 &&
                        questLog.GetQuestProgress(boarQuest.QuestId).CurrentCount == 1,
                    "Parallel objective counters must advance independently.");

                ValidateTracker(trackerProbe, questLog, wolfQuest, boarQuest);

                List<QuestProgressData> savedProgress = questLog.CaptureState();
                QuestLog restoredLog = restoreProbe.AddComponent<QuestLog>();
                restoredLog.RestoreState(savedProgress);
                Require(restoredLog.GetQuestState(wolfQuest.QuestId) == QuestState.Active &&
                        restoredLog.GetQuestState(boarQuest.QuestId) == QuestState.Active &&
                        restoredLog.GetQuestProgress(wolfQuest.QuestId).CurrentCount == 1 &&
                        restoredLog.GetQuestProgress(boarQuest.QuestId).CurrentCount == 1,
                    "Two simultaneous quests must restore from one profile state.");

                for (int index = 1; index < wolfQuest.Objective.RequiredCount; index++)
                {
                    Require(questLog.ReportObjectiveProgress(wolfQuest.Objective.TargetId),
                        $"Wolf objective failed at step {index + 1}.");
                }

                Require(questLog.ReportObjectiveProgress(boarQuest.Objective.TargetId),
                    "Boar objective did not reach its second step.");
                Require(questLog.GetQuestState(wolfQuest.QuestId) == QuestState.ReadyToTurnIn &&
                        questLog.GetQuestState(boarQuest.QuestId) == QuestState.ReadyToTurnIn,
                    "Both parallel quests must independently reach ready-to-turn-in.");

                Require(questLog.TryTurnInQuest(boarQuest.QuestId),
                    "Boar quest could not be turned in independently.");
                Require(questLog.GetQuestState(wolfQuest.QuestId) == QuestState.ReadyToTurnIn,
                    "Turning in the boar quest must not complete the wolf quest.");
                Require(questLog.TryTurnInQuest(wolfQuest.QuestId),
                    "Wolf quest could not be turned in after the boar quest.");
                Require(progression.Level == 2 && progression.CurrentExperience == 80,
                    "Independent quest rewards must grant their authored total experience.");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(playerProbe);
                UnityEngine.Object.DestroyImmediate(restoreProbe);
                UnityEngine.Object.DestroyImmediate(trackerProbe);
            }
        }

        private static void ValidateTracker(
            GameObject trackerProbe,
            QuestLog questLog,
            QuestDefinition wolfQuest,
            QuestDefinition boarQuest)
        {
            GameObject root = new("TrackerRoot");
            root.transform.SetParent(trackerProbe.transform);
            Text title = new GameObject(
                "TrackerTitle",
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Text)).GetComponent<Text>();
            title.transform.SetParent(root.transform);
            Text objective = new GameObject(
                "TrackerObjective",
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Text)).GetComponent<Text>();
            objective.transform.SetParent(root.transform);

            QuestTrackerView tracker = trackerProbe.AddComponent<QuestTrackerView>();
            tracker.Initialize(root, title, objective, questLog, null);
            tracker.RefreshNow();

            Require(root.activeSelf && title.text.Contains("(2)"),
                "Tracker must show that two quests are active.");
            Require(objective.text.Contains(wolfQuest.DisplayName) &&
                    objective.text.Contains(boarQuest.DisplayName),
                "Tracker must include both active quest titles.");
        }

        private static void ValidateScene(
            QuestDefinition wolfQuest,
            QuestDefinition boarQuest)
        {
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            InteractableNpc[] npcs =
                UnityEngine.Object.FindObjectsByType<InteractableNpc>(
                    FindObjectsInactive.Include,
                    FindObjectsSortMode.None);
            InteractableNpc elder = Array.Find(npcs, npc => npc.name == "NPC_VillageElder");
            InteractableNpc guard = Array.Find(npcs, npc => npc.name == "NPC_GuardCaptain");
            Require(elder != null && elder.DisplayName == "Староста деревни" &&
                    elder.QuestDefinition == wolfQuest,
                "Village elder must keep the wolf quest.");
            Require(guard != null && guard.DisplayName == "Капитан стражи" &&
                    guard.QuestDefinition == boarQuest,
                "Guard captain must own the boar quest.");

            QuestTrackerView[] trackers =
                UnityEngine.Object.FindObjectsByType<QuestTrackerView>(
                    FindObjectsInactive.Include,
                    FindObjectsSortMode.None);
            Require(trackers.Length == 1,
                "Starter scene must keep one shared parallel quest tracker.");

            Button[] buttons = UnityEngine.Object.FindObjectsByType<Button>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);
            Button clearTarget = Array.Find(
                buttons,
                button => button.name == "Button_ClearTarget");
            Text clearLabel = clearTarget != null
                ? clearTarget.GetComponentInChildren<Text>(true)
                : null;
            Require(clearLabel != null && clearLabel.text == "×" &&
                    clearLabel.fontStyle == FontStyle.Bold && clearLabel.fontSize >= 24,
                "Selected-target close button must show a readable close symbol.");
        }

        private static void Require(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException(
                    $"Sprint 026 validation failed: {message}");
            }
        }
    }
}
