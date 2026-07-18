using ProjectGenesis.Data;
using UnityEngine;

namespace ProjectGenesis.Gameplay
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Health))]
    public sealed class EnemyLootDrop : MonoBehaviour
    {
        [Header("Regular Loot")]
        [SerializeField] private ItemDefinition item;
        [SerializeField] private Material pickupMaterial;
        [SerializeField, Range(0f, 1f)] private float itemDropChance = 0.35f;

        [Header("Quest Loot")]
        [SerializeField] private string questObjectiveTargetId = "wolf_tail";
        [SerializeField, Range(0f, 1f)] private float questItemDropChance = 0.7f;

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

        public void Configure(
            ItemDefinition droppedItem,
            Material material,
            float regularDropChance,
            string objectiveTargetId,
            float objectiveDropChance)
        {
            item = droppedItem;
            pickupMaterial = material;
            itemDropChance = Mathf.Clamp01(regularDropChance);
            questObjectiveTargetId = objectiveTargetId;
            questItemDropChance = Mathf.Clamp01(objectiveDropChance);
        }

        private void HandleDeath(Health _)
        {
            if (hasDropped)
            {
                return;
            }

            hasDropped = true;

            if (item != null && Random.value <= itemDropChance)
            {
                SpawnRegularItem();
            }

            QuestLog questLog = FindFirstObjectByType<QuestLog>();
            if (questLog != null &&
                questLog.HasActiveObjective(questObjectiveTargetId) &&
                Random.value <= questItemDropChance)
            {
                questLog.ReportObjectiveProgress(questObjectiveTargetId);
            }
        }

        private void SpawnRegularItem()
        {
            GameObject pickupObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pickupObject.transform.SetPositionAndRotation(
                transform.position + new Vector3(-0.24f, 0.32f, 0f),
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
