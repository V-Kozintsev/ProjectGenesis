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
        private LocalMessageStream messageStream;

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

            ApplyEquipmentBonuses();
        }

        public bool EquipMainHand(ItemInstance item)
        {
            if (!EnsureDependencies() || item == null ||
                item.ItemType != ItemType.Weapon || !inventory.Contains(item))
            {
                return false;
            }

            ItemInstance previousItem = MainHand;
            if (!inventory.TryReplaceInstance(item, previousItem))
            {
                return false;
            }

            mainHand = item;
            ApplyEquipmentBonuses();
            Changed?.Invoke(this);
            PublishEquipmentMessage("Экипировано оружие", item, previousItem);
            return true;
        }

        public bool UnequipMainHand()
        {
            ItemInstance item = MainHand;
            if (item == null)
            {
                return false;
            }

            if (!inventory.TryAddInstance(item))
            {
                return false;
            }

            mainHand = null;
            ApplyEquipmentBonuses();
            Changed?.Invoke(this);
            messageStream?.Publish(
                LocalMessageCategory.System,
                $"Снято оружие: {item.DisplayName}.");
            return true;
        }

        public void RestoreMainHand(string instanceId)
        {
            if (!EnsureDependencies())
            {
                mainHand = null;
                return;
            }

            ItemInstance item = inventory.FindByInstanceId(instanceId);
            mainHand = null;
            if (item != null && item.ItemType == ItemType.Weapon)
            {
                inventory.TryReplaceInstance(item, null);
                mainHand = item;
            }

            ApplyEquipmentBonuses();
            Changed?.Invoke(this);
        }

        public void RestoreMainHand(ItemInstance item)
        {
            mainHand = item != null && item.IsValid && item.ItemType == ItemType.Weapon
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

            ItemInstance previousItem = Body;
            if (!inventory.TryReplaceInstance(item, previousItem))
            {
                return false;
            }

            body = item;
            ApplyEquipmentBonuses();
            Changed?.Invoke(this);
            PublishEquipmentMessage("Экипирована броня", item, previousItem);
            return true;
        }

        public bool UnequipBody()
        {
            ItemInstance item = Body;
            if (item == null)
            {
                return false;
            }

            if (!inventory.TryAddInstance(item))
            {
                return false;
            }

            body = null;
            ApplyEquipmentBonuses();
            Changed?.Invoke(this);
            messageStream?.Publish(
                LocalMessageCategory.System,
                $"Снята броня: {item.DisplayName}.");
            return true;
        }

        public void RestoreBody(string instanceId)
        {
            if (!EnsureDependencies())
            {
                body = null;
                return;
            }

            ItemInstance item = inventory.FindByInstanceId(instanceId);
            body = null;
            if (item != null && item.ItemType == ItemType.Armor)
            {
                inventory.TryReplaceInstance(item, null);
                body = item;
            }

            ApplyEquipmentBonuses();
            Changed?.Invoke(this);
        }

        public void RestoreBody(ItemInstance item)
        {
            body = item != null && item.IsValid && item.ItemType == ItemType.Armor
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

            messageStream ??= GetComponent<LocalMessageStream>();

            return inventory != null && combatStats != null;
        }

        private void PublishEquipmentMessage(
            string action,
            ItemInstance equippedItem,
            ItemInstance returnedItem)
        {
            string suffix = returnedItem != null
                ? $" {returnedItem.DisplayName} возвращён в сумку."
                : string.Empty;
            messageStream?.Publish(
                LocalMessageCategory.System,
                $"{action}: {equippedItem.DisplayName}.{suffix}");
        }
    }
}
