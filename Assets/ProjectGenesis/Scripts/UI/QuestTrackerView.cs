using System.Collections.Generic;
using System.Text;
using ProjectGenesis.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectGenesis.UI
{
    public sealed class QuestTrackerView : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private Text titleText;
        [SerializeField] private Text objectiveText;
        [SerializeField] private QuestLog questLog;

        public void Initialize(
            GameObject trackerRoot,
            Text title,
            Text objective,
            QuestLog playerQuestLog,
            InteractableNpc npc)
        {
            root = trackerRoot;
            titleText = title;
            objectiveText = objective;
            questLog = playerQuestLog;
            Refresh();
        }

        public void RefreshNow()
        {
            Refresh();
        }

        private void Awake()
        {
            if (questLog != null)
            {
                questLog.Changed += HandleQuestChanged;
            }

            Refresh();
        }

        private void OnDestroy()
        {
            if (questLog != null)
            {
                questLog.Changed -= HandleQuestChanged;
            }
        }

        private void HandleQuestChanged(QuestLog _)
        {
            Refresh();
        }

        private void Refresh()
        {
            if (root == null || questLog == null)
            {
                return;
            }

            List<QuestProgressData> trackedQuests = new();
            foreach (QuestProgressData progress in questLog.GetAllQuestProgress())
            {
                if (progress.State == QuestState.Active ||
                    progress.State == QuestState.ReadyToTurnIn)
                {
                    trackedQuests.Add(progress);
                }
            }

            root.SetActive(trackedQuests.Count > 0);
            if (trackedQuests.Count == 0)
            {
                return;
            }

            if (trackedQuests.Count == 1)
            {
                QuestProgressData progress = trackedQuests[0];
                if (titleText != null)
                {
                    titleText.text = GetQuestTitle(progress);
                }

                if (objectiveText != null)
                {
                    objectiveText.fontSize = 19;
                    objectiveText.text = GetSingleObjective(progress);
                }

                return;
            }

            if (titleText != null)
            {
                titleText.text = $"Активные задания ({trackedQuests.Count})";
            }

            if (objectiveText != null)
            {
                objectiveText.fontSize = 16;
                StringBuilder lines = new();
                int visibleCount = Mathf.Min(3, trackedQuests.Count);
                for (int index = 0; index < visibleCount; index++)
                {
                    if (index > 0)
                    {
                        lines.Append('\n');
                    }

                    QuestProgressData progress = trackedQuests[index];
                    lines.Append(GetQuestTitle(progress));
                    lines.Append(": ");
                    lines.Append(progress.State == QuestState.ReadyToTurnIn
                        ? "готово к сдаче"
                        : $"{progress.CurrentCount} / {progress.RequiredCount}");
                }

                if (trackedQuests.Count > visibleCount)
                {
                    lines.Append($"\n+ ещё {trackedQuests.Count - visibleCount}");
                }

                objectiveText.text = lines.ToString();
            }
        }

        private static string GetQuestTitle(QuestProgressData progress)
        {
            return string.IsNullOrWhiteSpace(progress.Title) ? "Задание" : progress.Title;
        }

        private static string GetSingleObjective(QuestProgressData progress)
        {
            if (progress.State == QuestState.ReadyToTurnIn)
            {
                return string.IsNullOrWhiteSpace(progress.QuestGiverName)
                    ? "Вернитесь к заказчику"
                    : $"Вернитесь: {progress.QuestGiverName}";
            }

            string objective = string.IsNullOrWhiteSpace(progress.ObjectiveText)
                ? "Цель"
                : progress.ObjectiveText;
            return $"{objective}: {progress.CurrentCount} / {progress.RequiredCount}";
        }
    }
}
