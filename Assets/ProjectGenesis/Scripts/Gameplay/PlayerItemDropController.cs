using ProjectGenesis.Data;
using UnityEngine;
using UnityEngine.AI;

namespace ProjectGenesis.Gameplay
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerInventory), typeof(Health))]
    public sealed class PlayerItemDropController : MonoBehaviour
    {
        [SerializeField] private Material pickupMaterial;
        [SerializeField, Min(0.5f)] private float dropDistance = 1.25f;
        [SerializeField, Min(0.1f)] private float pickupHeight = 0.32f;

        private PlayerInventory inventory;
        private Health health;
        private LocalMessageStream messageStream;

        public Material PickupMaterial => pickupMaterial;

        public void Configure(Material material, float distance = 1.25f)
        {
            pickupMaterial = material;
            dropDistance = Mathf.Max(0.5f, distance);
        }

        private void Awake()
        {
            EnsureDependencies();
        }

        public bool TryDrop(ItemInstance item, out WorldLootPickup pickup)
        {
            pickup = null;
            if (!CanRemove(item))
            {
                return false;
            }

            GameObject pickupObject = CreatePickupObject(item);
            if (!inventory.TryRemoveInstance(item))
            {
                DestroyObject(pickupObject);
                return false;
            }

            pickup = pickupObject.GetComponent<WorldLootPickup>();
            if (pickup == null)
            {
                return false;
            }

            messageStream?.Publish(
                LocalMessageCategory.Loot,
                $"Выброшено на землю: {item.DisplayName}.");
            return true;
        }

        public bool TryDestroy(ItemInstance item)
        {
            if (!CanRemove(item) || !inventory.TryRemoveInstance(item))
            {
                return false;
            }

            messageStream?.Publish(
                LocalMessageCategory.System,
                $"Удалено безвозвратно: {item.DisplayName}.");
            return true;
        }

        private bool CanRemove(ItemInstance item)
        {
            EnsureDependencies();
            return item != null && item.IsValid && inventory != null &&
                   inventory.Contains(item) && health != null && !health.IsDead;
        }

        private GameObject CreatePickupObject(ItemInstance item)
        {
            GameObject pickupObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pickupObject.transform.SetPositionAndRotation(
                ResolveDropPosition(),
                Quaternion.Euler(0f, 45f, 20f));
            pickupObject.transform.localScale = GetPickupScale(item.ItemType);

            if (pickupMaterial != null)
            {
                pickupObject.GetComponent<Renderer>().sharedMaterial = pickupMaterial;
            }

            WorldLootPickup pickup = pickupObject.AddComponent<WorldLootPickup>();
            pickup.Initialize(item);
            return pickupObject;
        }

        private Vector3 ResolveDropPosition()
        {
            Vector3 candidate = transform.position + transform.forward * dropDistance;
            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                candidate = hit.position;
            }

            candidate.y += pickupHeight;
            return candidate;
        }

        private static Vector3 GetPickupScale(ItemType itemType)
        {
            return itemType switch
            {
                ItemType.Weapon => new Vector3(0.18f, 0.55f, 0.12f),
                ItemType.Armor => new Vector3(0.42f, 0.38f, 0.16f),
                ItemType.Consumable => new Vector3(0.24f, 0.34f, 0.24f),
                _ => Vector3.one * 0.3f
            };
        }

        private void EnsureDependencies()
        {
            inventory ??= GetComponent<PlayerInventory>();
            health ??= GetComponent<Health>();
            messageStream ??= GetComponent<LocalMessageStream>();
        }

        private static void DestroyObject(GameObject target)
        {
            if (target == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                Object.Destroy(target);
            }
            else
            {
                Object.DestroyImmediate(target);
            }
        }
    }
}
