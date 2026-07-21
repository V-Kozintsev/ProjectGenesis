using ProjectGenesis.Data;
using UnityEngine;

namespace ProjectGenesis.Gameplay
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Health))]
    public sealed class EnemyLootDrop : MonoBehaviour
    {
        [Header("Regular Loot")]
        [SerializeField] private LootTableDefinition lootTable;
        [SerializeField] private Material pickupMaterial;

        [Header("Quest Loot")]
        [SerializeField] private string questObjectiveTargetId = "wolf_tail";
        [SerializeField, Range(0f, 1f)] private float questItemDropChance = 0.7f;

        private Health health;
        private bool hasDropped;

        public LootTableDefinition LootTable => lootTable;
        public string QuestObjectiveTargetId => questObjectiveTargetId;
        public float QuestItemDropChance => questItemDropChance;

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
            LootTableDefinition regularLootTable,
            Material material,
            string objectiveTargetId,
            float objectiveDropChance)
        {
            lootTable = regularLootTable;
            pickupMaterial = material;
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

            if (lootTable != null && lootTable.TryRoll(Random.value, out LootTableEntry regularLoot))
            {
                SpawnRegularItem(regularLoot.Item);
            }

            QuestLog questLog = FindFirstObjectByType<QuestLog>();
            if (questLog != null &&
                questLog.HasActiveObjective(questObjectiveTargetId) &&
                Random.value <= questItemDropChance)
            {
                questLog.ReportObjectiveProgress(questObjectiveTargetId);
            }
        }

        private void SpawnRegularItem(ItemDefinition item)
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
