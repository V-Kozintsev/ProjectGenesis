using ProjectGenesis.Data;
using UnityEngine;
using UnityEngine.AI;

namespace ProjectGenesis.Gameplay
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NavMeshAgent), typeof(PlayerInventory))]
    public sealed class PlayerLootController : MonoBehaviour
    {
        [SerializeField, Min(0.25f)] private float pickupDistance = 0.85f;
        [SerializeField, Min(1f)] private float maximumClickDistance = 30f;

        private NavMeshAgent agent;
        private PlayerInventory inventory;
        private WorldLootPickup target;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            inventory = GetComponent<PlayerInventory>();
        }

        private void Update()
        {
            if (target == null)
            {
                return;
            }

            float distance = GetPlanarDistance(transform.position, target.transform.position);
            if (distance > maximumClickDistance)
            {
                CancelLootAction();
                return;
            }

            if (distance <= pickupDistance)
            {
                target.TryCollect(inventory);
                target = null;
                StopAgent();
                return;
            }

            MoveTowardTarget();
        }

        public void HandleLootClick(WorldLootPickup pickup)
        {
            if (pickup == null || pickup.Item == null)
            {
                return;
            }

            target = pickup;
            MoveTowardTarget();
        }

        public void CancelLootAction()
        {
            target = null;
            StopAgent();
        }

        private void MoveTowardTarget()
        {
            if (target == null || agent == null || !agent.enabled || !agent.isOnNavMesh)
            {
                return;
            }

            if (NavMesh.SamplePosition(target.transform.position, out NavMeshHit hit, 1.5f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
        }

        private void StopAgent()
        {
            if (agent != null && agent.enabled && agent.isOnNavMesh)
            {
                agent.ResetPath();
            }
        }

        private static float GetPlanarDistance(Vector3 first, Vector3 second)
        {
            Vector3 offset = second - first;
            offset.y = 0f;
            return offset.magnitude;
        }
    }
}
