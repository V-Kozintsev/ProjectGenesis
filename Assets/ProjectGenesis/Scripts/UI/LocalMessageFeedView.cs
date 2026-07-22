using System.Text;
using ProjectGenesis.Core;
using ProjectGenesis.Gameplay;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace ProjectGenesis.UI
{
    [DisallowMultipleComponent]
    public sealed class LocalMessageFeedView : MonoBehaviour
    {
        [SerializeField] private GameObject windowRoot;
        [SerializeField] private Button reopenButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button allButton;
        [SerializeField] private Button systemButton;
        [SerializeField] private Button lootButton;
        [SerializeField] private Button combatButton;
        [SerializeField] private Button chatButton;
        [SerializeField] private Button announcementButton;
        [SerializeField] private Text messagesText;
        [SerializeField] private ScrollRect messageScroll;
        [SerializeField] private InputField chatInput;
        [SerializeField] private Button sendButton;
        [SerializeField] private LocalMessageStream messageStream;

        private readonly StringBuilder builder = new();
        private LocalMessageFilter activeFilter = LocalMessageFilter.All;

        public GameObject WindowRoot => windowRoot;
        public Button ReopenButton => reopenButton;
        public Text MessagesText => messagesText;
        public ScrollRect MessageScroll => messageScroll;
        public InputField ChatInput => chatInput;
        public Button SendButton => sendButton;
        public LocalMessageStream MessageStream => messageStream;
        public LocalMessageFilter ActiveFilter => activeFilter;

        public void Initialize(
            GameObject root,
            Button openButton,
            Button hideButton,
            Button showAllButton,
            Button showSystemButton,
            Button showLootButton,
            Button showCombatButton,
            Button showChatButton,
            Button showAnnouncementButton,
            Text messageList,
            ScrollRect scroll,
            InputField localChatInput,
            Button localChatSendButton,
            LocalMessageStream stream)
        {
            windowRoot = root;
            reopenButton = openButton;
            closeButton = hideButton;
            allButton = showAllButton;
            systemButton = showSystemButton;
            lootButton = showLootButton;
            combatButton = showCombatButton;
            chatButton = showChatButton;
            announcementButton = showAnnouncementButton;
            messagesText = messageList;
            messageScroll = scroll;
            chatInput = localChatInput;
            sendButton = localChatSendButton;
            messageStream = stream;
        }

        private void Awake()
        {
            closeButton?.onClick.AddListener(Hide);
            reopenButton?.onClick.AddListener(Show);
            allButton?.onClick.AddListener(() => SetFilter(LocalMessageFilter.All));
            systemButton?.onClick.AddListener(() => SetFilter(LocalMessageFilter.System));
            lootButton?.onClick.AddListener(() => SetFilter(LocalMessageFilter.Loot));
            combatButton?.onClick.AddListener(() => SetFilter(LocalMessageFilter.Combat));
            chatButton?.onClick.AddListener(
                () => SetFilter(LocalMessageFilter.LocalChat));
            announcementButton?.onClick.AddListener(
                () => SetFilter(LocalMessageFilter.Announcement));
            sendButton?.onClick.AddListener(SendLocalChatMessage);
            chatInput?.onEndEdit.AddListener(HandleChatEndEdit);
        }

        private void OnEnable()
        {
            Subscribe();
            Show();
            Refresh();
        }

        private void OnDisable()
        {
            Unsubscribe();
            windowRoot?.SetActive(false);
            reopenButton?.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            Unsubscribe();
            closeButton?.onClick.RemoveListener(Hide);
            reopenButton?.onClick.RemoveListener(Show);
            allButton?.onClick.RemoveAllListeners();
            systemButton?.onClick.RemoveAllListeners();
            lootButton?.onClick.RemoveAllListeners();
            combatButton?.onClick.RemoveAllListeners();
            chatButton?.onClick.RemoveAllListeners();
            announcementButton?.onClick.RemoveAllListeners();
            sendButton?.onClick.RemoveListener(SendLocalChatMessage);
            chatInput?.onEndEdit.RemoveListener(HandleChatEndEdit);
        }

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard != null && keyboard.lKey.wasPressedThisFrame &&
                !GameplayInputGate.IsTextEntryFocused)
            {
                SetVisible(windowRoot == null || !windowRoot.activeSelf);
            }
        }

        public void SetFilter(LocalMessageFilter filter)
        {
            activeFilter = filter == LocalMessageFilter.None
                ? LocalMessageFilter.All
                : filter;
            Refresh();
        }

        public void Show()
        {
            SetVisible(true);
        }

        public void Hide()
        {
            SetVisible(false);
        }

        public void Refresh()
        {
            if (messagesText == null)
            {
                return;
            }

            builder.Clear();
            if (messageStream != null)
            {
                for (int index = messageStream.Entries.Count - 1;
                     index >= 0;
                     index--)
                {
                    LocalMessageEntry entry = messageStream.Entries[index];
                    if (!LocalMessageStream.Matches(entry.Category, activeFilter))
                    {
                        continue;
                    }

                    if (builder.Length > 0)
                    {
                        builder.Insert(0, '\n');
                    }

                    builder.Insert(0, FormatEntry(entry));
                }
            }

            messagesText.text = builder.Length > 0
                ? builder.ToString()
                : "Событий пока нет.";
            RefreshButtonColors();
            ScrollToLatest();
        }

        private void SetVisible(bool visible)
        {
            windowRoot?.SetActive(visible);
            reopenButton?.gameObject.SetActive(!visible);
            if (visible)
            {
                windowRoot?.transform.SetAsLastSibling();
                Refresh();
            }
        }

        private void Subscribe()
        {
            if (messageStream == null)
            {
                return;
            }

            messageStream.MessagePublished -= HandleMessageChanged;
            messageStream.Cleared -= HandleMessagesCleared;
            messageStream.MessagePublished += HandleMessageChanged;
            messageStream.Cleared += HandleMessagesCleared;
        }

        private void Unsubscribe()
        {
            if (messageStream == null)
            {
                return;
            }

            messageStream.MessagePublished -= HandleMessageChanged;
            messageStream.Cleared -= HandleMessagesCleared;
        }

        private void HandleMessageChanged(LocalMessageEntry _)
        {
            Refresh();
        }

        private void HandleMessagesCleared()
        {
            Refresh();
        }

        private void RefreshButtonColors()
        {
            SetButtonSelected(allButton, activeFilter == LocalMessageFilter.All);
            SetButtonSelected(systemButton, activeFilter == LocalMessageFilter.System);
            SetButtonSelected(lootButton, activeFilter == LocalMessageFilter.Loot);
            SetButtonSelected(combatButton, activeFilter == LocalMessageFilter.Combat);
            SetButtonSelected(chatButton, activeFilter == LocalMessageFilter.LocalChat);
            SetButtonSelected(
                announcementButton,
                activeFilter == LocalMessageFilter.Announcement);
        }

        private void HandleChatEndEdit(string _)
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard != null &&
                (keyboard.enterKey.wasPressedThisFrame ||
                 keyboard.numpadEnterKey.wasPressedThisFrame))
            {
                SendLocalChatMessage();
            }
        }

        private void SendLocalChatMessage()
        {
            string text = chatInput != null ? chatInput.text?.Trim() : string.Empty;
            if (string.IsNullOrEmpty(text) || messageStream == null)
            {
                return;
            }

            PlayerIdentity identity = messageStream.GetComponent<PlayerIdentity>();
            string author = identity != null ? identity.CharacterName : "Игрок";
            messageStream.Publish(LocalMessageCategory.LocalChat, $"{author}: {text}");
            chatInput.text = string.Empty;
            chatInput.ActivateInputField();
        }

        private void ScrollToLatest()
        {
            if (messageScroll == null || messagesText == null)
            {
                return;
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(messagesText.rectTransform);
            Canvas.ForceUpdateCanvases();
            messageScroll.verticalNormalizedPosition = 0f;
        }

        private static void SetButtonSelected(Button button, bool selected)
        {
            if (button != null && button.targetGraphic is Image image)
            {
                image.color = selected
                    ? new Color(0.22f, 0.45f, 0.56f, 1f)
                    : new Color(0.16f, 0.25f, 0.34f, 1f);
            }
        }

        private static string FormatEntry(LocalMessageEntry entry)
        {
            string label = entry.Category switch
            {
                LocalMessageCategory.System => "Система",
                LocalMessageCategory.Loot => "Лут",
                LocalMessageCategory.Combat => "Бой",
                LocalMessageCategory.LocalChat => "Общий",
                LocalMessageCategory.Announcement => "Объявление",
                _ => "Событие"
            };
            string color = entry.Category switch
            {
                LocalMessageCategory.System => "#B9C7D0",
                LocalMessageCategory.Loot => "#F0C35A",
                LocalMessageCategory.Combat => "#EE8C72",
                LocalMessageCategory.LocalChat => "#E5E7E8",
                LocalMessageCategory.Announcement => "#7ED7E8",
                _ => "#E5E7E8"
            };

            return $"<color={color}>[{label}]</color> {entry.Text}";
        }

    }
}
