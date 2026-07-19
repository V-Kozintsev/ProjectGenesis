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

        [Header("Death Penalty")]
        [SerializeField, Range(0f, 1f)] private float deathExperienceLossRate = 0.1f;
        [SerializeField, Min(0)] private int minimumDeathExperienceLoss = 10;

        private Health health;
        private CombatStats combatStats;

        public event Action<PlayerProgression> ExperienceChanged;
        public event Action<PlayerProgression> LevelChanged;

        public int Level => level;
        public int CurrentExperience => currentExperience;
        public int MaximumLevel => maximumLevel;
        public float DeathExperienceLossRate => deathExperienceLossRate;
        public int MinimumDeathExperienceLoss => minimumDeathExperienceLoss;
        public int ExperienceToNextLevel => level >= maximumLevel
            ? 0
            : GetExperienceRequirement(level);

        private void Awake()
        {
            CacheDependencies();
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

        public int ApplyDeathPenalty()
        {
            int calculatedLoss = Mathf.CeilToInt(
                GetExperienceRequirement(level) * deathExperienceLossRate);
            int requestedLoss = Mathf.Max(minimumDeathExperienceLoss, calculatedLoss);
            return RemoveExperience(requestedLoss);
        }

        public int RemoveExperience(int amount)
        {
            if (amount <= 0 || (level <= 1 && currentExperience <= 0))
            {
                return 0;
            }

            int previousLevel = level;
            int remainingLoss = amount;

            while (remainingLoss > 0)
            {
                if (currentExperience >= remainingLoss)
                {
                    currentExperience -= remainingLoss;
                    remainingLoss = 0;
                    break;
                }

                remainingLoss -= currentExperience;
                currentExperience = 0;

                if (level <= 1)
                {
                    break;
                }

                level--;
                currentExperience = GetExperienceRequirement(level);
            }

            int actualLoss = amount - remainingLoss;
            if (level != previousLevel)
            {
                ApplyLevelBonuses(false);
                LevelChanged?.Invoke(this);
            }

            if (actualLoss > 0)
            {
                ExperienceChanged?.Invoke(this);
            }

            return actualLoss;
        }

        public void ConfigureDeathPenalty(float lossRate, int minimumLoss)
        {
            deathExperienceLossRate = Mathf.Clamp01(lossRate);
            minimumDeathExperienceLoss = Mathf.Max(0, minimumLoss);
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
            CacheDependencies();
            int levelsGained = Mathf.Max(0, level - 1);
            health.SetMaximumHealth(100 + levelsGained * healthPerLevel, restoreHealth);
            combatStats.SetProgressionAttackBonus(levelsGained * attackPowerPerLevel);
        }

        private void CacheDependencies()
        {
            health ??= GetComponent<Health>();
            combatStats ??= GetComponent<CombatStats>();
        }

        private int GetExperienceRequirement(int sourceLevel)
        {
            return baseExperienceRequirement +
                   Mathf.Max(0, sourceLevel - 1) * requirementGrowthPerLevel;
        }
    }
}
