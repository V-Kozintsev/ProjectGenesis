using System;
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
        [SerializeField] private Button[] slotButtons;
        [SerializeField] private Text[] slotTexts;
        [SerializeField] private Text capacityText;
        [SerializeField] private Text attackPowerText;
        [SerializeField] private Text mainHandText;
        [SerializeField] private Text itemNameText;
        [SerializeField] private Text itemDetailsText;
        [SerializeField] private Text itemActionText;
        [SerializeField] private PlayerInventory inventory;
        [SerializeField] private PlayerEquipment equipment;
        [SerializeField] private CombatStats combatStats;
        [SerializeField] private string selectedInstanceId;

        public GameObject WindowRoot => windowRoot;
        public int SlotCount => slotButtons != null ? slotButtons.Length : 0;
        public string SelectedInstanceId => selectedInstanceId;

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
            Button[] inventorySlotButtons,
            Text[] inventorySlotTexts,
            Text capacityLabel,
            Text attackPowerLabel,
            Text mainHandLabel,
            Text itemNameLabel,
            Text itemDetailsLabel,
            Text actionLabel,
            PlayerInventory playerInventory,
            PlayerEquipment playerEquipment,
            CombatStats playerCombatStats)
        {
            windowRoot = inventoryWindow;
            openButton = inventoryOpenButton;
            closeButton = inventoryCloseButton;
            itemActionButton = actionButton;
            slotButtons = inventorySlotButtons ?? Array.Empty<Button>();
            slotTexts = inventorySlotTexts ?? Array.Empty<Text>();
            capacityText = capacityLabel;
            attackPowerText = attackPowerLabel;
            mainHandText = mainHandLabel;
            itemNameText = itemNameLabel;
            itemDetailsText = itemDetailsLabel;
            itemActionText = actionLabel;
            inventory = playerInventory;
            equipment = playerEquipment;
            combatStats = playerCombatStats;
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

            if (combatStats != null)
            {
                combatStats.Changed += HandleCombatStatsChanged;
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

            if (combatStats != null)
            {
                combatStats.Changed -= HandleCombatStatsChanged;
            }
        }

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard != null && keyboard.iKey.wasPressedThisFrame)
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
            Refresh();
        }

        private void CloseWindow()
        {
            if (windowRoot != null)
            {
                windowRoot.SetActive(false);
            }
        }

        private void HandleItemAction()
        {
            ItemInstance item = GetSelectedItem();
            if (item == null || equipment == null)
            {
                return;
            }

            if (equipment.MainHand != null &&
                equipment.MainHand.InstanceId == item.InstanceId)
            {
                equipment.UnequipMainHand();
            }
            else
            {
                equipment.EquipMainHand(item);
            }
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
            Refresh();
        }

        private void HandleInventoryChanged(PlayerInventory _)
        {
            Refresh();
        }

        private void HandleEquipmentChanged(PlayerEquipment _)
        {
            Refresh();
        }

        private void HandleCombatStatsChanged(CombatStats _)
        {
            Refresh();
        }

        private void Refresh()
        {
            if (inventory == null || equipment == null || combatStats == null)
            {
                return;
            }

            if (capacityText != null)
            {
                capacityText.text = $"Ячейки: {inventory.Count} / {inventory.Capacity}";
            }

            if (attackPowerText != null)
            {
                attackPowerText.text = $"Сила атаки: {combatStats.AttackPower}";
            }

            if (mainHandText != null)
            {
                mainHandText.text = equipment.MainHand != null
                    ? $"Оружие: {equipment.MainHand.DisplayName}"
                    : "Оружие: не экипировано";
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
                    ? $"Оружие, +{item.AttackBonus} к силе атаки"
                    : "Победите волка и подберите выпавший предмет.";
            }

            if (itemActionButton != null)
            {
                itemActionButton.gameObject.SetActive(hasItem);
            }

            if (itemActionText != null && hasItem)
            {
                itemActionText.text = equipment.MainHand != null &&
                                      equipment.MainHand.InstanceId == item.InstanceId
                    ? "Снять"
                    : "Надеть";
            }
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
                bool isEquipped = item != null && equipment.MainHand != null &&
                                  item.InstanceId == equipment.MainHand.InstanceId;

                if (label != null)
                {
                    label.text = item != null
                        ? $"{index + 1}. {item.DisplayName}{(isEquipped ? "  [Надето]" : string.Empty)}"
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
    }
}
