using ProjectGenesis.Data;
using UnityEngine;

namespace ProjectGenesis.Gameplay
{
    public enum ItemUseResult
    {
        Success,
        InvalidItem,
        NotConsumable,
        Dead,
        FullHealth
    }

    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerInventory), typeof(Health))]
    public sealed class PlayerItemUseController : MonoBehaviour
    {
        private PlayerInventory inventory;
        private Health health;

        public Health Health => health != null ? health : GetComponent<Health>();

        private void Awake()
        {
            inventory = GetComponent<PlayerInventory>();
            health = GetComponent<Health>();
        }

        public bool TryUse(ItemInstance item, out ItemUseResult result)
        {
            EnsureDependencies();
            if (item == null || !item.IsValid || inventory == null ||
                !inventory.Contains(item))
            {
                result = ItemUseResult.InvalidItem;
                return false;
            }

            if (item.ItemType != ItemType.Consumable || item.HealingAmount <= 0)
            {
                result = ItemUseResult.NotConsumable;
                return false;
            }

            if (health == null || health.IsDead)
            {
                result = ItemUseResult.Dead;
                return false;
            }

            if (health.CurrentHealth >= health.MaximumHealth)
            {
                result = ItemUseResult.FullHealth;
                return false;
            }

            if (!health.Heal(item.HealingAmount) || !inventory.TryRemoveInstance(item))
            {
                result = ItemUseResult.InvalidItem;
                return false;
            }

            result = ItemUseResult.Success;
            return true;
        }

        private void EnsureDependencies()
        {
            inventory ??= GetComponent<PlayerInventory>();
            health ??= GetComponent<Health>();
        }
    }
}
