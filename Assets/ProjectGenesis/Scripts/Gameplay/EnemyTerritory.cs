using UnityEngine;

namespace ProjectGenesis.Gameplay
{
    [DisallowMultipleComponent]
    public sealed class EnemyTerritory : MonoBehaviour
    {
        [SerializeField] private Vector2 size = new(15.6f, 9.2f);
        [SerializeField, Min(0f)] private float edgePadding = 0.2f;

        public Vector2 Size => size;
        public float EdgePadding => edgePadding;

        public void Configure(Vector2 territorySize, float padding)
        {
            size = new Vector2(
                Mathf.Max(0.5f, territorySize.x),
                Mathf.Max(0.5f, territorySize.y));
            edgePadding = Mathf.Max(0f, padding);
        }

        public bool Contains(Vector3 worldPosition)
        {
            Vector3 localPosition = transform.InverseTransformPoint(worldPosition);
            Vector2 halfSize = GetUsableHalfSize();

            return Mathf.Abs(localPosition.x) <= halfSize.x &&
                   Mathf.Abs(localPosition.z) <= halfSize.y;
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
            Gizmos.color = new Color(0.25f, 0.9f, 0.45f, 0.9f);
            Vector2 usableHalfSize = GetUsableHalfSize();
            Gizmos.DrawWireCube(
                Vector3.zero,
                new Vector3(usableHalfSize.x * 2f, 0.1f, usableHalfSize.y * 2f));

            Gizmos.matrix = previousMatrix;
            Gizmos.color = previousColor;
        }
    }
}

