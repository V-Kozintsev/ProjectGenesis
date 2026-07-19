using System;
using ProjectGenesis.Gameplay;
using UnityEditor;
using UnityEngine;

namespace ProjectGenesis.Tools.Editor
{
    public static class QuestSystemDevelopmentValidator
    {
        private const string QuestId = "validation-quest";
        private const string TargetId = "validation-target";

        [MenuItem("Project Genesis/Sprint 008/Validate Quest State Rules")]
        public static void ValidateQuestStateRules()
        {
            GameObject testObject = new("QuestStateValidation");

            try
            {
                testObject.AddComponent<PlayerProgression>();
                QuestLog questLog = testObject.AddComponent<QuestLog>();

                Require(!questLog.ReportObjectiveProgress(TargetId), "Progress advanced before quest acceptance.");
                Require(AcceptQuest(questLog), "Quest could not be accepted.");
                Require(questLog.GetQuestState(QuestId) == QuestState.Active, "Accepted quest is not active.");

                Require(questLog.ReportObjectiveProgress(TargetId), "Active objective did not advance.");
                Require(questLog.GetQuestProgress(QuestId).CurrentCount == 1, "Objective did not reach 1 / 5.");
                Require(questLog.TryAbandonQuest(QuestId), "Active quest could not be abandoned.");
                Require(questLog.GetQuestState(QuestId) == QuestState.NotStarted, "Abandoned quest did not reset.");

                Require(AcceptQuest(questLog), "Abandoned quest could not be accepted again.");
                for (int index = 0; index < 5; index++)
                {
                    Require(questLog.ReportObjectiveProgress(TargetId), $"Objective failed at step {index + 1}.");
                }

                QuestProgressData readyProgress = questLog.GetQuestProgress(QuestId);
                Require(readyProgress.CurrentCount == 5, "Objective did not stop at 5 / 5.");
                Require(readyProgress.State == QuestState.ReadyToTurnIn, "Quest is not ready to turn in at 5 / 5.");
                Require(!questLog.HasActiveObjective(TargetId), "Ready objective is still considered active.");
                Require(!questLog.ReportObjectiveProgress(TargetId), "Ready objective advanced beyond its cap.");

                Require(questLog.TryTurnInQuest(QuestId), "Ready quest could not be turned in.");
                Require(questLog.GetQuestState(QuestId) == QuestState.Completed, "Turned-in quest is not completed.");
                Require(!questLog.TryAbandonQuest(QuestId), "Completed quest could be abandoned.");
                Require(!questLog.ReportObjectiveProgress(TargetId), "Completed objective advanced.");

                QuestProgressData saved = questLog.CaptureState()[0];
                Require(saved.Title == "Проверочное задание", "Quest title was not preserved.");
                Require(saved.ObjectiveText == "Проверочные предметы", "Objective label was not preserved.");

                Debug.Log("Sprint 008 quest state validation passed.");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(testObject);
            }
        }

        private static bool AcceptQuest(QuestLog questLog)
        {
            return questLog.TryAcceptQuest(
                QuestId,
                "Проверочное задание",
                "Проверяет основные переходы состояния.",
                "Проверочные предметы",
                "Проверочный NPC",
                TargetId,
                5,
                80);
        }

        private static void Require(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException($"Sprint 008 validation failed: {message}");
            }
        }
    }
}
