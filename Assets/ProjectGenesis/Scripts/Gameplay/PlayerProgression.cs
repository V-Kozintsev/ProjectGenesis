using System;
using UnityEngine;

namespace ProjectGenesis.Gameplay
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Health), typeof(CombatStats))]
    public sealed class PlayerProgression : MonoBehaviour
    {
        [SerializeField, Min(1)] private int level = 1;
        [SerializeField, Min(0)] private int currentExperience;
        [SerializeField, Min(1)] private int maximumLevel = 5;
        [SerializeField, Min(1)] private int baseExperienceRequirement = 100;
        [SerializeField, Min(0)] private int requirementGrowthPerLevel = 50;
        [SerializeField, Min(0)] private int healthPerLevel = 10;
        [SerializeField, Min(0)] private int attackPowerPerLevel = 2;

        private Health health;
        private CombatStats combatStats;

        public event Action<PlayerProgression> ExperienceChanged;
        public event Action<PlayerProgression> LevelChanged;

        public int Level => level;
        public int CurrentExperience => currentExperience;
        public int MaximumLevel => maximumLevel;
        public int ExperienceToNextLevel => level >= maximumLevel
            ? 0
            : baseExperienceRequirement + (level - 1) * requirementGrowthPerLevel;

        private void Awake()
        {
            health = GetComponent<Health>();
            combatStats = GetComponent<CombatStats>();
            ApplyLevelBonuses(false);
        }

        public void AddExperience(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            currentExperience += amount;

            bool leveledUp = false;
            while (level < maximumLevel && currentExperience >= ExperienceToNextLevel)
            {
                currentExperience -= ExperienceToNextLevel;
                level++;
                leveledUp = true;
            }

            if (leveledUp)
            {
                ApplyLevelBonuses(true);
                LevelChanged?.Invoke(this);
            }

            ExperienceChanged?.Invoke(this);
        }

        public void RestoreState(int savedLevel, int savedExperience)
        {
            level = Mathf.Clamp(savedLevel, 1, maximumLevel);
            int maximumExperience = level >= maximumLevel ? 0 : Mathf.Max(0, ExperienceToNextLevel - 1);
            currentExperience = Mathf.Clamp(savedExperience, 0, maximumExperience);
            ApplyLevelBonuses(true);
            LevelChanged?.Invoke(this);
            ExperienceChanged?.Invoke(this);
        }

        private void ApplyLevelBonuses(bool restoreHealth)
        {
            int levelsGained = Mathf.Max(0, level - 1);
            health.SetMaximumHealth(100 + levelsGained * healthPerLevel, restoreHealth);
            combatStats.SetProgressionAttackBonus(levelsGained * attackPowerPerLevel);
        }
    }
}
