using ProjectGenesis.Data;
using UnityEngine;

namespace ProjectGenesis.Gameplay
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Health))]
    public sealed class EnemyLootDrop : MonoBehaviour
    {
        [SerializeField] private ItemDefinition item;
        [SerializeField] private Material pickupMaterial;

        private Health health;
        private bool hasDropped;

        private void Awake()
        {
            health = GetComponent<Health>();
            health.Died += HandleDeath;
        }

        private void OnDestroy()
        {
            if (health != null)
            {
                health.Died -= HandleDeath;
            }
        }

        public void Configure(ItemDefinition droppedItem, Material material)
        {
            item = droppedItem;
            pickupMaterial = material;
        }

        private void HandleDeath(Health _)
        {
            if (hasDropped || item == null)
            {
                return;
            }

            hasDropped = true;
            GameObject pickupObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pickupObject.transform.SetPositionAndRotation(
                transform.position + new Vector3(0f, 0.32f, 0f),
                Quaternion.Euler(0f, 45f, 20f));
            pickupObject.transform.localScale = new Vector3(0.18f, 0.55f, 0.12f);

            if (pickupMaterial != null)
            {
                pickupObject.GetComponent<Renderer>().sharedMaterial = pickupMaterial;
            }

            WorldLootPickup pickup = pickupObject.AddComponent<WorldLootPickup>();
            pickup.Initialize(item);
        }
    }
}
