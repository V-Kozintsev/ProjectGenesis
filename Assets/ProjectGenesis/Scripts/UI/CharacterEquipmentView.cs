using ProjectGenesis.Data;
using ProjectGenesis.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectGenesis.UI
{
    public sealed class CharacterEquipmentView : MonoBehaviour
    {
        private enum EquipmentSlot
        {
            MainHand,
            Body
        }

        [SerializeField] private GameObject windowRoot;
        [SerializeField] private Button mainHandSlotButton;
        [SerializeField] private Button bodySlotButton;
        [SerializeField] private Button mainHandUnequipButton;
        [SerializeField] private Button bodyUnequipButton;
        [SerializeField] private Text mainHandSlotText;
        [SerializeField] private Text bodySlotText;
        [SerializeField] private Text comparisonText;
        [SerializeField] private Button actionButton;
        [SerializeField] private Text actionText;
        [SerializeField] private Text feedbackText;
        [SerializeField] private InventoryView inventoryView;
        [SerializeField] private PlayerInventory inventory;
        [SerializeField] private PlayerEquipment equipment;
        [SerializeField] private EquipmentSlot selectedSlot;

        private string feedback;

        public GameObject WindowRoot => windowRoot;
        public Button MainHandSlotButton => mainHandSlotButton;
        public Button BodySlotButton => bodySlotButton;
        public Button MainHandUnequipButton => mainHandUnequipButton;
        public Button BodyUnequipButton => bodyUnequipButton;
        public PlayerInventory Inventory => inventory;
        public PlayerEquipment Equipment => equipment;

        public void Initialize(
            GameObject characterWindow,
            Button mainHandButton,
            Button bodyButton,
            Button mainHandRemoveButton,
            Button bodyRemoveButton,
            Text mainHandLabel,
            Text bodyLabel,
            Text comparisonLabel,
            Button equipmentActionButton,
            Text equipmentActionText,
            Text equipmentFeedbackText,
            InventoryView bagView,
            PlayerInventory playerInventory,
            PlayerEquipment playerEquipment)
        {
            windowRoot = characterWindow;
            mainHandSlotButton = mainHandButton;
            bodySlotButton = bodyButton;
            mainHandUnequipButton = mainHandRemoveButton;
            bodyUnequipButton = bodyRemoveButton;
            mainHandSlotText = mainHandLabel;
            bodySlotText = bodyLabel;
            comparisonText = comparisonLabel;
            actionButton = equipmentActionButton;
            actionText = equipmentActionText;
            feedbackText = equipmentFeedbackText;
            inventoryView = bagView;
            inventory = playerInventory;
            equipment = playerEquipment;
        }

        private void Awake()
        {
            mainHandSlotButton?.onClick.AddListener(SelectMainHand);
            bodySlotButton?.onClick.AddListener(SelectBody);
            mainHandUnequipButton?.onClick.AddListener(UnequipMainHand);
            bodyUnequipButton?.onClick.AddListener(UnequipBody);
            actionButton?.onClick.AddListener(HandleAction);

            if (inventoryView != null)
            {
                inventoryView.SelectionChanged += HandleInventorySelectionChanged;
            }

            if (inventory != null)
            {
                inventory.Changed += HandleInventoryChanged;
            }

            if (equipment != null)
            {
                equipment.Changed += HandleEquipmentChanged;
            }

            Refresh();
        }

        private void OnDestroy()
        {
            mainHandSlotButton?.onClick.RemoveListener(SelectMainHand);
            bodySlotButton?.onClick.RemoveListener(SelectBody);
            mainHandUnequipButton?.onClick.RemoveListener(UnequipMainHand);
            bodyUnequipButton?.onClick.RemoveListener(UnequipBody);
            actionButton?.onClick.RemoveListener(HandleAction);

            if (inventoryView != null)
            {
                inventoryView.SelectionChanged -= HandleInventorySelectionChanged;
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

        private void SelectMainHand()
        {
            selectedSlot = EquipmentSlot.MainHand;
            feedback = string.Empty;
            Refresh();
        }

        private void SelectBody()
        {
            selectedSlot = EquipmentSlot.Body;
            feedback = string.Empty;
            Refresh();
        }

        private void HandleAction()
        {
            if (equipment == null || inventory == null)
            {
                return;
            }

            ItemInstance candidate = GetCompatibleSelectedBagItem();
            if (candidate == null)
            {
                return;
            }

            bool equipped = selectedSlot == EquipmentSlot.MainHand
                ? equipment.EquipMainHand(candidate)
                : equipment.EquipBody(candidate);
            feedback = equipped
                ? $"Надето: {candidate.DisplayName}."
                : "Не удалось надеть предмет.";

            Refresh();
        }

        private void UnequipMainHand()
        {
            Unequip(EquipmentSlot.MainHand);
        }

        private void UnequipBody()
        {
            Unequip(EquipmentSlot.Body);
        }

        private void Unequip(EquipmentSlot slot)
        {
            if (equipment == null)
            {
                return;
            }

            selectedSlot = slot;
            ItemInstance current = GetCurrentItem();
            if (current == null)
            {
                return;
            }

            bool unequipped = slot == EquipmentSlot.MainHand
                ? equipment.UnequipMainHand()
                : equipment.UnequipBody();
            feedback = unequipped
                ? $"Снято: {current.DisplayName}."
                : "В сумке нет свободной ячейки.";
            Refresh();
        }

        private void HandleInventorySelectionChanged(InventoryView _)
        {
            feedback = string.Empty;
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

        private void Refresh()
        {
            if (equipment == null)
            {
                return;
            }

            SetSlotText(mainHandSlotText, "Правая рука", equipment.MainHand);
            SetSlotText(bodySlotText, "Тело", equipment.Body);
            RefreshSlotColors();

            ItemInstance current = GetCurrentItem();
            ItemInstance candidate = GetCompatibleSelectedBagItem();
            if (comparisonText != null)
            {
                comparisonText.text = BuildComparison(current, candidate);
            }

            if (actionButton != null)
            {
                actionButton.gameObject.SetActive(candidate != null);
            }

            if (actionText != null)
            {
                actionText.text = candidate != null
                    ? current != null ? "Заменить" : "Надеть"
                    : string.Empty;
            }

            if (mainHandUnequipButton != null)
            {
                mainHandUnequipButton.gameObject.SetActive(equipment.MainHand != null);
            }

            if (bodyUnequipButton != null)
            {
                bodyUnequipButton.gameObject.SetActive(equipment.Body != null);
            }

            if (feedbackText != null)
            {
                feedbackText.text = feedback;
            }
        }

        private ItemInstance GetCurrentItem()
        {
            return selectedSlot == EquipmentSlot.MainHand
                ? equipment?.MainHand
                : equipment?.Body;
        }

        private ItemInstance GetCompatibleSelectedBagItem()
        {
            ItemInstance selected = inventoryView != null ? inventoryView.SelectedItem : null;
            ItemType expectedType = selectedSlot == EquipmentSlot.MainHand
                ? ItemType.Weapon
                : ItemType.Armor;
            return selected != null && selected.ItemType == expectedType ? selected : null;
        }

        private void RefreshSlotColors()
        {
            SetButtonColors(mainHandSlotButton, selectedSlot == EquipmentSlot.MainHand);
            SetButtonColors(bodySlotButton, selectedSlot == EquipmentSlot.Body);
        }

        private static void SetSlotText(Text label, string slotName, ItemInstance item)
        {
            if (label == null)
            {
                return;
            }

            string itemDetails = item == null
                ? "Пусто"
                : item.ItemType == ItemType.Weapon
                    ? $"{item.DisplayName}\nАтака +{item.AttackBonus}"
                    : $"{item.DisplayName}\nЗащита +{item.DefenseBonus}";
            label.text = $"{slotName}\n{itemDetails}";
        }

        private static void SetButtonColors(Button button, bool selected)
        {
            if (button == null)
            {
                return;
            }

            ColorBlock colors = button.colors;
            colors.normalColor = selected
                ? new Color(0.42f, 0.32f, 0.12f, 1f)
                : new Color(0.16f, 0.25f, 0.34f, 1f);
            colors.highlightedColor = selected
                ? new Color(0.5f, 0.39f, 0.16f, 1f)
                : new Color(0.22f, 0.34f, 0.44f, 1f);
            colors.selectedColor = colors.highlightedColor;
            button.colors = colors;
        }

        private string BuildComparison(ItemInstance current, ItemInstance candidate)
        {
            if (candidate == null)
            {
                return current == null
                    ? "Слот пуст."
                    : current.ItemType == ItemType.Weapon
                        ? $"Текущий бонус атаки: +{current.AttackBonus}."
                        : $"Текущий бонус защиты: +{current.DefenseBonus}.";
            }

            int currentBonus = current == null
                ? 0
                : selectedSlot == EquipmentSlot.MainHand
                    ? current.AttackBonus
                    : current.DefenseBonus;
            int candidateBonus = selectedSlot == EquipmentSlot.MainHand
                ? candidate.AttackBonus
                : candidate.DefenseBonus;
            int difference = candidateBonus - currentBonus;
            string statName = selectedSlot == EquipmentSlot.MainHand ? "атаки" : "защиты";
            return $"Выбрано в сумке: {candidate.DisplayName}\n" +
                   $"Бонус {statName}: +{currentBonus} -> +{candidateBonus} " +
                   $"({FormatSigned(difference)}).";
        }

        private static string FormatSigned(int value)
        {
            return value > 0 ? $"+{value}" : value.ToString();
        }
    }
}
