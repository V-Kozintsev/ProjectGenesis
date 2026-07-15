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
                acceptQuestButton.onClick.AddListener(AcceptCurrentQuest);
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
            canInteract = null;
        }

        private void AcceptCurrentQuest()
        {
            if (currentNpc == null || currentQuestLog == null || !CanStillInteract())
            {
                Hide();
                return;
            }

            currentQuestLog.TryAcceptQuest(currentNpc.QuestId);
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

            QuestState state = currentQuestLog != null ? currentQuestLog.GetQuestState(currentNpc.QuestId) : QuestState.NotStarted;

            if (speakerText != null)
            {
                speakerText.text = currentNpc.DisplayName;
            }

            if (bodyText != null)
            {
                bodyText.text = state == QuestState.NotStarted ? currentNpc.GreetingText : currentNpc.ActiveQuestText;
            }

            if (questText != null)
            {
                questText.text = $"{currentNpc.QuestTitle}\n{currentNpc.QuestObjectiveText}\nСостояние: {GetStateLabel(state)}";
            }

            if (acceptQuestButton != null)
            {
                acceptQuestButton.gameObject.SetActive(state == QuestState.NotStarted);
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
