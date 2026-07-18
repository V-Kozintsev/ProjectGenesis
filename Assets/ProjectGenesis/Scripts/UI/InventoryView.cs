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
        [SerializeField] private Text capacityText;
        [SerializeField] private Text attackPowerText;
        [SerializeField] private Text mainHandText;
        [SerializeField] private Text itemNameText;
        [SerializeField] private Text itemDetailsText;
        [SerializeField] private Text itemActionText;
        [SerializeField] private PlayerInventory inventory;
        [SerializeField] private PlayerEquipment equipment;
        [SerializeField] private CombatStats combatStats;

        public void Initialize(
            GameObject inventoryWindow,
            Button inventoryOpenButton,
            Button inventoryCloseButton,
            Button actionButton,
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
            ItemDefinition item = GetFirstItem();
            if (item == null || equipment == null)
            {
                return;
            }

            if (equipment.MainHand == item)
            {
                equipment.UnequipMainHand();
            }
            else
            {
                equipment.EquipMainHand(item);
            }
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

            ItemDefinition item = GetFirstItem();
            bool hasItem = item != null;

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
                itemActionText.text = equipment.MainHand == item ? "Снять" : "Надеть";
            }
        }

        private ItemDefinition GetFirstItem()
        {
            return inventory != null && inventory.Count > 0 ? inventory.Items[0] : null;
        }
    }
}
