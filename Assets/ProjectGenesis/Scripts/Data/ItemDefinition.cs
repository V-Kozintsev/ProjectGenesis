using UnityEngine;

namespace ProjectGenesis.Data
{
    public enum ItemType
    {
        Weapon,
        Armor,
        Consumable
    }

    [CreateAssetMenu(fileName = "ITM_NewItem", menuName = "Project Genesis/Item Definition")]
    public sealed class ItemDefinition : ScriptableObject
    {
        [SerializeField] private string itemId = "item.new";
        [SerializeField] private string displayName = "New Item";
        [SerializeField] private ItemType itemType = ItemType.Weapon;
        [SerializeField, Min(0)] private int attackBonus;
        [SerializeField, Min(0)] private int defenseBonus;
        [SerializeField, Min(0)] private int healingAmount;

        public string ItemId => itemId;
        public string DisplayName => displayName;
        public ItemType ItemType => itemType;
        public int AttackBonus => attackBonus;
        public int DefenseBonus => defenseBonus;
        public int HealingAmount => healingAmount;

        public void Configure(
            string id,
            string itemName,
            ItemType type,
            int attack = 0,
            int defense = 0,
            int healing = 0)
        {
            itemId = string.IsNullOrWhiteSpace(id) ? "item.new" : id;
            displayName = string.IsNullOrWhiteSpace(itemName) ? "New Item" : itemName;
            itemType = type;
            attackBonus = type == ItemType.Weapon ? Mathf.Max(0, attack) : 0;
            defenseBonus = type == ItemType.Armor ? Mathf.Max(0, defense) : 0;
            healingAmount = type == ItemType.Consumable ? Mathf.Max(0, healing) : 0;
        }
    }
}
