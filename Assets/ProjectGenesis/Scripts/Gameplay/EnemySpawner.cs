using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace ProjectGenesis.Gameplay
{
    [DisallowMultipleComponent]
    public sealed class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private Transform player;
        [SerializeField] private EnemyTerritory territory;
        [SerializeField, Min(1f)] private float respawnDelay = 12f;

        private EnemyBrain currentEnemy;
        private float respawnReadyTime = -1f;

        public float RespawnDelay => respawnDelay;
        public EnemyTerritory Territory => territory;
        public GameObject EnemyPrefab => enemyPrefab;
        public EnemyBrain CurrentEnemy => currentEnemy;
        public bool IsRespawnPending => respawnReadyTime >= 0f;

        public void Configure(
            GameObject prefab,
            Transform playerTransform,
            float delay,
            EnemyTerritory enemyTerritory = null)
        {
            enemyPrefab = prefab;
            player = playerTransform;
            respawnDelay = Mathf.Max(1f, delay);
            territory = enemyTerritory;
        }

        public void TickRespawn(float currentTime)
        {
            if (respawnReadyTime < 0f || currentTime < respawnReadyTime)
            {
                return;
            }

            respawnReadyTime = -1f;
            SpawnEnemy();
        }

        private void Start()
        {
            SpawnEnemy();
        }

        private void Update()
        {
            TickRespawn(Time.time);
        }

        private void OnDestroy()
        {
            if (currentEnemy != null)
            {
                currentEnemy.Died -= HandleEnemyDied;
            }
        }

        private void SpawnEnemy()
        {
            if (enemyPrefab == null || currentEnemy != null)
            {
                return;
            }

            GameObject instance = Instantiate(enemyPrefab, transform.position, transform.rotation);
            instance.name = $"{enemyPrefab.name}_{name}";
            currentEnemy = instance.GetComponent<EnemyBrain>();

            if (currentEnemy == null)
            {
                Destroy(instance);
                return;
            }

            currentEnemy.SetPlayer(player);
            currentEnemy.SetTerritory(territory);
            currentEnemy.Died += HandleEnemyDied;

            if (Application.isPlaying)
            {
                StartCoroutine(EnableAgentAfterNavMeshReady(currentEnemy));
            }
            else
            {
                PlaceAgentOnNavMesh(currentEnemy);
            }
        }

        private void HandleEnemyDied(EnemyBrain enemy)
        {
            if (enemy != currentEnemy)
            {
                return;
            }

            currentEnemy.Died -= HandleEnemyDied;
            currentEnemy = null;

            respawnReadyTime = Time.time + respawnDelay;
        }

        private static IEnumerator EnableAgentAfterNavMeshReady(EnemyBrain enemy)
        {
            yield return null;

            PlaceAgentOnNavMesh(enemy);
        }

        private static void PlaceAgentOnNavMesh(EnemyBrain enemy)
        {
            if (enemy == null)
            {
                return;
            }

            NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
            if (agent == null)
            {
                return;
            }

            if (agent.enabled)
            {
                enemy.SetHomePosition(agent.transform.position);
                return;
            }

            if (NavMesh.SamplePosition(agent.transform.position, out NavMeshHit hit, 3f, NavMesh.AllAreas))
            {
                agent.transform.position = hit.position;
                enemy.SetHomePosition(hit.position);
                agent.enabled = true;
            }
            else
            {
                Debug.LogWarning($"Could not place respawned enemy '{agent.name}' on the runtime NavMesh.");
            }
        }
    }
}
