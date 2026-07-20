using UnityEngine;

namespace ProjectGenesis.Data
{
    public enum SkillTargetType
    {
        Enemy
    }

    public enum SkillClassRequirement
    {
        Any,
        Warrior,
        Mage,
        Rogue
    }

    [CreateAssetMenu(
        fileName = "SO_Skill_NewSkill",
        menuName = "Project Genesis/Skill Definition")]
    public sealed class SkillDefinition : ScriptableObject
    {
        [SerializeField] private string skillId = "skill.id";
        [SerializeField] private string displayName = "New Skill";
        [SerializeField, TextArea] private string description = "Skill description.";
        [SerializeField] private SkillClassRequirement classRequirement = SkillClassRequirement.Any;
        [SerializeField] private SkillTargetType targetType = SkillTargetType.Enemy;
        [SerializeField, Min(0.1f)] private float attackPowerMultiplier = 1f;
        [SerializeField, Min(0.25f)] private float range = 1.4f;
        [SerializeField, Min(0.1f)] private float cooldown = 3f;

        public string SkillId => skillId;
        public string DisplayName => displayName;
        public string Description => description;
        public SkillClassRequirement ClassRequirement => classRequirement;
        public SkillTargetType TargetType => targetType;
        public float AttackPowerMultiplier => attackPowerMultiplier;
        public float Range => range;
        public float Cooldown => cooldown;

        public bool IsValid =>
            !string.IsNullOrWhiteSpace(skillId) &&
            !string.IsNullOrWhiteSpace(displayName) &&
            !string.IsNullOrWhiteSpace(description) &&
            attackPowerMultiplier > 0f &&
            range > 0f &&
            cooldown > 0f;

        public void Configure(
            string id,
            string skillName,
            SkillClassRequirement requiredClass,
            SkillTargetType requiredTarget,
            float powerMultiplier,
            float useRange,
            float reuseDelay,
            string skillDescription = "Skill description.")
        {
            skillId = string.IsNullOrWhiteSpace(id) ? "skill.id" : id;
            displayName = string.IsNullOrWhiteSpace(skillName) ? skillId : skillName;
            description = string.IsNullOrWhiteSpace(skillDescription)
                ? "Skill description."
                : skillDescription.Trim();
            classRequirement = requiredClass;
            targetType = requiredTarget;
            attackPowerMultiplier = Mathf.Max(0.1f, powerMultiplier);
            range = Mathf.Max(0.25f, useRange);
            cooldown = Mathf.Max(0.1f, reuseDelay);
        }
    }
}
