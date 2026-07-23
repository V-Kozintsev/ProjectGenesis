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
    public static class EliteQuestAndMerchantDevelopmentValidator
    {
        private const string ScenePath =
            "Assets/ProjectGenesis/Scenes/StarterVillage.unity";
        private const string BoarQuestPath =
            "Assets/ProjectGenesis/Data/Quests/SO_Quest_BoarHunt.asset";
        private const string AlphaQuestPath =
            "Assets/ProjectGenesis/Data/Quests/SO_Quest_WolfAlphaHunt.asset";
        private const string AlphaPrefabPath =
            "Assets/ProjectGenesis/Prefabs/Enemies/PF_Enemy_WolfAlpha.prefab";

        [MenuItem("Project Genesis/Sprint 030/Validate Elite Quest And Merchant")]
        public static void ValidateEliteQuestAndMerchant()
        {
            QuestDefinition boarQuest = LoadValidQuest(BoarQuestPath);
            QuestDefinition alphaQuest = LoadValidQuest(AlphaQuestPath);
            ValidateAuthoredData(boarQuest, alphaQuest);
            ValidateRuntimeQuestFlow(boarQuest, alphaQuest);
            ValidateMerchantDialogueFallback();
            ValidateScene(boarQuest, alphaQuest);
            Debug.Log("Sprint 030 elite quest and merchant validation passed.");
        }

        [MenuItem("Project Genesis/Sprint 030/Validate Relevant Regression Suite")]
        public static void ValidateRelevantRegressionSuite()
        {
            ValidateEliteQuestAndMerchant();
            CombatReadabilityDevelopmentValidator.ValidateRelevantRegressionSuite();
            Debug.Log("Sprint 030 relevant regression suite passed.");
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
            QuestDefinition boarQuest,
            QuestDefinition alphaQuest)
        {
            Require(alphaQuest.QuestId == "wolf-alpha-hunt" &&
                    alphaQuest.DisplayName == "Вожак стаи",
                "Elite quest identity changed unexpectedly.");
            Require(alphaQuest.PrerequisiteQuestId == boarQuest.QuestId,
                "Elite quest must unlock after the boar hunt.");
            Require(alphaQuest.Objective.ObjectiveType == QuestObjectiveType.DefeatTarget &&
                    alphaQuest.Objective.TargetId == "wolf_alpha" &&
                    alphaQuest.Objective.RequiredCount == 1,
                "Elite quest must require one Wolf Alpha defeat.");
            Require(alphaQuest.Reward.Experience == 160,
                "Elite quest must grant 160 experience.");

            GameObject alphaPrefab =
                AssetDatabase.LoadAssetAtPath<GameObject>(AlphaPrefabPath);
            Require(alphaPrefab != null, "Wolf Alpha prefab was not found.");
            EnemyBrain alpha = alphaPrefab.GetComponent<EnemyBrain>();
            Require(alpha != null &&
                    alpha.QuestTargetId == alphaQuest.Objective.TargetId,
                "Wolf Alpha prefab target id must advance the elite quest.");
        }

        private static void ValidateRuntimeQuestFlow(
            QuestDefinition boarQuest,
            QuestDefinition alphaQuest)
        {
            GameObject playerProbe = new("EliteQuestPlayerValidation");
            GameObject guardProbe = new("EliteQuestGuardValidation");
            try
            {
                PlayerProgression progression = playerProbe.AddComponent<PlayerProgression>();
                QuestLog questLog = playerProbe.AddComponent<QuestLog>();
                InteractableNpc guard = guardProbe.AddComponent<InteractableNpc>();
                guard.ConfigureDisplayName("Капитан стражи");
                guard.ConfigureQuests(boarQuest, alphaQuest);

                Require(guard.ResolveQuestDefinition(questLog) == boarQuest,
                    "Guard must offer the boar hunt before the elite quest.");
                Require(!questLog.TryAcceptQuest(alphaQuest, guard.DisplayName),
                    "Elite quest must not be accepted before its prerequisite is complete.");

                Require(questLog.TryAcceptQuest(boarQuest, guard.DisplayName),
                    "Boar quest could not be accepted.");
                questLog.ReportEnemyDefeated(boarQuest.Objective.TargetId);
                questLog.ReportEnemyDefeated(boarQuest.Objective.TargetId);
                Require(questLog.TryTurnInQuest(boarQuest.QuestId),
                    "Boar quest could not be completed.");

                Require(guard.ResolveQuestDefinition(questLog) == alphaQuest,
                    "Guard must offer the elite quest after the boar hunt is complete.");
                Require(questLog.TryAcceptQuest(alphaQuest, guard.DisplayName),
                    "Elite quest could not be accepted after its prerequisite.");
                Require(!questLog.ReportObjectiveProgress("wolf"),
                    "Regular wolf defeats must not advance the elite quest.");
                Require(questLog.ReportObjectiveProgress(alphaQuest.Objective.TargetId),
                    "Wolf Alpha defeat did not advance the elite quest.");
                Require(questLog.GetQuestState(alphaQuest.QuestId) == QuestState.ReadyToTurnIn,
                    "Elite quest must become ready after one Wolf Alpha defeat.");
                Require(questLog.TryTurnInQuest(alphaQuest.QuestId),
                    "Elite quest could not be turned in.");
                Require(progression.Level == 3 && progression.CurrentExperience == 10,
                    "Boar and elite quest rewards must apply through progression.");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(guardProbe);
                UnityEngine.Object.DestroyImmediate(playerProbe);
            }
        }

        private static void ValidateMerchantDialogueFallback()
        {
            GameObject merchantProbe = new("MerchantDialogueValidation");
            GameObject playerProbe = new("MerchantQuestLogValidation");
            try
            {
                InteractableNpc merchant = merchantProbe.AddComponent<InteractableNpc>();
                merchant.ConfigureDisplayName("Деревенский торговец");
                merchant.ConfigureQuests(null);
                QuestLog questLog = playerProbe.AddComponent<QuestLog>();
                Require(merchant.ResolveQuestDefinition(questLog) == null,
                    "Merchant placeholder must not expose a quest.");

                GameObject root = new("DialogueRoot");
                root.transform.SetParent(merchantProbe.transform);
                UnityEngine.UI.Text speaker =
                    CreateProbeText("Speaker", root.transform);
                UnityEngine.UI.Text body =
                    CreateProbeText("Body", root.transform);
                UnityEngine.UI.Text quest =
                    CreateProbeText("Quest", root.transform);
                UnityEngine.UI.Button accept =
                    new GameObject("Accept", typeof(RectTransform), typeof(UnityEngine.UI.Button))
                        .GetComponent<UnityEngine.UI.Button>();
                accept.transform.SetParent(root.transform);
                UnityEngine.UI.Button close =
                    new GameObject("Close", typeof(RectTransform), typeof(UnityEngine.UI.Button))
                        .GetComponent<UnityEngine.UI.Button>();
                close.transform.SetParent(root.transform);

                DialogueWindow window = merchantProbe.AddComponent<DialogueWindow>();
                window.Initialize(root, speaker, body, quest, accept, close);
                window.Show(merchant, questLog);
                Require(speaker.text == "Деревенский торговец" &&
                        body.text.Contains("Торговля") &&
                        !accept.gameObject.activeSelf,
                    "Questless merchant dialogue must show a readable placeholder without a quest action.");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(playerProbe);
                UnityEngine.Object.DestroyImmediate(merchantProbe);
            }
        }

        private static UnityEngine.UI.Text CreateProbeText(
            string name,
            Transform parent)
        {
            UnityEngine.UI.Text text = new GameObject(
                name,
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(UnityEngine.UI.Text)).GetComponent<UnityEngine.UI.Text>();
            text.transform.SetParent(parent);
            return text;
        }

        private static void ValidateScene(
            QuestDefinition boarQuest,
            QuestDefinition alphaQuest)
        {
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            InteractableNpc[] npcs =
                UnityEngine.Object.FindObjectsByType<InteractableNpc>(
                    FindObjectsInactive.Include,
                    FindObjectsSortMode.None);
            InteractableNpc guard = Array.Find(npcs, npc => npc.name == "NPC_GuardCaptain");
            InteractableNpc merchant = Array.Find(npcs, npc => npc.name == "NPC_VillageMerchant");

            Require(guard != null &&
                    guard.DisplayName == "Капитан стражи" &&
                    guard.QuestDefinition == boarQuest &&
                    guard.QuestDefinitions.Contains(alphaQuest),
                "Guard captain must own the boar quest and the elite follow-up quest.");
            Require(merchant != null &&
                    merchant.DisplayName == "Деревенский торговец" &&
                    !merchant.QuestDefinitions.Any(),
                "Village merchant must exist as a questless placeholder NPC.");
        }

        private static void Require(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException(
                    $"Sprint 030 validation failed: {message}");
            }
        }
    }
}
