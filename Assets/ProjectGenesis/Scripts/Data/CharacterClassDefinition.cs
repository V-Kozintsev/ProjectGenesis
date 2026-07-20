using UnityEngine;

namespace ProjectGenesis.Data
{
    [CreateAssetMenu(
        fileName = "SO_Class_New",
        menuName = "Project Genesis/Character Class Definition")]
    public sealed class CharacterClassDefinition : ScriptableObject
    {
        [SerializeField] private string classId = "class.new";
        [SerializeField] private string displayName = "New Class";
        [SerializeField, Min(0)] private int maximumHealthBonus;
        [SerializeField, Min(0)] private int attackPowerBonus;

        public string ClassId => classId;
        public string DisplayName => displayName;
        public int MaximumHealthBonus => maximumHealthBonus;
        public int AttackPowerBonus => attackPowerBonus;
        public bool IsValid =>
            !string.IsNullOrWhiteSpace(classId) &&
            !string.IsNullOrWhiteSpace(displayName) &&
            maximumHealthBonus >= 0 &&
            attackPowerBonus >= 0;

        public void Configure(
            string id,
            string className,
            int healthBonus = 0,
            int attackBonus = 0)
        {
            classId = string.IsNullOrWhiteSpace(id) ? "class.new" : id.Trim();
            displayName = string.IsNullOrWhiteSpace(className) ? "New Class" : className.Trim();
            maximumHealthBonus = Mathf.Max(0, healthBonus);
            attackPowerBonus = Mathf.Max(0, attackBonus);
        }
    }
}
