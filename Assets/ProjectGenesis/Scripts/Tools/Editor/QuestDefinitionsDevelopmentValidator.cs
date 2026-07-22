using System;
using System.Collections.Generic;
using ProjectGenesis.Data;
using ProjectGenesis.Gameplay;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ProjectGenesis.Tools.Editor
{
    public static class QuestDefinitionsDevelopmentValidator
    {
        private const string ScenePath =
            "Assets/ProjectGenesis/Scenes/StarterVillage.unity";
        private const string QuestPath =
            "Assets/ProjectGenesis/Data/Quests/SO_Quest_WolfTrophies.asset";

        [MenuItem("Project Genesis/Sprint 025/Validate Quest Definitions")]
        public static void ValidateQuestDefinitions()
        {
            QuestDefinition definition = ValidateDefinitionAsset();
            ValidateDefinitionDrivenProgress(definition);
            ValidateSceneReference(definition);
            Debug.Log("Sprint 025 quest definitions validation passed.");
        }

        [MenuItem("Project Genesis/Sprint 025/Validate Relevant Regression Suite")]
        public static void ValidateRelevantRegressionSuite()
        {
            ValidateQuestDefinitions();
            LocalMessageFeedDevelopmentValidator.ValidateRelevantRegressionSuite();
            Debug.Log("Sprint 025 relevant regression suite passed.");
        }

        private static QuestDefinition ValidateDefinitionAsset()
        {
            QuestDefinition definition =
                AssetDatabase.LoadAssetAtPath<QuestDefinition>(QuestPath);
            Require(definition != null, "Wolf trophies quest asset was not found.");
            Require(definition.TryValidate(out string error),
                $"Wolf trophies quest data is invalid: {error}");
            Require(definition.QuestId == "wolves-near-the-road",
                "Quest id changed unexpectedly.");
            Require(definition.DisplayName == "Волчьи трофеи",
                "Quest display name changed unexpectedly.");
            Require(definition.Objective.ObjectiveType == QuestObjectiveType.DefeatTarget,
                "Wolf trophies must use the defeat-target objective type.");
            Require(definition.Objective.TargetId == "wolf_tail" &&
                    definition.Objective.RequiredCount == 5,
                "Wolf trophies objective must require five wolf-tail progress events.");
            Require(definition.Reward.Experience == 80,
                "Wolf trophies reward must grant 80 experience.");
            return definition;
        }

        private static void ValidateDefinitionDrivenProgress(QuestDefinition definition)
        {
            GameObject playerProbe = new("QuestDefinitionPlayerValidation");
            GameObject restoreProbe = new("QuestDefinitionRestoreValidation");
            try
            {
                PlayerProgression progression = playerProbe.AddComponent<PlayerProgression>();
                QuestLog questLog = playerProbe.AddComponent<QuestLog>();
                Require(questLog.TryAcceptQuest(definition, "Староста деревни"),
                    "A valid quest definition could not be accepted.");

                QuestProgressData progress = questLog.GetQuestProgress(definition.QuestId);
                Require(progress != null && progress.Title == definition.DisplayName &&
                        progress.Description == definition.Description &&
                        progress.ObjectiveText == definition.Objective.DisplayText &&
                        progress.QuestGiverName == "Староста деревни" &&
                        progress.TargetId == definition.Objective.TargetId &&
                        progress.RequiredCount == definition.Objective.RequiredCount &&
                        progress.RewardExperience == definition.Reward.Experience,
                    "Accepted progress must copy all persistent fields from the definition.");

                for (int index = 0; index < definition.Objective.RequiredCount; index++)
                {
                    Require(questLog.ReportObjectiveProgress(definition.Objective.TargetId),
                        $"Definition-driven objective failed at step {index + 1}.");
                }

                Require(progress.State == QuestState.ReadyToTurnIn &&
                        progress.CurrentCount == definition.Objective.RequiredCount,
                    "Definition-driven objective did not reach the ready state.");
                List<QuestProgressData> savedProgress = questLog.CaptureState();

                Require(questLog.TryTurnInQuest(definition.QuestId),
                    "Definition-driven quest could not be turned in.");
                Require(progression.CurrentExperience == definition.Reward.Experience,
                    "Definition-driven quest did not grant its experience reward.");

                QuestLog restoredLog = restoreProbe.AddComponent<QuestLog>();
                restoredLog.RestoreState(savedProgress);
                QuestProgressData restored = restoredLog.GetQuestProgress(definition.QuestId);
                Require(restored != null && restored.State == QuestState.ReadyToTurnIn &&
                        restored.Title == definition.DisplayName &&
                        restored.CurrentCount == definition.Objective.RequiredCount &&
                        restored.RewardExperience == definition.Reward.Experience,
                    "Existing self-contained quest save data must restore without migration.");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(playerProbe);
                UnityEngine.Object.DestroyImmediate(restoreProbe);
            }
        }

        private static void ValidateSceneReference(QuestDefinition definition)
        {
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            InteractableNpc[] npcs =
                UnityEngine.Object.FindObjectsByType<InteractableNpc>(
                    FindObjectsInactive.Include,
                    FindObjectsSortMode.None);
            InteractableNpc elder = Array.Find(
                npcs,
                npc => npc.name == "NPC_VillageElder");
            Require(elder != null, "Starter scene village elder was not found.");

            Require(elder.QuestDefinition == definition,
                "Village elder must reference the wolf trophies quest asset.");
            Require(elder.QuestId == definition.QuestId &&
                    elder.QuestTitle == definition.DisplayName &&
                    elder.QuestObjectiveText == definition.Objective.DisplayText &&
                    elder.QuestReadyObjectiveText == definition.Objective.ReadyText &&
                    elder.RequiredObjectiveCount == definition.Objective.RequiredCount &&
                    elder.RewardExperience == definition.Reward.Experience,
                "NPC quest-facing properties must come from its referenced definition.");
        }

        private static void Require(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException(
                    $"Sprint 025 validation failed: {message}");
            }
        }
    }
}
