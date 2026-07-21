using System;
using System.Collections.Generic;
using ProjectGenesis.Data;
using UnityEngine;

namespace ProjectGenesis.Gameplay
{
    [DisallowMultipleComponent]
    public sealed class PlayerInventory : MonoBehaviour
    {
        [SerializeField, Min(1)] private int capacity = 8;
        [SerializeField] private List<ItemInstance> items = new();

        public event Action<PlayerInventory> Changed;

        public int Capacity => capacity;
        public int Count => items.Count;
        public IReadOnlyList<ItemInstance> Items => items;

        public bool TryAdd(ItemDefinition item)
        {
            return TryAdd(item, out _);
        }

        public bool TryAdd(ItemDefinition item, out ItemInstance addedInstance)
        {
            addedInstance = ItemInstance.Create(item);
            if (!TryAddInstance(addedInstance))
            {
                addedInstance = null;
                return false;
            }

            return true;
        }

        public bool TryAddInstance(ItemInstance item)
        {
            if (item == null || !item.IsValid || items.Count >= capacity ||
                ContainsInstanceId(item.InstanceId))
            {
                return false;
            }

            items.Add(item);
            Changed?.Invoke(this);
            return true;
        }

        public bool Contains(ItemInstance item)
        {
            return item != null && ContainsInstanceId(item.InstanceId);
        }

        public bool ContainsInstanceId(string instanceId)
        {
            return FindByInstanceId(instanceId) != null;
        }

        public ItemInstance FindByInstanceId(string instanceId)
        {
            if (string.IsNullOrWhiteSpace(instanceId))
            {
                return null;
            }

            return items.Find(item => item != null && item.InstanceId == instanceId);
        }

        public void RestoreItems(IEnumerable<ItemInstance> restoredItems)
        {
            items.Clear();

            if (restoredItems != null)
            {
                foreach (ItemInstance item in restoredItems)
                {
                    TryAddInstanceWithoutNotification(item);
                }
            }

            Changed?.Invoke(this);
        }

        private bool TryAddInstanceWithoutNotification(ItemInstance item)
        {
            if (item == null || !item.IsValid || items.Count >= capacity ||
                ContainsInstanceId(item.InstanceId))
            {
                return false;
            }

            items.Add(item);
            return true;
        }
    }
}
