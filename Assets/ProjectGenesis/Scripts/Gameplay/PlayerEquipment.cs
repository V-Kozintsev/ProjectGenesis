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
        [SerializeField] private ItemInstance body;

        private PlayerInventory inventory;
        private CombatStats combatStats;

        public event Action<PlayerEquipment> Changed;

        public ItemInstance MainHand => mainHand != null && mainHand.IsValid ? mainHand : null;
        public ItemInstance Body => body != null && body.IsValid ? body : null;

        private void Awake()
        {
            EnsureDependencies();
            if (mainHand != null && !mainHand.IsValid)
            {
                mainHand = null;
            }

            if (body != null && !body.IsValid)
            {
                body = null;
            }

            inventory.Changed += HandleInventoryChanged;
            ApplyEquipmentBonuses();
        }

        private void OnDestroy()
        {
            if (inventory != null)
            {
                inventory.Changed -= HandleInventoryChanged;
            }
        }

        public bool EquipMainHand(ItemInstance item)
        {
            if (!EnsureDependencies() || item == null ||
                item.ItemType != ItemType.Weapon || !inventory.Contains(item))
            {
                return false;
            }

            mainHand = item;
            ApplyEquipmentBonuses();
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
            ApplyEquipmentBonuses();
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
            ApplyEquipmentBonuses();
            Changed?.Invoke(this);
        }

        public bool EquipBody(ItemInstance item)
        {
            if (!EnsureDependencies() || item == null ||
                item.ItemType != ItemType.Armor || !inventory.Contains(item))
            {
                return false;
            }

            body = item;
            ApplyEquipmentBonuses();
            Changed?.Invoke(this);
            return true;
        }

        public void UnequipBody()
        {
            if (body == null)
            {
                return;
            }

            body = null;
            ApplyEquipmentBonuses();
            Changed?.Invoke(this);
        }

        public void RestoreBody(string instanceId)
        {
            if (!EnsureDependencies())
            {
                body = null;
                return;
            }

            ItemInstance item = inventory.FindByInstanceId(instanceId);
            body = item != null && item.ItemType == ItemType.Armor
                ? item
                : null;
            ApplyEquipmentBonuses();
            Changed?.Invoke(this);
        }

        public bool IsEquipped(ItemInstance item)
        {
            if (item == null)
            {
                return false;
            }

            return MainHand != null && MainHand.InstanceId == item.InstanceId ||
                   Body != null && Body.InstanceId == item.InstanceId;
        }

        private void HandleInventoryChanged(PlayerInventory _)
        {
            bool changed = false;
            if (mainHand != null && !inventory.Contains(mainHand))
            {
                mainHand = null;
                changed = true;
            }

            if (body != null && !inventory.Contains(body))
            {
                body = null;
                changed = true;
            }

            if (!changed)
            {
                return;
            }

            ApplyEquipmentBonuses();
            Changed?.Invoke(this);
        }

        private void ApplyEquipmentBonuses()
        {
            if (!EnsureDependencies())
            {
                return;
            }

            int bonus = MainHand != null ? MainHand.AttackBonus : 0;
            combatStats.SetEquipmentAttackBonus(bonus);
            int defenseBonus = Body != null ? Body.DefenseBonus : 0;
            combatStats.SetEquipmentDefenseBonus(defenseBonus);
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
