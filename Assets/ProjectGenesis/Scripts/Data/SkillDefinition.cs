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
        [SerializeField] private SkillClassRequirement classRequirement = SkillClassRequirement.Any;
        [SerializeField] private SkillTargetType targetType = SkillTargetType.Enemy;
        [SerializeField, Min(1)] private int damage = 10;
        [SerializeField, Min(0.25f)] private float range = 1.4f;
        [SerializeField, Min(0.1f)] private float cooldown = 3f;

        public string SkillId => skillId;
        public string DisplayName => displayName;
        public SkillClassRequirement ClassRequirement => classRequirement;
        public SkillTargetType TargetType => targetType;
        public int Damage => damage;
        public float Range => range;
        public float Cooldown => cooldown;

        public bool IsValid =>
            !string.IsNullOrWhiteSpace(skillId) &&
            !string.IsNullOrWhiteSpace(displayName) &&
            damage > 0 &&
            range > 0f &&
            cooldown > 0f;

        public void Configure(
            string id,
            string skillName,
            SkillClassRequirement requiredClass,
            SkillTargetType requiredTarget,
            int directDamage,
            float useRange,
            float reuseDelay)
        {
            skillId = string.IsNullOrWhiteSpace(id) ? "skill.id" : id;
            displayName = string.IsNullOrWhiteSpace(skillName) ? skillId : skillName;
            classRequirement = requiredClass;
            targetType = requiredTarget;
            damage = Mathf.Max(1, directDamage);
            range = Mathf.Max(0.25f, useRange);
            cooldown = Mathf.Max(0.1f, reuseDelay);
        }
    }
}
