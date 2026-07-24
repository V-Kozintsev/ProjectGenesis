using System;
using ProjectGenesis.Data;
using ProjectGenesis.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectGenesis.UI
{
    public sealed class MerchantShopView : MonoBehaviour
    {
        [SerializeField] private GameObject windowRoot;
        [SerializeField] private Button closeButton;
        [SerializeField] private Text titleText;
        [SerializeField] private Text goldText;
        [SerializeField] private Text feedbackText;
        [SerializeField] private Button[] buyButtons = Array.Empty<Button>();
        [SerializeField] private Text[] buyButtonTexts = Array.Empty<Text>();
        [SerializeField] private Button[] sellButtons = Array.Empty<Button>();
        [SerializeField] private Text[] sellButtonTexts = Array.Empty<Text>();

        private ItemDefinition[] currentStock = Array.Empty<ItemDefinition>();
        private PlayerWallet wallet;
        private PlayerInventory inventory;
        private LocalMessageStream messageStream;
        private int pendingBuyIndex = -1;
        private int pendingSellSlotIndex = -1;

        public bool IsOpen => windowRoot != null && windowRoot.activeSelf;
        public PlayerWallet Wallet => wallet;
        public PlayerInventory Inventory => inventory;

        public void Initialize(
            GameObject shopWindow,
            Button shopCloseButton,
            Text shopTitleText,
            Text playerGoldText,
            Text actionFeedbackText,
            Button[] stockButtons,
            Text[] stockButtonTexts,
            Button[] inventorySellButtons,
            Text[] inventorySellButtonTexts)
        {
            windowRoot = shopWindow;
            closeButton = shopCloseButton;
            titleText = shopTitleText;
            goldText = playerGoldText;
            feedbackText = actionFeedbackText;
            buyButtons = stockButtons ?? Array.Empty<Button>();
            buyButtonTexts = stockButtonTexts ?? Array.Empty<Text>();
            sellButtons = inventorySellButtons ?? Array.Empty<Button>();
            sellButtonTexts = inventorySellButtonTexts ?? Array.Empty<Text>();
        }

        private void Awake()
        {
            closeButton?.onClick.AddListener(Close);

            for (int index = 0; index < buyButtons.Length; index++)
            {
                int stockIndex = index;
                buyButtons[index]?.onClick.AddListener(() => TryBuyStockIndex(stockIndex));
            }

            for (int index = 0; index < sellButtons.Length; index++)
            {
                int slotIndex = index;
                sellButtons[index]?.onClick.AddListener(() => TrySellSlotIndex(slotIndex));
            }

            Close();
        }

        private void OnDestroy()
        {
            closeButton?.onClick.RemoveListener(Close);

            if (buyButtons != null)
            {
                foreach (Button button in buyButtons)
                {
                    button?.onClick.RemoveAllListeners();
                }
            }

            if (sellButtons != null)
            {
                foreach (Button button in sellButtons)
                {
                    button?.onClick.RemoveAllListeners();
                }
            }

            Unsubscribe();
        }

        public void Open(
            string shopName,
            ItemDefinition[] stock,
            PlayerWallet playerWallet,
            PlayerInventory playerInventory,
            LocalMessageStream localMessages)
        {
            Unsubscribe();
            currentStock = stock ?? Array.Empty<ItemDefinition>();
            wallet = playerWallet;
            inventory = playerInventory;
            messageStream = localMessages;

            if (wallet != null)
            {
                wallet.Changed += HandleWalletChanged;
            }

            if (inventory != null)
            {
                inventory.Changed += HandleInventoryChanged;
            }

            if (titleText != null)
            {
                titleText.text = string.IsNullOrWhiteSpace(shopName) ? "Торговец" : shopName;
            }

            if (windowRoot != null)
            {
                windowRoot.SetActive(true);
                windowRoot.transform.SetAsLastSibling();
            }

            SetFeedback("Выберите предмет для покупки или продажи.");
            ClearPendingConfirmation();
            Refresh();
        }

        public void Close()
        {
            if (windowRoot != null)
            {
                windowRoot.SetActive(false);
            }

            ClearPendingConfirmation();
        }

        public bool TryBuy(ItemDefinition item)
        {
            if (item == null)
            {
                SetFeedback("У торговца нет выбранного предмета.");
                return false;
            }

            if (wallet == null || inventory == null)
            {
                SetFeedback("Магазин не подключен к персонажу.");
                return false;
            }

            if (!inventory.HasFreeSlot)
            {
                SetFeedback("В рюкзаке нет свободной ячейки.");
                return false;
            }

            if (!wallet.CanAfford(item.BuyPrice))
            {
                SetFeedback($"Недостаточно золота: нужно {item.BuyPrice}.");
                return false;
            }

            if (!wallet.TrySpend(item.BuyPrice) || !inventory.TryAdd(item))
            {
                SetFeedback("Покупка не удалась.");
                return false;
            }

            string message = $"Куплено: {item.DisplayName} за {item.BuyPrice} золота.";
            SetFeedback(message);
            messageStream?.Publish(LocalMessageCategory.System, message);
            ClearPendingConfirmation();
            Refresh();
            return true;
        }

        public bool TrySell(ItemInstance item)
        {
            if (item == null || !item.IsValid)
            {
                SetFeedback("В этой ячейке нечего продавать.");
                return false;
            }

            if (wallet == null || inventory == null)
            {
                SetFeedback("Магазин не подключен к персонажу.");
                return false;
            }

            int price = Mathf.Max(0, item.SellPrice);
            if (price <= 0)
            {
                SetFeedback($"{item.DisplayName} пока нельзя продать.");
                return false;
            }

            if (!inventory.TryRemoveInstance(item))
            {
                SetFeedback("Предмет уже не найден в рюкзаке.");
                return false;
            }

            wallet.AddGold(price);
            string message = $"Продано: {item.DisplayName} за {price} золота.";
            SetFeedback(message);
            messageStream?.Publish(LocalMessageCategory.System, message);
            ClearPendingConfirmation();
            Refresh();
            return true;
        }

        private void TryBuyStockIndex(int stockIndex)
        {
            ItemDefinition item =
                stockIndex >= 0 && stockIndex < currentStock.Length
                    ? currentStock[stockIndex]
                    : null;
            if (pendingBuyIndex != stockIndex)
            {
                pendingBuyIndex = stockIndex;
                pendingSellSlotIndex = -1;
                SetFeedback(item != null
                    ? $"Купить {item.DisplayName} за {item.BuyPrice}? Нажмите ещё раз для подтверждения."
                    : "У торговца нет выбранного предмета.");
                Refresh();
                return;
            }

            TryBuy(item);
        }

        private void TrySellSlotIndex(int slotIndex)
        {
            ItemInstance item = inventory != null ? inventory.GetItemAt(slotIndex) : null;
            if (pendingSellSlotIndex != slotIndex)
            {
                pendingSellSlotIndex = slotIndex;
                pendingBuyIndex = -1;
                SetFeedback(item != null
                    ? $"Продать {item.DisplayName} за {item.SellPrice}? Нажмите ещё раз для подтверждения."
                    : "В этой ячейке нечего продавать.");
                Refresh();
                return;
            }

            TrySell(item);
        }

        private void HandleWalletChanged(PlayerWallet changedWallet)
        {
            Refresh();
        }

        private void HandleInventoryChanged(PlayerInventory changedInventory)
        {
            Refresh();
        }

        private void Refresh()
        {
            if (goldText != null)
            {
                goldText.text = wallet != null ? $"Золото: {wallet.Gold}" : "Золото: -";
            }

            for (int index = 0; index < buyButtons.Length; index++)
            {
                ItemDefinition item =
                    index < currentStock.Length ? currentStock[index] : null;
                bool canShow = item != null;
                buyButtons[index].gameObject.SetActive(canShow);
                if (!canShow || index >= buyButtonTexts.Length)
                {
                    continue;
                }

                buyButtonTexts[index].text =
                    pendingBuyIndex == index
                        ? $"{item.DisplayName}\nПодтвердить: {item.BuyPrice}"
                        : $"{item.DisplayName}\nЦена: {item.BuyPrice}";
                buyButtons[index].interactable =
                    wallet != null && inventory != null &&
                    inventory.HasFreeSlot && wallet.CanAfford(item.BuyPrice);
            }

            for (int index = 0; index < sellButtons.Length; index++)
            {
                ItemInstance item = inventory != null ? inventory.GetItemAt(index) : null;
                bool hasItem = item != null && item.IsValid;
                if (index < sellButtonTexts.Length)
                {
                    sellButtonTexts[index].text = hasItem
                        ? pendingSellSlotIndex == index
                            ? $"{index + 1}. {item.DisplayName}\nПодтвердить: {item.SellPrice}"
                            : $"{index + 1}. {item.DisplayName}\nПродать: {item.SellPrice}"
                        : $"{index + 1}. Пусто";
                }

                sellButtons[index].interactable = hasItem && item.SellPrice > 0;
            }
        }

        private void SetFeedback(string message)
        {
            if (feedbackText != null)
            {
                feedbackText.text = message ?? string.Empty;
            }
        }

        private void ClearPendingConfirmation()
        {
            pendingBuyIndex = -1;
            pendingSellSlotIndex = -1;
        }

        private void Unsubscribe()
        {
            if (wallet != null)
            {
                wallet.Changed -= HandleWalletChanged;
            }

            if (inventory != null)
            {
                inventory.Changed -= HandleInventoryChanged;
            }
        }
    }
}
