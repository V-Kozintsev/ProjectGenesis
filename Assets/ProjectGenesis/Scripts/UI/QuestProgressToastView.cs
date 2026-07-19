using System.Collections;
using ProjectGenesis.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectGenesis.UI
{
    public sealed class QuestProgressToastView : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private Text messageText;
        [SerializeField] private QuestLog questLog;
        [SerializeField, Min(0.5f)] private float visibleDuration = 2.6f;

        private Coroutine hideRoutine;

        public void Initialize(GameObject toastRoot, Text message, QuestLog playerQuestLog)
        {
            root = toastRoot;
            messageText = message;
            questLog = playerQuestLog;
        }

        private void Awake()
        {
            if (questLog != null)
            {
                questLog.ObjectiveProgressed += HandleObjectiveProgressed;
            }

            if (root != null)
            {
                root.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            if (questLog != null)
            {
                questLog.ObjectiveProgressed -= HandleObjectiveProgressed;
            }
        }

        private void HandleObjectiveProgressed(QuestProgressData progress)
        {
            if (root == null || progress == null)
            {
                return;
            }

            if (messageText != null)
            {
                string objective = string.IsNullOrWhiteSpace(progress.ObjectiveText)
                    ? "Прогресс"
                    : progress.ObjectiveText;
                messageText.text =
                    $"{progress.Title}\n" +
                    $"{objective}: {progress.CurrentCount} / {progress.RequiredCount}";
            }

            root.SetActive(true);
            if (hideRoutine != null)
            {
                StopCoroutine(hideRoutine);
            }

            hideRoutine = StartCoroutine(HideAfterDelay());
        }

        private IEnumerator HideAfterDelay()
        {
            yield return new WaitForSecondsRealtime(visibleDuration);
            hideRoutine = null;

            if (root != null)
            {
                root.SetActive(false);
            }
        }
    }
}
