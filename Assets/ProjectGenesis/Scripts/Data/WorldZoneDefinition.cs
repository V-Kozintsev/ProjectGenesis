using UnityEngine;

namespace ProjectGenesis.Data
{
    public enum WorldZoneType
    {
        Peaceful,
        Combat
    }

    [CreateAssetMenu(
        fileName = "SO_Zone_NewZone",
        menuName = "Project Genesis/World Zone Definition")]
    public sealed class WorldZoneDefinition : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string zoneId = "zone.new";
        [SerializeField] private string displayName = "New Zone";
        [SerializeField] private WorldZoneType zoneType = WorldZoneType.Combat;

        [Header("Player Feedback")]
        [SerializeField, TextArea] private string enterMessage = "Entered a new zone.";
        [SerializeField, TextArea] private string combatBlockedMessage =
            "Combat is not allowed here.";

        public string ZoneId => zoneId;
        public string DisplayName => displayName;
        public WorldZoneType ZoneType => zoneType;
        public string EnterMessage => enterMessage;
        public string CombatBlockedMessage => combatBlockedMessage;
        public bool AllowsCombat => zoneType == WorldZoneType.Combat;
        public bool IsValid => TryValidate(out _);

        public void Configure(
            string id,
            string zoneName,
            WorldZoneType type,
            string arrivalMessage,
            string blockedCombatMessage = "")
        {
            zoneId = id?.Trim() ?? string.Empty;
            displayName = zoneName?.Trim() ?? string.Empty;
            zoneType = type;
            enterMessage = arrivalMessage?.Trim() ?? string.Empty;
            combatBlockedMessage = blockedCombatMessage?.Trim() ?? string.Empty;
        }

        public bool TryValidate(out string error)
        {
            if (string.IsNullOrWhiteSpace(zoneId))
            {
                error = "Zone id is empty.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(displayName))
            {
                error = "Zone display name is empty.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(enterMessage))
            {
                error = "Zone enter message is empty.";
                return false;
            }

            if (!AllowsCombat && string.IsNullOrWhiteSpace(combatBlockedMessage))
            {
                error = "Peaceful zone combat-blocked message is empty.";
                return false;
            }

            error = string.Empty;
            return true;
        }

        private void OnValidate()
        {
            zoneId = zoneId?.Trim() ?? string.Empty;
            displayName = displayName?.Trim() ?? string.Empty;
            enterMessage = enterMessage?.Trim() ?? string.Empty;
            combatBlockedMessage = combatBlockedMessage?.Trim() ?? string.Empty;
        }
    }
}
