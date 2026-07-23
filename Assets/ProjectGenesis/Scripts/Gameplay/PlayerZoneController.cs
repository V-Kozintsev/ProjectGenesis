using System;
using ProjectGenesis.Data;
using UnityEngine;

namespace ProjectGenesis.Gameplay
{
    [DefaultExecutionOrder(100)]
    [DisallowMultipleComponent]
    public sealed class PlayerZoneController : MonoBehaviour
    {
        [SerializeField] private WorldZoneDefinition defaultZone;
        [SerializeField] private WorldZoneVolume[] zoneVolumes = Array.Empty<WorldZoneVolume>();

        private LocalMessageStream messageStream;
        private WorldZoneDefinition currentZone;
        private Vector3 lastCheckedPosition;
        private bool hasCheckedPosition;

        public event Action<WorldZoneDefinition> ZoneChanged;

        public WorldZoneDefinition DefaultZone => defaultZone;
        public WorldZoneDefinition CurrentZone => currentZone != null ? currentZone : defaultZone;
        public WorldZoneVolume[] ZoneVolumes => zoneVolumes;
        public bool IsCombatAllowed => CurrentZone == null || CurrentZone.AllowsCombat;

        private void Awake()
        {
            EnsureDependencies();
            RefreshNow(false);
        }

        private void OnEnable()
        {
            RefreshNow(false);
        }

        private void Update()
        {
            Vector3 position = transform.position;
            if (!hasCheckedPosition ||
                (position - lastCheckedPosition).sqrMagnitude >= 0.01f)
            {
                RefreshNow(true);
            }
        }

        public void Configure(
            WorldZoneDefinition fallbackZone,
            WorldZoneVolume[] volumes)
        {
            defaultZone = fallbackZone;
            zoneVolumes = volumes ?? Array.Empty<WorldZoneVolume>();
            hasCheckedPosition = false;

            if (Application.isPlaying)
            {
                RefreshNow(false);
            }
        }

        public void RefreshNow(bool publishMessage = true)
        {
            EnsureDependencies();
            lastCheckedPosition = transform.position;
            hasCheckedPosition = true;
            WorldZoneDefinition nextZone = ResolveZone(lastCheckedPosition);
            if (nextZone == currentZone)
            {
                return;
            }

            currentZone = nextZone;
            ZoneChanged?.Invoke(CurrentZone);

            if (publishMessage && CurrentZone != null)
            {
                messageStream?.Publish(
                    LocalMessageCategory.System,
                    CurrentZone.EnterMessage);
            }
        }

        public bool TryAuthorizeCombat()
        {
            RefreshNow(false);
            if (IsCombatAllowed)
            {
                return true;
            }

            string message = CurrentZone != null
                ? CurrentZone.CombatBlockedMessage
                : "Боевые действия здесь запрещены.";
            messageStream?.Publish(LocalMessageCategory.System, message);
            return false;
        }

        public WorldZoneDefinition ResolveZone(Vector3 worldPosition)
        {
            WorldZoneDefinition resolved = defaultZone;
            int resolvedPriority = int.MinValue;

            if (zoneVolumes == null)
            {
                return resolved;
            }

            foreach (WorldZoneVolume volume in zoneVolumes)
            {
                if (volume == null || volume.Definition == null ||
                    !volume.Contains(worldPosition) || volume.Priority < resolvedPriority)
                {
                    continue;
                }

                resolved = volume.Definition;
                resolvedPriority = volume.Priority;
            }

            return resolved;
        }

        private void EnsureDependencies()
        {
            messageStream ??= GetComponent<LocalMessageStream>();
            zoneVolumes ??= Array.Empty<WorldZoneVolume>();
        }

        private void OnValidate()
        {
            zoneVolumes ??= Array.Empty<WorldZoneVolume>();
        }
    }
}
