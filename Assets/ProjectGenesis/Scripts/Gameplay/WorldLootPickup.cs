using ProjectGenesis.Data;
using UnityEngine;

namespace ProjectGenesis.Gameplay
{
    [DisallowMultipleComponent]
    public sealed class WorldLootPickup : MonoBehaviour
    {
        [SerializeField] private ItemDefinition item;
        [SerializeField] private ItemInstance instance;
        [SerializeField] private float bobHeight = 0.08f;
        [SerializeField] private float bobSpeed = 2.5f;
        [SerializeField] private float rotationSpeed = 70f;

        private float baseHeight;
        private bool isCollected;

        public ItemDefinition Item => item;
        public ItemInstance Instance => instance;
        public bool IsCollectible => item != null && instance != null && instance.IsValid;

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
            Initialize(ItemInstance.Create(itemDefinition));
        }

        public void Initialize(ItemInstance itemInstance)
        {
            instance = itemInstance != null && itemInstance.IsValid ? itemInstance : null;
            item = instance != null ? instance.Definition : null;
            baseHeight = transform.position.y;
            name = item != null ? $"Loot_{item.DisplayName}" : "Loot_Unknown";
        }

        public bool TryCollect(PlayerInventory inventory)
        {
            if (isCollected || !IsCollectible || inventory == null ||
                !inventory.TryAddInstance(instance))
            {
                return false;
            }

            isCollected = true;
            if (Application.isPlaying)
            {
                Destroy(gameObject);
            }
            else
            {
                DestroyImmediate(gameObject);
            }

            return true;
        }
    }
}
