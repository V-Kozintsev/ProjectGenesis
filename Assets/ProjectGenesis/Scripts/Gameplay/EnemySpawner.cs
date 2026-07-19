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
        [SerializeField, Min(1f)] private float respawnDelay = 12f;

        private EnemyBrain currentEnemy;
        private Coroutine respawnRoutine;

        public void Configure(GameObject prefab, Transform playerTransform, float delay)
        {
            enemyPrefab = prefab;
            player = playerTransform;
            respawnDelay = Mathf.Max(1f, delay);
        }

        private IEnumerator Start()
        {
            yield return null;
            SpawnEnemy();
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
            instance.name = $"Enemy_YoungWolf_{name}";
            currentEnemy = instance.GetComponent<EnemyBrain>();

            if (currentEnemy == null)
            {
                Destroy(instance);
                return;
            }

            currentEnemy.SetPlayer(player);
            currentEnemy.Died += HandleEnemyDied;
            StartCoroutine(EnableAgentAfterNavMeshReady(currentEnemy));
        }

        private void HandleEnemyDied(EnemyBrain enemy)
        {
            if (enemy != currentEnemy)
            {
                return;
            }

            currentEnemy.Died -= HandleEnemyDied;
            currentEnemy = null;

            if (respawnRoutine != null)
            {
                StopCoroutine(respawnRoutine);
            }

            respawnRoutine = StartCoroutine(RespawnAfterDelay());
        }

        private IEnumerator RespawnAfterDelay()
        {
            yield return new WaitForSeconds(respawnDelay);
            respawnRoutine = null;
            SpawnEnemy();
        }

        private static IEnumerator EnableAgentAfterNavMeshReady(EnemyBrain enemy)
        {
            yield return null;

            if (enemy == null)
            {
                yield break;
            }

            NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
            if (agent == null)
            {
                yield break;
            }

            if (agent.enabled)
            {
                enemy.SetHomePosition(agent.transform.position);
                yield break;
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
