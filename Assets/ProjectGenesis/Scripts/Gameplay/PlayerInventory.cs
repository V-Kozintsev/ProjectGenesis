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
        public int Count
        {
            get
            {
                EnsureSlotStorage();
                int occupiedCount = 0;
                foreach (ItemInstance item in items)
                {
                    if (item != null && item.IsValid)
                    {
                        occupiedCount++;
                    }
                }

                return occupiedCount;
            }
        }

        public IReadOnlyList<ItemInstance> Items
        {
            get
            {
                EnsureSlotStorage();
                return items;
            }
        }

        private void Awake()
        {
            EnsureSlotStorage();
        }

        private void OnValidate()
        {
            capacity = Mathf.Max(1, capacity);
            EnsureSlotStorage();
        }

        public bool TryAdd(ItemDefinition item)
        {
            return TryAdd(item, out _);
        }

        public bool TryAdd(ItemDefinition item, out ItemInstance addedInstance)
        {
            if (item == null || FindFirstEmptySlot() < 0)
            {
                addedInstance = null;
                return false;
            }

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
            int emptySlot = FindFirstEmptySlot();
            if (item == null || !item.IsValid || emptySlot < 0 ||
                ContainsInstanceId(item.InstanceId))
            {
                return false;
            }

            items[emptySlot] = item;
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

            EnsureSlotStorage();
            return items.Find(item => item != null && item.InstanceId == instanceId);
        }

        public void RestoreItems(IEnumerable<ItemInstance> restoredItems)
        {
            ClearSlots();

            if (restoredItems != null)
            {
                foreach (ItemInstance item in restoredItems)
                {
                    TryAddInstanceWithoutNotification(item);
                }
            }

            Changed?.Invoke(this);
        }

        public void RestoreSlots(IReadOnlyList<ItemInstance> restoredSlots)
        {
            ClearSlots();
            if (restoredSlots != null)
            {
                int restoredCount = Mathf.Min(capacity, restoredSlots.Count);
                for (int index = 0; index < restoredCount; index++)
                {
                    ItemInstance item = restoredSlots[index];
                    if (item != null && item.IsValid &&
                        !ContainsInstanceId(item.InstanceId))
                    {
                        items[index] = item;
                    }
                }
            }

            Changed?.Invoke(this);
        }

        public ItemInstance GetItemAt(int slotIndex)
        {
            EnsureSlotStorage();
            return slotIndex >= 0 && slotIndex < capacity ? items[slotIndex] : null;
        }

        public int GetSlotIndex(string instanceId)
        {
            if (string.IsNullOrWhiteSpace(instanceId))
            {
                return -1;
            }

            EnsureSlotStorage();
            return items.FindIndex(item => item != null && item.InstanceId == instanceId);
        }

        public bool TryMoveOrSwap(int sourceSlotIndex, int targetSlotIndex)
        {
            EnsureSlotStorage();
            if (sourceSlotIndex < 0 || sourceSlotIndex >= capacity ||
                targetSlotIndex < 0 || targetSlotIndex >= capacity ||
                sourceSlotIndex == targetSlotIndex || items[sourceSlotIndex] == null)
            {
                return false;
            }

            ItemInstance targetItem = items[targetSlotIndex];
            items[targetSlotIndex] = items[sourceSlotIndex];
            items[sourceSlotIndex] = targetItem;
            Changed?.Invoke(this);
            return true;
        }

        private bool TryAddInstanceWithoutNotification(ItemInstance item)
        {
            int emptySlot = FindFirstEmptySlot();
            if (item == null || !item.IsValid || emptySlot < 0 ||
                ContainsInstanceId(item.InstanceId))
            {
                return false;
            }

            items[emptySlot] = item;
            return true;
        }

        private int FindFirstEmptySlot()
        {
            EnsureSlotStorage();
            return items.FindIndex(item => item == null || !item.IsValid);
        }

        private void ClearSlots()
        {
            EnsureSlotStorage();
            for (int index = 0; index < items.Count; index++)
            {
                items[index] = null;
            }
        }

        private void EnsureSlotStorage()
        {
            items ??= new List<ItemInstance>();
            while (items.Count < capacity)
            {
                items.Add(null);
            }

            if (items.Count > capacity)
            {
                items.RemoveRange(capacity, items.Count - capacity);
            }
        }
    }
}
