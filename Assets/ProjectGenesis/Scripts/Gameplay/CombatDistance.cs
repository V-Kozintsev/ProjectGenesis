using UnityEngine;
using UnityEngine.AI;

namespace ProjectGenesis.Gameplay
{
    public static class CombatDistance
    {
        public static float GetPlanarSurfaceDistance(
            Transform first,
            Collider firstCollider,
            Transform second,
            Collider secondCollider)
        {
            if (first == null || second == null)
            {
                return float.MaxValue;
            }

            Vector3 offset = second.position - first.position;
            offset.y = 0f;

            float firstRadius = GetRadius(first, firstCollider);
            float secondRadius = GetRadius(second, secondCollider);
            return Mathf.Max(0f, offset.magnitude - firstRadius - secondRadius);
        }

        private static float GetRadius(Transform owner, Collider collider)
        {
            if (collider != null)
            {
                Bounds bounds = collider.bounds;
                return Mathf.Max(bounds.extents.x, bounds.extents.z);
            }

            NavMeshAgent agent = owner.GetComponent<NavMeshAgent>();
            return agent != null ? agent.radius : 0f;
        }
    }
}
