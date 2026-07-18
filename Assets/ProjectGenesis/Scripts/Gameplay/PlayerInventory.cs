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
        [SerializeField] private List<ItemDefinition> items = new();

        public event Action<PlayerInventory> Changed;

        public int Capacity => capacity;
        public int Count => items.Count;
        public IReadOnlyList<ItemDefinition> Items => items;

        public bool TryAdd(ItemDefinition item)
        {
            if (item == null || items.Count >= capacity)
            {
                return false;
            }

            items.Add(item);
            Changed?.Invoke(this);
            return true;
        }

        public bool Contains(ItemDefinition item)
        {
            return item != null && items.Contains(item);
        }
    }
}
