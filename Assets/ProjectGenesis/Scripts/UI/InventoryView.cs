using System;
using ProjectGenesis.Core;
using ProjectGenesis.Data;
using ProjectGenesis.Gameplay;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace ProjectGenesis.UI
{
    public sealed class InventoryView : MonoBehaviour
    {
        [SerializeField] private GameObject windowRoot;
        [SerializeField] private Button openButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button itemActionButton;
        [SerializeField] private Button dropButton;
        [SerializeField] private Button destroyButton;
        [SerializeField] private Button[] slotButtons;
        [SerializeField] private Text[] slotTexts;
        [SerializeField] private Text capacityText;
        [SerializeField] private Text itemNameText;
        [SerializeField] private Text itemDetailsText;
        [SerializeField] private Text itemActionText;
        [SerializeField] private Text destroyActionText;
        [SerializeField] private Text itemFeedbackText;
        [SerializeField] private GameObject destroyConfirmationRoot;
        [SerializeField] private Text destroyConfirmationText;
        [SerializeField] private Button confirmDestroyButton;
        [SerializeField] private Button cancelDestroyButton;
        [SerializeField] private PlayerInventory inventory;
        [SerializeField] private PlayerEquipment equipment;
        [SerializeField] private PlayerItemUseController itemUseController;
        [SerializeField] private PlayerItemDropController itemDropController;
        [SerializeField] private string selectedInstanceId;

        private string actionFeedback;
        private string pendingDestroyInstanceId;

        public event Action<InventoryView> SelectionChanged;

        public GameObject WindowRoot => windowRoot;
        public int SlotCount => slotButtons != null ? slotButtons.Length : 0;
        public string SelectedInstanceId => selectedInstanceId;
        public ItemInstance SelectedItem => GetSelectedItem();
        public PlayerItemUseController ItemUseController => itemUseController;
        public PlayerItemDropController ItemDropController => itemDropController;

        public bool CanDragSlot(int slotIndex)
        {
            return inventory != null && inventory.GetItemAt(slotIndex) != null;
        }

        public bool TryMoveOrSwapSlots(int sourceSlotIndex, int targetSlotIndex)
        {
            return inventory != null &&
                   inventory.TryMoveOrSwap(sourceSlotIndex, targetSlotIndex);
        }

        public void Initialize(
            GameObject inventoryWindow,
            Button inventoryOpenButton,
            Button inventoryCloseButton,
            Button actionButton,
            Button itemDropButton,
            Button itemDestroyButton,
            Button[] inventorySlotButtons,
            Text[] inventorySlotTexts,
            Text capacityLabel,
            Text itemNameLabel,
            Text itemDetailsLabel,
            Text actionLabel,
            Text destroyLabel,
            Text feedbackLabel,
            GameObject confirmationRoot,
            Text confirmationLabel,
            Button confirmationButton,
            Button cancellationButton,
            PlayerInventory playerInventory,
            PlayerEquipment playerEquipment,
            PlayerItemUseController playerItemUseController,
            PlayerItemDropController playerItemDropController)
        {
            windowRoot = inventoryWindow;
            openButton = inventoryOpenButton;
            closeButton = inventoryCloseButton;
            itemActionButton = actionButton;
            dropButton = itemDropButton;
            destroyButton = itemDestroyButton;
            slotButtons = inventorySlotButtons ?? Array.Empty<Button>();
            slotTexts = inventorySlotTexts ?? Array.Empty<Text>();
            capacityText = capacityLabel;
            itemNameText = itemNameLabel;
            itemDetailsText = itemDetailsLabel;
            itemActionText = actionLabel;
            destroyActionText = destroyLabel;
            itemFeedbackText = feedbackLabel;
            destroyConfirmationRoot = confirmationRoot;
            destroyConfirmationText = confirmationLabel;
            confirmDestroyButton = confirmationButton;
            cancelDestroyButton = cancellationButton;
            inventory = playerInventory;
            equipment = playerEquipment;
            itemUseController = playerItemUseController;
            itemDropController = playerItemDropController;
        }

        private void Awake()
        {
            if (openButton != null)
            {
                openButton.onClick.AddListener(ToggleWindow);
            }

            if (closeButton != null)
            {
                closeButton.onClick.AddListener(CloseWindow);
            }

            if (itemActionButton != null)
            {
                itemActionButton.onClick.AddListener(HandleItemAction);
            }

            dropButton?.onClick.AddListener(HandleDropAction);
            destroyButton?.onClick.AddListener(HandleDestroyAction);
            confirmDestroyButton?.onClick.AddListener(ConfirmDestroyAction);
            cancelDestroyButton?.onClick.AddListener(CancelDestroyAction);

            for (int index = 0; index < slotButtons.Length; index++)
            {
                int slotIndex = index;
                slotButtons[index]?.onClick.AddListener(() => HandleSlotSelected(slotIndex));
            }

            if (inventory != null)
            {
                inventory.Changed += HandleInventoryChanged;
            }

            if (equipment != null)
            {
                equipment.Changed += HandleEquipmentChanged;
            }

            CloseWindow();
            Refresh();
        }

        private void OnDestroy()
        {
            if (openButton != null)
            {
                openButton.onClick.RemoveListener(ToggleWindow);
            }

            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(CloseWindow);
            }

            if (itemActionButton != null)
            {
                itemActionButton.onClick.RemoveListener(HandleItemAction);
            }

            dropButton?.onClick.RemoveListener(HandleDropAction);
            destroyButton?.onClick.RemoveListener(HandleDestroyAction);
            confirmDestroyButton?.onClick.RemoveListener(ConfirmDestroyAction);
            cancelDestroyButton?.onClick.RemoveListener(CancelDestroyAction);

            if (slotButtons != null)
            {
                foreach (Button slotButton in slotButtons)
                {
                    slotButton?.onClick.RemoveAllListeners();
                }
            }

            if (inventory != null)
            {
                inventory.Changed -= HandleInventoryChanged;
            }

            if (equipment != null)
            {
                equipment.Changed -= HandleEquipmentChanged;
            }

        }

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (!GameplayInputGate.IsTextEntryFocused && keyboard != null &&
                keyboard.iKey.wasPressedThisFrame)
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

            windowRoot.SetActive(!windowRoot.activeSelf);
            if (!windowRoot.activeSelf)
            {
                ResetDestroyConfirmation();
            }

            Refresh();
        }

        private void CloseWindow()
        {
            if (windowRoot != null)
            {
                windowRoot.SetActive(false);
            }

            ResetDestroyConfirmation();
        }

        private void HandleItemAction()
        {
            ItemInstance item = GetSelectedItem();
            if (item == null)
            {
                return;
            }

            bool succeeded = item.ItemType switch
            {
                ItemType.Weapon => HandleWeaponAction(item),
                ItemType.Armor => HandleArmorAction(item),
                ItemType.Consumable => HandleConsumableAction(item),
                _ => false
            };

            if (item.ItemType != ItemType.Consumable)
            {
                actionFeedback = succeeded
                    ? "Экипировка обновлена."
                    : "Этот предмет нельзя использовать здесь.";
            }

            Refresh();
        }

        private void HandleSlotSelected(int slotIndex)
        {
            if (inventory == null)
            {
                return;
            }

            ItemInstance item = inventory.GetItemAt(slotIndex);
            if (item == null)
            {
                return;
            }

            selectedInstanceId = item != null ? item.InstanceId : string.Empty;
            actionFeedback = string.Empty;
            ResetDestroyConfirmation();
            Refresh();
            SelectionChanged?.Invoke(this);
        }

        private void HandleInventoryChanged(PlayerInventory _)
        {
            string previousSelection = selectedInstanceId;
            Refresh();
            if (previousSelection != selectedInstanceId)
            {
                ResetDestroyConfirmation();
                SelectionChanged?.Invoke(this);
            }
        }

        private void HandleEquipmentChanged(PlayerEquipment _)
        {
            Refresh();
        }

        private void Refresh()
        {
            if (inventory == null || equipment == null)
            {
                return;
            }

            if (capacityText != null)
            {
                capacityText.text = $"Ячейки: {inventory.Count} / {inventory.Capacity}";
            }

            ItemInstance item = ResolveSelectedItem();
            bool hasItem = item != null;

            RefreshSlots(item);

            if (itemNameText != null)
            {
                itemNameText.text = hasItem ? item.DisplayName : "Инвентарь пуст";
            }

            if (itemDetailsText != null)
            {
                itemDetailsText.text = hasItem
                    ? GetItemDetails(item)
                    : "Побеждайте врагов и подбирайте выпавшие предметы.";
            }

            if (itemActionButton != null)
            {
                itemActionButton.gameObject.SetActive(hasItem);
            }

            dropButton?.gameObject.SetActive(hasItem);
            destroyButton?.gameObject.SetActive(hasItem);

            if (itemActionText != null && hasItem)
            {
                itemActionText.text = item.ItemType == ItemType.Consumable
                    ? "Использовать"
                    : "Надеть";
            }

            if (itemFeedbackText != null)
            {
                itemFeedbackText.text = actionFeedback;
            }

            if (!hasItem)
            {
                ResetDestroyConfirmation();
            }
        }

        private void HandleDropAction()
        {
            ItemInstance item = GetSelectedItem();
            if (item == null || itemDropController == null)
            {
                return;
            }

            bool dropped = itemDropController.TryDrop(item, out _);
            actionFeedback = dropped
                ? $"Выброшено: {item.DisplayName}."
                : "Предмет сейчас выбросить нельзя.";
            ResetDestroyConfirmation();
            Refresh();
        }

        private void HandleDestroyAction()
        {
            ItemInstance item = GetSelectedItem();
            if (item == null || itemDropController == null)
            {
                return;
            }

            RequestDestroy(item);
        }

        public void RequestDestroySlot(int slotIndex)
        {
            ItemInstance item = inventory != null ? inventory.GetItemAt(slotIndex) : null;
            if (item == null)
            {
                return;
            }

            selectedInstanceId = item.InstanceId;
            actionFeedback = string.Empty;
            SelectionChanged?.Invoke(this);
            RequestDestroy(item);
            Refresh();
        }

        private void RequestDestroy(ItemInstance item)
        {
            pendingDestroyInstanceId = item.InstanceId;
            if (destroyConfirmationText != null)
            {
                destroyConfirmationText.text =
                    $"Удалить {item.DisplayName} навсегда?\nЭто действие нельзя отменить.";
            }

            if (destroyConfirmationRoot != null)
            {
                destroyConfirmationRoot.SetActive(true);
                destroyConfirmationRoot.transform.SetAsLastSibling();
            }
        }

        private void ConfirmDestroyAction()
        {
            ItemInstance item = inventory != null
                ? inventory.FindByInstanceId(pendingDestroyInstanceId)
                : null;
            if (item == null || itemDropController == null)
            {
                ResetDestroyConfirmation();
                return;
            }

            bool destroyed = itemDropController.TryDestroy(item);
            actionFeedback = destroyed
                ? $"Удалено: {item.DisplayName}."
                : "Предмет сейчас удалить нельзя.";
            ResetDestroyConfirmation();
            Refresh();
        }

        private void CancelDestroyAction()
        {
            ResetDestroyConfirmation();
            Refresh();
        }

        private void ResetDestroyConfirmation()
        {
            pendingDestroyInstanceId = string.Empty;
            if (destroyActionText != null)
            {
                destroyActionText.text = "Корзина";
            }

            destroyConfirmationRoot?.SetActive(false);
        }

        private ItemInstance ResolveSelectedItem()
        {
            ItemInstance selected = GetSelectedItem();
            if (selected != null)
            {
                return selected;
            }

            selected = GetFirstOccupiedItem();
            selectedInstanceId = selected != null ? selected.InstanceId : string.Empty;
            return selected;
        }

        private ItemInstance GetSelectedItem()
        {
            return inventory != null
                ? inventory.FindByInstanceId(selectedInstanceId)
                : null;
        }

        private void RefreshSlots(ItemInstance selected)
        {
            if (slotButtons == null || slotTexts == null)
            {
                return;
            }

            int visibleSlots = Mathf.Min(slotButtons.Length, slotTexts.Length);
            for (int index = 0; index < visibleSlots; index++)
            {
                Button button = slotButtons[index];
                Text label = slotTexts[index];
                ItemInstance item = inventory != null ? inventory.GetItemAt(index) : null;
                bool isSelected = item != null && selected != null &&
                                  item.InstanceId == selected.InstanceId;
                if (label != null)
                {
                    label.text = item != null
                        ? $"{index + 1}. {item.DisplayName}"
                        : $"{index + 1}. Пусто";
                    label.color = item != null
                        ? Color.white
                        : new Color(0.55f, 0.6f, 0.64f);
                }

                if (button != null)
                {
                    button.interactable = item != null;
                    ColorBlock colors = button.colors;
                    colors.normalColor = isSelected
                        ? new Color(0.42f, 0.32f, 0.12f, 1f)
                        : new Color(0.16f, 0.25f, 0.34f, 1f);
                    colors.highlightedColor = isSelected
                        ? new Color(0.5f, 0.39f, 0.16f, 1f)
                        : new Color(0.22f, 0.34f, 0.44f, 1f);
                    colors.selectedColor = colors.highlightedColor;
                    colors.disabledColor = new Color(0.1f, 0.12f, 0.14f, 0.72f);
                    button.colors = colors;
                }
            }
        }

        private ItemInstance GetFirstOccupiedItem()
        {
            if (inventory == null)
            {
                return null;
            }

            for (int index = 0; index < inventory.Capacity; index++)
            {
                ItemInstance item = inventory.GetItemAt(index);
                if (item != null)
                {
                    return item;
                }
            }

            return null;
        }

        private bool HandleWeaponAction(ItemInstance item)
        {
            if (equipment == null)
            {
                return false;
            }

            return equipment.EquipMainHand(item);
        }

        private bool HandleArmorAction(ItemInstance item)
        {
            if (equipment == null)
            {
                return false;
            }

            return equipment.EquipBody(item);
        }

        private bool HandleConsumableAction(ItemInstance item)
        {
            if (itemUseController == null)
            {
                actionFeedback = "Предмет сейчас использовать нельзя.";
                return false;
            }

            if (!itemUseController.TryUse(item, out ItemUseResult result))
            {
                actionFeedback = result switch
                {
                    ItemUseResult.FullHealth => "Здоровье уже полное.",
                    ItemUseResult.Dead => "Нельзя использовать после смерти.",
                    _ => "Предмет сейчас использовать нельзя."
                };
                return false;
            }

            actionFeedback = $"Восстановлено до {itemUseController.Health.CurrentHealth} здоровья.";
            return true;
        }

        private static string GetItemDetails(ItemInstance item)
        {
            return item.ItemType switch
            {
                ItemType.Weapon => $"Оружие · +{item.AttackBonus} к силе атаки",
                ItemType.Armor => $"Броня · +{item.DefenseBonus} к защите",
                ItemType.Consumable => $"Расходник · восстанавливает {item.HealingAmount} здоровья",
                _ => "Неизвестный предмет"
            };
        }
    }
}
