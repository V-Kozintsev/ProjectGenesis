using System;
using ProjectGenesis.Data;
using UnityEngine;

namespace ProjectGenesis.Gameplay
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerInventory), typeof(CombatStats))]
    public sealed class PlayerEquipment : MonoBehaviour
    {
        [SerializeField] private ItemDefinition mainHand;

        private PlayerInventory inventory;
        private CombatStats combatStats;

        public event Action<PlayerEquipment> Changed;

        public ItemDefinition MainHand => mainHand;

        private void Awake()
        {
            inventory = GetComponent<PlayerInventory>();
            combatStats = GetComponent<CombatStats>();
            ApplyMainHandBonus();
        }

        public bool EquipMainHand(ItemDefinition item)
        {
            if (item == null || item.ItemType != ItemType.Weapon || !inventory.Contains(item))
            {
                return false;
            }

            mainHand = item;
            ApplyMainHandBonus();
            Changed?.Invoke(this);
            return true;
        }

        public void UnequipMainHand()
        {
            if (mainHand == null)
            {
                return;
            }

            mainHand = null;
            ApplyMainHandBonus();
            Changed?.Invoke(this);
        }

        public void RestoreMainHand(ItemDefinition item)
        {
            mainHand = item != null && item.ItemType == ItemType.Weapon && inventory.Contains(item)
                ? item
                : null;
            ApplyMainHandBonus();
            Changed?.Invoke(this);
        }

        private void ApplyMainHandBonus()
        {
            int bonus = mainHand != null ? mainHand.AttackBonus : 0;
            combatStats.SetEquipmentAttackBonus(bonus);
        }
    }
}
