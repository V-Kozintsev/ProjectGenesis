using ProjectGenesis.Data;
using UnityEngine;

namespace ProjectGenesis.Gameplay
{
    [DisallowMultipleComponent]
    public sealed class WorldLootPickup : MonoBehaviour
    {
        [SerializeField] private ItemDefinition item;
        [SerializeField] private float bobHeight = 0.08f;
        [SerializeField] private float bobSpeed = 2.5f;
        [SerializeField] private float rotationSpeed = 70f;

        private float baseHeight;
        private bool isCollected;

        public ItemDefinition Item => item;

        private void Awake()
        {
            baseHeight = transform.position.y;
        }

        private void Update()
        {
            Vector3 position = transform.position;
            position.y = baseHeight + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            transform.position = position;
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
        }

        public void Initialize(ItemDefinition itemDefinition)
        {
            item = itemDefinition;
            baseHeight = transform.position.y;
            name = item != null ? $"Loot_{item.DisplayName}" : "Loot_Unknown";
        }

        public bool TryCollect(PlayerInventory inventory)
        {
            if (isCollected || item == null || inventory == null || !inventory.TryAdd(item))
            {
                return false;
            }

            isCollected = true;
            Destroy(gameObject);
            return true;
        }
    }
}
