using System;
using UnityEngine;

namespace ProjectGenesis.Gameplay
{
    [DisallowMultipleComponent]
    public sealed class Health : MonoBehaviour
    {
        [SerializeField, Min(1)] private int maximumHealth = 100;
        [SerializeField] private int currentHealth = 100;

        public event Action<Health> Changed;
        public event Action<Health, int> Damaged;
        public event Action<Health> Died;

        public int MaximumHealth => maximumHealth;
        public int CurrentHealth => currentHealth;
        public bool IsDead => currentHealth <= 0;

        private void Awake()
        {
            currentHealth = maximumHealth;
        }

        public void Configure(int maximum)
        {
            maximumHealth = Mathf.Max(1, maximum);
            currentHealth = maximumHealth;
        }

        public bool TakeDamage(int amount)
        {
            if (IsDead || amount <= 0)
            {
                return false;
            }

            int previousHealth = currentHealth;
            currentHealth = Mathf.Max(0, currentHealth - amount);
            int damageTaken = previousHealth - currentHealth;
            Changed?.Invoke(this);
            Damaged?.Invoke(this, damageTaken);

            if (IsDead)
            {
                Died?.Invoke(this);
            }

            return true;
        }

        public bool Heal(int amount)
        {
            if (IsDead || amount <= 0 || currentHealth >= maximumHealth)
            {
                return false;
            }

            currentHealth = Mathf.Min(maximumHealth, currentHealth + amount);
            Changed?.Invoke(this);
            return true;
        }

        public void RestoreFull()
        {
            currentHealth = maximumHealth;
            Changed?.Invoke(this);
        }

        public void SetMaximumHealth(int maximum, bool restoreFull)
        {
            maximumHealth = Mathf.Max(1, maximum);
            currentHealth = restoreFull
                ? maximumHealth
                : Mathf.Clamp(currentHealth, 0, maximumHealth);
            Changed?.Invoke(this);
        }
    }
}
