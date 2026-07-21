using System;
using ProjectGenesis.Data;
using UnityEngine;

namespace ProjectGenesis.Gameplay
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerInventory), typeof(CombatStats))]
    public sealed class PlayerEquipment : MonoBehaviour
    {
        [SerializeField] private ItemInstance mainHand;

        private PlayerInventory inventory;
        private CombatStats combatStats;

        public event Action<PlayerEquipment> Changed;

        public ItemInstance MainHand => mainHand != null && mainHand.IsValid ? mainHand : null;

        private void Awake()
        {
            EnsureDependencies();
            if (mainHand != null && !mainHand.IsValid)
            {
                mainHand = null;
            }

            ApplyMainHandBonus();
        }

        public bool EquipMainHand(ItemInstance item)
        {
            if (!EnsureDependencies() || item == null ||
                item.ItemType != ItemType.Weapon || !inventory.Contains(item))
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

        public void RestoreMainHand(string instanceId)
        {
            if (!EnsureDependencies())
            {
                mainHand = null;
                return;
            }

            ItemInstance item = inventory.FindByInstanceId(instanceId);
            mainHand = item != null && item.ItemType == ItemType.Weapon
                ? item
                : null;
            ApplyMainHandBonus();
            Changed?.Invoke(this);
        }

        private void ApplyMainHandBonus()
        {
            if (!EnsureDependencies())
            {
                return;
            }

            int bonus = MainHand != null ? MainHand.AttackBonus : 0;
            combatStats.SetEquipmentAttackBonus(bonus);
        }

        private bool EnsureDependencies()
        {
            if (inventory == null)
            {
                inventory = GetComponent<PlayerInventory>();
            }

            if (combatStats == null)
            {
                combatStats = GetComponent<CombatStats>();
            }

            return inventory != null && combatStats != null;
        }
    }
}
