using UnityEngine;

namespace ProjectGenesis.Data
{
    [CreateAssetMenu(
        fileName = "SO_Race_New",
        menuName = "Project Genesis/Character Race Definition")]
    public sealed class CharacterRaceDefinition : ScriptableObject
    {
        [SerializeField] private string raceId = "race.new";
        [SerializeField] private string displayName = "New Race";

        public string RaceId => raceId;
        public string DisplayName => displayName;
        public bool IsValid =>
            !string.IsNullOrWhiteSpace(raceId) &&
            !string.IsNullOrWhiteSpace(displayName);

        public void Configure(string id, string raceName)
        {
            raceId = string.IsNullOrWhiteSpace(id) ? "race.new" : id.Trim();
            displayName = string.IsNullOrWhiteSpace(raceName) ? "New Race" : raceName.Trim();
        }
    }
}
