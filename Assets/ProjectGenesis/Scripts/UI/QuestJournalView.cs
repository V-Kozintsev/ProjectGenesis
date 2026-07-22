using System.Collections.Generic;
using ProjectGenesis.Core;
using ProjectGenesis.Gameplay;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace ProjectGenesis.UI
{
    public sealed class QuestJournalView : MonoBehaviour
    {
        private enum JournalTab
        {
            Active,
            Completed
        }

        [SerializeField] private GameObject windowRoot;
        [SerializeField] private Button openButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button activeTabButton;
        [SerializeField] private Button completedTabButton;
        [SerializeField] private RectTransform questListRoot;
        [SerializeField] private Button questButtonTemplate;
        [SerializeField] private Text emptyText;
        [SerializeField] private Text titleText;
        [SerializeField] private Text stateText;
        [SerializeField] private Text descriptionText;
        [SerializeField] private Text objectiveText;
        [SerializeField] private Text giverText;
        [SerializeField] private Text rewardText;
        [SerializeField] private Button abandonButton;
        [SerializeField] private Text abandonButtonText;
        [SerializeField] private QuestLog questLog;
        [SerializeField] private DialogueWindow dialogueWindow;

        private readonly List<Button> spawnedQuestButtons = new();
        private JournalTab currentTab;
        private string selectedQuestId;
        private bool abandonConfirmationArmed;

        public void Initialize(
            GameObject journalWindow,
            Button journalOpenButton,
            Button journalCloseButton,
            Button activeTab,
            Button completedTab,
            RectTransform listRoot,
            Button listButtonTemplate,
            Text emptyLabel,
            Text questTitle,
            Text questState,
            Text questDescription,
            Text questObjective,
            Text questGiver,
            Text questReward,
            Button abandonQuestButton,
            Text abandonLabel,
            QuestLog playerQuestLog,
            DialogueWindow playerDialogueWindow)
        {
            windowRoot = journalWindow;
            openButton = journalOpenButton;
            closeButton = journalCloseButton;
            activeTabButton = activeTab;
            completedTabButton = completedTab;
            questListRoot = listRoot;
            questButtonTemplate = listButtonTemplate;
            emptyText = emptyLabel;
            titleText = questTitle;
            stateText = questState;
            descriptionText = questDescription;
            objectiveText = questObjective;
            giverText = questGiver;
            rewardText = questReward;
            abandonButton = abandonQuestButton;
            abandonButtonText = abandonLabel;
            questLog = playerQuestLog;
            dialogueWindow = playerDialogueWindow;
        }

        private void Awake()
        {
            openButton?.onClick.AddListener(ToggleWindow);
            closeButton?.onClick.AddListener(CloseWindow);
            activeTabButton?.onClick.AddListener(ShowActiveTab);
            completedTabButton?.onClick.AddListener(ShowCompletedTab);
            abandonButton?.onClick.AddListener(HandleAbandonQuest);

            if (questLog != null)
            {
                questLog.Changed += HandleQuestLogChanged;
            }

            if (questButtonTemplate != null)
            {
                questButtonTemplate.gameObject.SetActive(false);
            }

            currentTab = JournalTab.Active;
            CloseWindow();
            Refresh();
        }

        private void OnDestroy()
        {
            openButton?.onClick.RemoveListener(ToggleWindow);
            closeButton?.onClick.RemoveListener(CloseWindow);
            activeTabButton?.onClick.RemoveListener(ShowActiveTab);
            completedTabButton?.onClick.RemoveListener(ShowCompletedTab);
            abandonButton?.onClick.RemoveListener(HandleAbandonQuest);

            if (questLog != null)
            {
                questLog.Changed -= HandleQuestLogChanged;
            }
        }

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (!GameplayInputGate.IsTextEntryFocused && keyboard != null &&
                keyboard.jKey.wasPressedThisFrame)
            {
                ToggleWindow();
            }
        }

        private void ToggleWindow()
        {
            if (windowRoot == null)
            {
                return;
            }

            bool shouldOpen = !windowRoot.activeSelf;
            windowRoot.SetActive(shouldOpen);
            ResetAbandonConfirmation();

            if (shouldOpen)
            {
                dialogueWindow?.Hide();
                Refresh();
            }
        }

        private void CloseWindow()
        {
            if (windowRoot != null)
            {
                windowRoot.SetActive(false);
            }

            ResetAbandonConfirmation();
        }

        private void ShowActiveTab()
        {
            currentTab = JournalTab.Active;
            selectedQuestId = null;
            ResetAbandonConfirmation();
            Refresh();
        }

        private void ShowCompletedTab()
        {
            currentTab = JournalTab.Completed;
            selectedQuestId = null;
            ResetAbandonConfirmation();
            Refresh();
        }

        private void HandleQuestLogChanged(QuestLog _)
        {
            Refresh();
        }

        private void HandleAbandonQuest()
        {
            if (questLog == null || string.IsNullOrWhiteSpace(selectedQuestId))
            {
                return;
            }

            if (!abandonConfirmationArmed)
            {
                abandonConfirmationArmed = true;
                if (abandonButtonText != null)
                {
                    abandonButtonText.text = "Подтвердить отказ";
                }

                return;
            }

            string questId = selectedQuestId;
            selectedQuestId = null;
            ResetAbandonConfirmation();
            questLog.TryAbandonQuest(questId);
        }

        private void SelectQuest(string questId)
        {
            selectedQuestId = questId;
            ResetAbandonConfirmation();
            Refresh();
        }

        private void Refresh()
        {
            ClearQuestButtons();

            if (questLog == null)
            {
                RefreshDetails(null);
                return;
            }

            List<QuestProgressData> visibleQuests = new();
            foreach (QuestProgressData progress in questLog.GetAllQuestProgress())
            {
                if (MatchesCurrentTab(progress.State))
                {
                    visibleQuests.Add(progress);
                }
            }

            QuestProgressData selectedQuest = null;
            foreach (QuestProgressData progress in visibleQuests)
            {
                if (progress.QuestId == selectedQuestId)
                {
                    selectedQuest = progress;
                    break;
                }
            }

            if (selectedQuest == null && visibleQuests.Count > 0)
            {
                selectedQuest = visibleQuests[0];
                selectedQuestId = selectedQuest.QuestId;
            }

            for (int index = 0; index < visibleQuests.Count; index++)
            {
                CreateQuestButton(visibleQuests[index], index);
            }

            if (emptyText != null)
            {
                emptyText.gameObject.SetActive(visibleQuests.Count == 0);
                emptyText.text = currentTab == JournalTab.Active
                    ? "Нет активных заданий"
                    : "Нет завершённых заданий";
            }

            UpdateTabVisuals();
            RefreshDetails(selectedQuest);
        }

        private bool MatchesCurrentTab(QuestState state)
        {
            return currentTab == JournalTab.Active
                ? state == QuestState.Active || state == QuestState.ReadyToTurnIn
                : state == QuestState.Completed;
        }

        private void CreateQuestButton(QuestProgressData progress, int index)
        {
            if (questButtonTemplate == null || questListRoot == null)
            {
                return;
            }

            Button button = Instantiate(questButtonTemplate, questListRoot);
            button.name = $"Button_Quest_{progress.QuestId}";
            button.gameObject.SetActive(true);
            button.onClick.RemoveAllListeners();

            string questId = progress.QuestId;
            button.onClick.AddListener(() => SelectQuest(questId));

            RectTransform rect = button.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(0f, -index * 58f);
            rect.sizeDelta = new Vector2(262f, 50f);

            Text label = button.GetComponentInChildren<Text>();
            if (label != null)
            {
                string title = string.IsNullOrWhiteSpace(progress.Title) ? progress.QuestId : progress.Title;
                label.text = progress.State == QuestState.ReadyToTurnIn ? $"{title}\nМожно сдать" : title;
                label.fontSize = progress.State == QuestState.ReadyToTurnIn ? 16 : 18;
            }

            Image image = button.GetComponent<Image>();
            if (image != null)
            {
                image.color = progress.QuestId == selectedQuestId
                    ? new Color(0.22f, 0.38f, 0.48f, 1f)
                    : new Color(0.12f, 0.18f, 0.22f, 1f);
            }

            spawnedQuestButtons.Add(button);
        }

        private void ClearQuestButtons()
        {
            foreach (Button button in spawnedQuestButtons)
            {
                if (button != null)
                {
                    button.gameObject.SetActive(false);
                    Destroy(button.gameObject);
                }
            }

            spawnedQuestButtons.Clear();
        }

        private void RefreshDetails(QuestProgressData progress)
        {
            bool hasQuest = progress != null;
            string title = hasQuest && !string.IsNullOrWhiteSpace(progress.Title)
                ? progress.Title
                : hasQuest ? progress.QuestId : "Выберите задание";

            SetText(titleText, title);
            SetText(stateText, hasQuest ? GetStateLabel(progress.State) : string.Empty);
            SetText(descriptionText, hasQuest ? progress.Description : string.Empty);
            SetText(
                objectiveText,
                hasQuest
                    ? $"{(string.IsNullOrWhiteSpace(progress.ObjectiveText) ? "Прогресс" : progress.ObjectiveText)}: {progress.CurrentCount} / {progress.RequiredCount}"
                    : string.Empty);
            SetText(
                giverText,
                hasQuest && !string.IsNullOrWhiteSpace(progress.QuestGiverName)
                    ? $"Выдаёт: {progress.QuestGiverName}"
                    : string.Empty);
            SetText(rewardText, hasQuest ? $"Награда: {progress.RewardExperience} опыта" : string.Empty);

            if (abandonButton != null)
            {
                abandonButton.gameObject.SetActive(
                    hasQuest &&
                    currentTab == JournalTab.Active &&
                    progress.State != QuestState.Completed);
            }
        }

        private void UpdateTabVisuals()
        {
            SetTabColor(activeTabButton, currentTab == JournalTab.Active);
            SetTabColor(completedTabButton, currentTab == JournalTab.Completed);
        }

        private void ResetAbandonConfirmation()
        {
            abandonConfirmationArmed = false;
            if (abandonButtonText != null)
            {
                abandonButtonText.text = "Отказаться";
            }
        }

        private static void SetTabColor(Button button, bool selected)
        {
            if (button != null && button.TryGetComponent(out Image image))
            {
                image.color = selected
                    ? new Color(0.22f, 0.38f, 0.48f, 1f)
                    : new Color(0.1f, 0.15f, 0.18f, 1f);
            }
        }

        private static void SetText(Text text, string value)
        {
            if (text != null)
            {
                text.text = value;
            }
        }

        private static string GetStateLabel(QuestState state)
        {
            return state switch
            {
                QuestState.Active => "Выполняется",
                QuestState.ReadyToTurnIn => "Можно сдать",
                QuestState.Completed => "Завершено",
                _ => string.Empty
            };
        }
    }
}
