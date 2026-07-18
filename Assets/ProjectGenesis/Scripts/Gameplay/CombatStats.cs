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

        public int AttackPower => attackPower;
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

        public int CalculateDamageAgainst(CombatStats targetStats)
        {
            int targetDefense = targetStats != null ? targetStats.Defense : 0;
            return Mathf.Max(1, attackPower - targetDefense);
        }
    }
}
