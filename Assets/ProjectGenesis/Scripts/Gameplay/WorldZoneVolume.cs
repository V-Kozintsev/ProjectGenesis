using ProjectGenesis.Data;
using UnityEngine;

namespace ProjectGenesis.Gameplay
{
    [DisallowMultipleComponent]
    public sealed class WorldZoneVolume : MonoBehaviour
    {
        [SerializeField] private WorldZoneDefinition definition;
        [SerializeField] private Vector2 size = new(10f, 10f);
        [SerializeField, Min(0f)] private float edgePadding;
        [SerializeField] private int priority;

        public WorldZoneDefinition Definition => definition;
        public Vector2 Size => size;
        public float EdgePadding => edgePadding;
        public int Priority => priority;

        public void Configure(
            WorldZoneDefinition zoneDefinition,
            Vector2 zoneSize,
            float padding,
            int zonePriority)
        {
            definition = zoneDefinition;
            size = new Vector2(
                Mathf.Max(0.5f, zoneSize.x),
                Mathf.Max(0.5f, zoneSize.y));
            edgePadding = Mathf.Max(0f, padding);
            priority = zonePriority;
        }

        public bool Contains(Vector3 worldPosition)
        {
            Vector3 localPosition = transform.InverseTransformPoint(worldPosition);
            Vector2 halfSize = GetUsableHalfSize();
            return Mathf.Abs(localPosition.x) <= halfSize.x &&
                   Mathf.Abs(localPosition.z) <= halfSize.y;
        }

        public bool TryValidate(out string error)
        {
            if (definition == null || !definition.IsValid)
            {
                error = "Zone definition is missing or invalid.";
                return false;
            }

            Vector2 halfSize = size * 0.5f;
            if (halfSize.x - edgePadding <= 0f || halfSize.y - edgePadding <= 0f)
            {
                error = "Zone edge padding consumes the whole volume.";
                return false;
            }

            error = string.Empty;
            return true;
        }

        private Vector2 GetUsableHalfSize()
        {
            Vector2 halfSize = size * 0.5f;
            return new Vector2(
                Mathf.Max(0.05f, halfSize.x - edgePadding),
                Mathf.Max(0.05f, halfSize.y - edgePadding));
        }

        private void OnDrawGizmosSelected()
        {
            Matrix4x4 previousMatrix = Gizmos.matrix;
            Color previousColor = Gizmos.color;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = definition != null && definition.AllowsCombat
                ? new Color(0.9f, 0.3f, 0.2f, 0.9f)
                : new Color(0.25f, 0.75f, 1f, 0.9f);
            Vector2 halfSize = GetUsableHalfSize();
            Gizmos.DrawWireCube(
                Vector3.zero,
                new Vector3(halfSize.x * 2f, 0.1f, halfSize.y * 2f));
            Gizmos.matrix = previousMatrix;
            Gizmos.color = previousColor;
        }
    }
}
