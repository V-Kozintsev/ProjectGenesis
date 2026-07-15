using Unity.AI.Navigation;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace ProjectGenesis.Core
{
    [DefaultExecutionOrder(-1000)]
    [RequireComponent(typeof(NavMeshSurface))]
    public sealed class StarterVillageRuntimeBuilder : MonoBehaviour
    {
        private NavMeshSurface surface;

        private void Awake()
        {
            surface = GetComponent<NavMeshSurface>();
            surface.BuildNavMesh();
            StartCoroutine(EnableSceneAgentsAfterNavMeshReady());
        }

        private static IEnumerator EnableSceneAgentsAfterNavMeshReady()
        {
            yield return null;
            EnableSceneAgents();
        }

        private static void EnableSceneAgents()
        {
            NavMeshAgent[] agents = FindObjectsByType<NavMeshAgent>(FindObjectsSortMode.None);
            foreach (NavMeshAgent agent in agents)
            {
                if (!agent.enabled)
                {
                    TryEnableAgent(agent);
                }
            }
        }

        private static void TryEnableAgent(NavMeshAgent agent)
        {
            if (NavMesh.SamplePosition(agent.transform.position, out NavMeshHit hit, 3f, NavMesh.AllAreas))
            {
                agent.transform.position = hit.position;
                agent.enabled = true;
            }
            else
            {
                Debug.LogWarning($"Could not place NavMeshAgent '{agent.name}' on the runtime NavMesh.");
            }
        }
    }
}
