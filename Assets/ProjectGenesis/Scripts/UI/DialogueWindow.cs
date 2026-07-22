using ProjectGenesis.Data;
using ProjectGenesis.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectGenesis.UI
{
    public sealed class DialogueWindow : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private Text speakerText;
        [SerializeField] private Text bodyText;
        [SerializeField] private Text questText;
        [SerializeField] private Button acceptQuestButton;
        [SerializeField] private Button closeButton;

        private InteractableNpc currentNpc;
        private QuestLog currentQuestLog;
        private QuestDefinition currentQuestDefinition;
        private System.Func<InteractableNpc, bool> canInteract;

        public void Initialize(
            GameObject windowRoot,
            Text speaker,
            Text body,
            Text quest,
            Button acceptButton,
            Button close)
        {
            root = windowRoot;
            speakerText = speaker;
            bodyText = body;
            questText = quest;
            acceptQuestButton = acceptButton;
            closeButton = close;
        }

        private void Awake()
        {
            if (acceptQuestButton != null)
            {
                acceptQuestButton.onClick.AddListener(HandleQuestAction);
            }

            if (closeButton != null)
            {
                closeButton.onClick.AddListener(Hide);
            }

            Hide();
        }

        public bool IsVisible => root != null && root.activeSelf;
        public InteractableNpc CurrentNpc => currentNpc;

        public void Show(InteractableNpc npc, QuestLog questLog, System.Func<InteractableNpc, bool> canInteractWithNpc = null)
        {
            currentNpc = npc;
            currentQuestLog = questLog;
            currentQuestDefinition = npc != null ? npc.ResolveQuestDefinition(questLog) : null;
            canInteract = canInteractWithNpc;

            Refresh();

            if (root != null)
            {
                root.SetActive(true);
            }
        }

        public void Hide()
        {
            if (root != null)
            {
                root.SetActive(false);
            }

            currentNpc = null;
            currentQuestLog = null;
            currentQuestDefinition = null;
            canInteract = null;
        }

        private void HandleQuestAction()
        {
            if (currentNpc == null || currentQuestLog == null ||
                currentQuestDefinition == null || !CanStillInteract())
            {
                Hide();
                return;
            }

            QuestState state = currentQuestLog.GetQuestState(currentQuestDefinition.QuestId);
            if (state == QuestState.NotStarted)
            {
                currentQuestLog.TryAcceptQuest(
                    currentQuestDefinition,
                    currentNpc.DisplayName);
            }
            else if (state == QuestState.ReadyToTurnIn)
            {
                currentQuestLog.TryTurnInQuest(currentQuestDefinition.QuestId);
            }

            Refresh();
        }

        private bool CanStillInteract()
        {
            return canInteract == null || canInteract(currentNpc);
        }

        private void Refresh()
        {
            if (currentNpc == null)
            {
                return;
            }

            if (currentQuestDefinition == null)
            {
                if (bodyText != null)
                {
                    bodyText.text = string.Empty;
                }

                if (questText != null)
                {
                    questText.text = string.Empty;
                }

                if (acceptQuestButton != null)
                {
                    acceptQuestButton.gameObject.SetActive(false);
                }

                return;
            }

            QuestState state = currentQuestLog != null
                ? currentQuestLog.GetQuestState(currentQuestDefinition.QuestId)
                : QuestState.NotStarted;
            QuestProgressData progress = currentQuestLog != null
                ? currentQuestLog.GetQuestProgress(currentQuestDefinition.QuestId)
                : null;

            if (speakerText != null)
            {
                speakerText.text = currentNpc.DisplayName;
            }

            if (bodyText != null)
            {
                bodyText.text = state switch
                {
                    QuestState.NotStarted => currentQuestDefinition.OfferText,
                    QuestState.Active => currentQuestDefinition.ActiveText,
                    QuestState.ReadyToTurnIn => currentQuestDefinition.ReadyText,
                    QuestState.Completed => currentQuestDefinition.CompletedText,
                    _ => currentQuestDefinition.OfferText
                };
            }

            if (questText != null)
            {
                int currentCount = progress != null ? progress.CurrentCount : 0;
                int requiredCount = progress != null ? progress.RequiredCount : currentQuestDefinition.Objective.RequiredCount;
                questText.text =
                    $"{currentQuestDefinition.DisplayName}\n" +
                    $"{currentQuestDefinition.Objective.DisplayText} ({currentCount} / {requiredCount})\n" +
                    $"Награда: {currentQuestDefinition.Reward.Experience} опыта\n" +
                    $"Состояние: {GetStateLabel(state)}";
            }

            if (acceptQuestButton != null)
            {
                bool showAction = state == QuestState.NotStarted || state == QuestState.ReadyToTurnIn;
                acceptQuestButton.gameObject.SetActive(showAction);

                if (showAction)
                {
                    Text label = acceptQuestButton.GetComponentInChildren<Text>();
                    if (label != null)
                    {
                        label.text = state == QuestState.NotStarted ? "Принять поручение" : "Завершить поручение";
                    }
                }
            }
        }

        private static string GetStateLabel(QuestState state)
        {
            return state switch
            {
                QuestState.NotStarted => "не взят",
                QuestState.Active => "активен",
                QuestState.ReadyToTurnIn => "готов к сдаче",
                QuestState.Completed => "завершён",
                _ => state.ToString()
            };
        }
    }
}
