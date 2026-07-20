using System;
using UnityEngine;

namespace ProjectGenesis.Gameplay
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Health), typeof(CombatStats))]
    [RequireComponent(typeof(PlayerIdentity))]
    public sealed class PlayerProgression : MonoBehaviour
    {
        [SerializeField, Min(1)] private int level = 1;
        [SerializeField, Min(0)] private int currentExperience;
        [SerializeField, Min(1)] private int maximumLevel = 5;
        [SerializeField, Min(1)] private int baseExperienceRequirement = 100;
        [SerializeField, Min(0)] private int requirementGrowthPerLevel = 50;
        [SerializeField, Min(1)] private int baseMaximumHealth = 90;
        [SerializeField, Min(0)] private int healthPerLevel = 10;
        [SerializeField, Min(0)] private int attackPowerPerLevel = 2;

        [Header("Death Penalty")]
        [SerializeField, Range(0f, 1f)] private float deathExperienceLossRate = 0.1f;
        [SerializeField, Min(0)] private int minimumDeathExperienceLoss = 10;

        [Header("Enemy Experience Scaling")]
        [SerializeField, Range(0f, 1f)] private float lowerLevelPenaltyPerLevel = 0.25f;
        [SerializeField, Range(0f, 1f)] private float minimumEnemyRewardMultiplier = 0.1f;
        [SerializeField, Range(0f, 1f)] private float higherLevelBonusPerLevel = 0.1f;
        [SerializeField, Min(1f)] private float maximumEnemyRewardMultiplier = 1.5f;

        private Health health;
        private CombatStats combatStats;
        private PlayerIdentity identity;

        public event Action<PlayerProgression> ExperienceChanged;
        public event Action<PlayerProgression> LevelChanged;

        public int Level => level;
        public int CurrentExperience => currentExperience;
        public int MaximumLevel => maximumLevel;
        public int BaseMaximumHealth => baseMaximumHealth;
        public int ClassHealthBonus => identity != null && identity.CharacterClass != null
            ? identity.CharacterClass.MaximumHealthBonus
            : 0;
        public int ProgressionHealthBonus => Mathf.Max(0, level - 1) * healthPerLevel;
        public int HealthPerLevel => healthPerLevel;
        public int AttackPowerPerLevel => attackPowerPerLevel;
        public float DeathExperienceLossRate => deathExperienceLossRate;
        public int MinimumDeathExperienceLoss => minimumDeathExperienceLoss;
        public float LowerLevelPenaltyPerLevel => lowerLevelPenaltyPerLevel;
        public float MinimumEnemyRewardMultiplier => minimumEnemyRewardMultiplier;
        public float HigherLevelBonusPerLevel => higherLevelBonusPerLevel;
        public float MaximumEnemyRewardMultiplier => maximumEnemyRewardMultiplier;
        public int ExperienceToNextLevel => level >= maximumLevel
            ? 0
            : GetExperienceRequirement(level);

        private void Awake()
        {
            CacheDependencies();
            identity.Changed += HandleIdentityChanged;
            ApplyLevelBonuses(true);
        }

        private void OnDestroy()
        {
            if (identity != null)
            {
                identity.Changed -= HandleIdentityChanged;
            }
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

        public int AddEnemyExperience(int enemyLevel, int baseReward)
        {
            int reward = CalculateEnemyExperience(enemyLevel, baseReward);
            AddExperience(reward);
            return reward;
        }

        public int CalculateEnemyExperience(int enemyLevel, int baseReward)
        {
            if (baseReward <= 0)
            {
                return 0;
            }

            int levelDifference = Mathf.Max(1, enemyLevel) - level;
            float multiplier;

            if (levelDifference < 0)
            {
                multiplier = Mathf.Max(
                    minimumEnemyRewardMultiplier,
                    1f + levelDifference * lowerLevelPenaltyPerLevel);
            }
            else
            {
                multiplier = Mathf.Min(
                    maximumEnemyRewardMultiplier,
                    1f + levelDifference * higherLevelBonusPerLevel);
            }

            return Mathf.Max(0, Mathf.RoundToInt(baseReward * multiplier));
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

        public void ConfigureEnemyExperienceScaling(
            float lowerPenaltyPerLevel,
            float minimumRewardMultiplier,
            float higherBonusPerLevel,
            float maximumRewardMultiplier)
        {
            lowerLevelPenaltyPerLevel = Mathf.Clamp01(lowerPenaltyPerLevel);
            minimumEnemyRewardMultiplier = Mathf.Clamp01(minimumRewardMultiplier);
            higherLevelBonusPerLevel = Mathf.Clamp01(higherBonusPerLevel);
            maximumEnemyRewardMultiplier = Mathf.Max(1f, maximumRewardMultiplier);
        }

        public void ConfigureGrowth(
            int startingMaximumHealth,
            int maximumHealthPerLevel,
            int powerPerLevel)
        {
            baseMaximumHealth = Mathf.Max(1, startingMaximumHealth);
            healthPerLevel = Mathf.Max(0, maximumHealthPerLevel);
            attackPowerPerLevel = Mathf.Max(0, powerPerLevel);
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
            int classHealthBonus = identity.CharacterClass != null
                ? identity.CharacterClass.MaximumHealthBonus
                : 0;
            int classAttackBonus = identity.CharacterClass != null
                ? identity.CharacterClass.AttackPowerBonus
                : 0;

            health.SetMaximumHealth(
                CalculateMaximumHealth(
                    baseMaximumHealth,
                    classHealthBonus,
                    level,
                    healthPerLevel),
                restoreHealth);
            combatStats.SetClassAttackBonus(classAttackBonus);
            combatStats.SetProgressionAttackBonus(levelsGained * attackPowerPerLevel);
        }

        public static int CalculateMaximumHealth(
            int baseHealth,
            int classBonus,
            int characterLevel,
            int perLevelBonus)
        {
            int levelsGained = Mathf.Max(0, characterLevel - 1);
            return Mathf.Max(1, baseHealth) +
                   Mathf.Max(0, classBonus) +
                   levelsGained * Mathf.Max(0, perLevelBonus);
        }

        private void HandleIdentityChanged(PlayerIdentity _)
        {
            ApplyLevelBonuses(false);
        }

        private void CacheDependencies()
        {
            health ??= GetComponent<Health>();
            combatStats ??= GetComponent<CombatStats>();
            identity ??= GetComponent<PlayerIdentity>();
        }

        private int GetExperienceRequirement(int sourceLevel)
        {
            return baseExperienceRequirement +
                   Mathf.Max(0, sourceLevel - 1) * requirementGrowthPerLevel;
        }
    }
}
