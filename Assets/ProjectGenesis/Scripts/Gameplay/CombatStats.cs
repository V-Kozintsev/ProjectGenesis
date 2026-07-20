using System;
using UnityEngine;

namespace ProjectGenesis.Gameplay
{
    [DisallowMultipleComponent]
    public sealed class CombatStats : MonoBehaviour
    {
        [SerializeField, Min(1)] private int attackPower = 12;
        [SerializeField, Min(0)] private int defense = 3;
        [SerializeField, Min(0.25f)] private float attackRange = 1.35f;
        [SerializeField, Min(0.1f)] private float attackInterval = 0.8f;
        [SerializeField, Min(0)] private int classAttackBonus;
        [SerializeField, Min(0)] private int equipmentAttackBonus;
        [SerializeField, Min(0)] private int progressionAttackBonus;

        public event Action<CombatStats> Changed;

        public int BaseAttackPower => attackPower;
        public int ClassAttackBonus => classAttackBonus;
        public int EquipmentAttackBonus => equipmentAttackBonus;
        public int ProgressionAttackBonus => progressionAttackBonus;
        public int AttackPower =>
            attackPower + classAttackBonus + equipmentAttackBonus + progressionAttackBonus;
        public int Defense => defense;
        public float AttackRange => attackRange;
        public float AttackInterval => attackInterval;

        public void Configure(int power, int armor, float range, float interval)
        {
            attackPower = Mathf.Max(1, power);
            defense = Mathf.Max(0, armor);
            attackRange = Mathf.Max(0.25f, range);
            attackInterval = Mathf.Max(0.1f, interval);
        }

        public void SetEquipmentAttackBonus(int bonus)
        {
            int nextBonus = Mathf.Max(0, bonus);
            if (equipmentAttackBonus == nextBonus)
            {
                return;
            }

            equipmentAttackBonus = nextBonus;
            Changed?.Invoke(this);
        }

        public void SetClassAttackBonus(int bonus)
        {
            int nextBonus = Mathf.Max(0, bonus);
            if (classAttackBonus == nextBonus)
            {
                return;
            }

            classAttackBonus = nextBonus;
            Changed?.Invoke(this);
        }

        public void SetProgressionAttackBonus(int bonus)
        {
            int nextBonus = Mathf.Max(0, bonus);
            if (progressionAttackBonus == nextBonus)
            {
                return;
            }

            progressionAttackBonus = nextBonus;
            Changed?.Invoke(this);
        }

        public int CalculateDamageAgainst(CombatStats targetStats)
        {
            return CalculateScaledDamageAgainst(targetStats, 1f);
        }

        public int CalculateScaledDamageAgainst(CombatStats targetStats, float attackPowerMultiplier)
        {
            int targetDefense = targetStats != null ? targetStats.Defense : 0;
            int scaledAttackPower = Mathf.RoundToInt(
                AttackPower * Mathf.Max(0.1f, attackPowerMultiplier));
            return Mathf.Max(1, scaledAttackPower - targetDefense);
        }
    }
}
