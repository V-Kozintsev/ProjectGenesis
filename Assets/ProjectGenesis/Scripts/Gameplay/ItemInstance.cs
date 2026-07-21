using System;
using ProjectGenesis.Data;
using UnityEngine;

namespace ProjectGenesis.Gameplay
{
    [Serializable]
    public sealed class ItemInstance
    {
        [SerializeField] private string instanceId;
        [SerializeField] private ItemDefinition definition;

        private ItemInstance(string id, ItemDefinition itemDefinition)
        {
            instanceId = id;
            definition = itemDefinition;
        }

        public string InstanceId => instanceId;
        public ItemDefinition Definition => definition;
        public string ItemId => definition != null ? definition.ItemId : string.Empty;
        public string DisplayName => definition != null ? definition.DisplayName : "Unknown Item";
        public ItemType ItemType => definition != null ? definition.ItemType : ItemType.Weapon;
        public int AttackBonus => definition != null ? definition.AttackBonus : 0;
        public bool IsValid => definition != null && !string.IsNullOrWhiteSpace(instanceId);

        public static ItemInstance Create(ItemDefinition definition, string stableInstanceId = null)
        {
            if (definition == null)
            {
                return null;
            }

            string id = string.IsNullOrWhiteSpace(stableInstanceId)
                ? Guid.NewGuid().ToString("N")
                : stableInstanceId.Trim();
            return new ItemInstance(id, definition);
        }
    }
}
