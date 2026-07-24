using ProjectGenesis.Data;
using ProjectGenesis.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectGenesis.UI
{
    public sealed class NpcInteractionView : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private Text titleText;
        [SerializeField] private Text bodyText;
        [SerializeField] private Button questButton;
        [SerializeField] private Button tradeButton;
        [SerializeField] private Button talkButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button headerCloseButton;

        private InteractableNpc currentNpc;
        private GameObject currentPlayer;
        private QuestLog currentQuestLog;
        private DialogueWindow dialogueWindow;
        private MerchantShopView merchantShopView;
        private System.Func<InteractableNpc, bool> canInteract;

        public bool IsVisible => root != null && root.activeSelf;
        public InteractableNpc CurrentNpc => currentNpc;
        public Button QuestButton => questButton;
        public Button TradeButton => tradeButton;

        public void Initialize(
            GameObject windowRoot,
            Text title,
            Text body,
            Button quest,
            Button trade,
            Button talk,
            Button close,
            Button headerClose)
        {
            root = windowRoot;
            titleText = title;
            bodyText = body;
            questButton = quest;
            tradeButton = trade;
            talkButton = talk;
            closeButton = close;
            headerCloseButton = headerClose;
        }

        private void Awake()
        {
            questButton?.onClick.AddListener(OpenQuestDialogue);
            tradeButton?.onClick.AddListener(OpenTrade);
            talkButton?.onClick.AddListener(ShowSmallTalk);
            closeButton?.onClick.AddListener(Hide);
            headerCloseButton?.onClick.AddListener(Hide);
            Hide();
        }

        private void OnDestroy()
        {
            questButton?.onClick.RemoveListener(OpenQuestDialogue);
            tradeButton?.onClick.RemoveListener(OpenTrade);
            talkButton?.onClick.RemoveListener(ShowSmallTalk);
            closeButton?.onClick.RemoveListener(Hide);
            headerCloseButton?.onClick.RemoveListener(Hide);
        }

        public void Show(
            InteractableNpc npc,
            GameObject player,
            QuestLog questLog,
            DialogueWindow questDialogueWindow,
            MerchantShopView shopView,
            System.Func<InteractableNpc, bool> canInteractWithNpc)
        {
            currentNpc = npc;
            currentPlayer = player;
            currentQuestLog = questLog;
            dialogueWindow = questDialogueWindow;
            merchantShopView = shopView;
            canInteract = canInteractWithNpc;

            Refresh();

            if (root != null)
            {
                root.SetActive(true);
                root.transform.SetAsLastSibling();
            }
        }

        public void Hide()
        {
            if (root != null)
            {
                root.SetActive(false);
            }

            currentNpc = null;
            currentPlayer = null;
            currentQuestLog = null;
            dialogueWindow = null;
            merchantShopView = null;
            canInteract = null;
        }

        private void OpenQuestDialogue()
        {
            if (!CanStillInteract())
            {
                Hide();
                return;
            }

            dialogueWindow?.Show(currentNpc, currentQuestLog, canInteract);
            Hide();
        }

        private void OpenTrade()
        {
            if (!CanStillInteract())
            {
                Hide();
                return;
            }

            MerchantShop shop = currentNpc != null
                ? currentNpc.GetComponent<MerchantShop>()
                : null;
            shop?.OpenFor(currentPlayer, merchantShopView);
            Hide();
        }

        private void ShowSmallTalk()
        {
            if (bodyText != null)
            {
                bodyText.text = "Пока здесь нет отдельной истории. Позже этот раздел станет местом для слухов, услуг и знаний о мире.";
            }
        }

        private bool CanStillInteract()
        {
            return currentNpc != null && (canInteract == null || canInteract(currentNpc));
        }

        private void Refresh()
        {
            if (currentNpc == null)
            {
                return;
            }

            if (titleText != null)
            {
                titleText.text = currentNpc.DisplayName;
            }

            QuestDefinition quest = currentNpc.ResolveQuestDefinition(currentQuestLog);
            MerchantShop shop = currentNpc.GetComponent<MerchantShop>();

            if (bodyText != null)
            {
                bodyText.text = BuildGreeting(quest, shop);
            }

            if (questButton != null)
            {
                bool hasQuest = quest != null;
                questButton.gameObject.SetActive(hasQuest);
                if (hasQuest)
                {
                    Text label = questButton.GetComponentInChildren<Text>();
                    if (label != null)
                    {
                        label.text = BuildQuestButtonLabel(quest);
                    }
                }
            }

            if (tradeButton != null)
            {
                tradeButton.gameObject.SetActive(shop != null);
            }

            if (talkButton != null)
            {
                talkButton.gameObject.SetActive(true);
            }
        }

        private string BuildGreeting(QuestDefinition quest, MerchantShop shop)
        {
            if (shop != null)
            {
                return "Добро пожаловать. Могу показать товары или немного поговорить.";
            }

            if (quest != null)
            {
                QuestState state = currentQuestLog != null
                    ? currentQuestLog.GetQuestState(quest.QuestId)
                    : QuestState.NotStarted;
                return state switch
                {
                    QuestState.NotStarted => "Есть поручение, если готов помочь деревне.",
                    QuestState.Active => "Ты уже взял поручение. Можешь посмотреть детали.",
                    QuestState.ReadyToTurnIn => "Похоже, поручение выполнено. Давай обсудим награду.",
                    QuestState.Completed => "Спасибо за помощь. Сейчас новых поручений нет.",
                    _ => "Можем поговорить."
                };
            }

            return "Что вы хотели?";
        }

        private string BuildQuestButtonLabel(QuestDefinition quest)
        {
            QuestState state = currentQuestLog != null
                ? currentQuestLog.GetQuestState(quest.QuestId)
                : QuestState.NotStarted;
            return state == QuestState.ReadyToTurnIn
                ? "Завершить задание"
                : "Задание";
        }
    }
}
