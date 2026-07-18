using UnityEngine;

namespace ProjectGenesis.Data
{
    public enum ItemType
    {
        Weapon
    }

    [CreateAssetMenu(fileName = "ITM_NewItem", menuName = "Project Genesis/Item Definition")]
    public sealed class ItemDefinition : ScriptableObject
    {
        [SerializeField] private string itemId = "item.new";
        [SerializeField] private string displayName = "New Item";
        [SerializeField] private ItemType itemType = ItemType.Weapon;
        [SerializeField, Min(0)] private int attackBonus;

        public string ItemId => itemId;
        public string DisplayName => displayName;
        public ItemType ItemType => itemType;
        public int AttackBonus => attackBonus;

        public void Configure(string id, string itemName, ItemType type, int bonus)
        {
            itemId = string.IsNullOrWhiteSpace(id) ? "item.new" : id;
            displayName = string.IsNullOrWhiteSpace(itemName) ? "New Item" : itemName;
            itemType = type;
            attackBonus = Mathf.Max(0, bonus);
        }
    }
}
