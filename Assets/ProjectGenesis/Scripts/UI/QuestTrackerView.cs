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
        [SerializeField] private InteractableNpc questOwner;

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
            questOwner = npc;
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
            if (root == null || questLog == null || questOwner == null)
            {
                return;
            }

            QuestProgressData progress = questLog.GetQuestProgress(questOwner.QuestId);
            bool isTracked = progress != null &&
                (progress.State == QuestState.Active || progress.State == QuestState.ReadyToTurnIn);
            root.SetActive(isTracked);

            if (!isTracked)
            {
                return;
            }

            if (titleText != null)
            {
                titleText.text = questOwner.QuestTitle;
            }

            if (objectiveText != null)
            {
                objectiveText.text = progress.State == QuestState.ReadyToTurnIn
                    ? "Вернитесь к старосте"
                    : $"Волчьи хвосты: {progress.CurrentCount} / {progress.RequiredCount}";
            }
        }
    }
}
