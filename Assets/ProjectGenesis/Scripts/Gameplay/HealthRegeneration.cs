using UnityEngine;

namespace ProjectGenesis.Gameplay
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Health))]
    public sealed class HealthRegeneration : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float recoveryDelay = 8f;
        [SerializeField, Min(1)] private int healAmount = 2;
        [SerializeField, Min(0.1f)] private float tickInterval = 1f;
        [SerializeField] private bool startsEnabled = true;

        private Health health;
        private bool regenerationAllowed;
        private float nextHealTime;

        public float RecoveryDelay => recoveryDelay;
        public int HealAmount => healAmount;
        public float TickInterval => tickInterval;
        public bool StartsEnabled => startsEnabled;
        public bool IsRegenerationAllowed => regenerationAllowed;

        private void Awake()
        {
            health = GetComponent<Health>();
            health.Damaged += HandleDamaged;
            regenerationAllowed = startsEnabled;
            RestartDelay();
        }

        private void OnDestroy()
        {
            if (health != null)
            {
                health.Damaged -= HandleDamaged;
            }
        }

        private void Update()
        {
            if (!regenerationAllowed || health == null || health.IsDead ||
                health.CurrentHealth >= health.MaximumHealth || Time.time < nextHealTime)
            {
                return;
            }

            health.Heal(healAmount);
            nextHealTime = Time.time + tickInterval;
        }

        public void Configure(float delay, int amount, float interval, bool enabledAtStart)
        {
            recoveryDelay = Mathf.Max(0f, delay);
            healAmount = Mathf.Max(1, amount);
            tickInterval = Mathf.Max(0.1f, interval);
            startsEnabled = enabledAtStart;
            regenerationAllowed = enabledAtStart;
            RestartDelay();
        }

        public void SetRegenerationAllowed(bool allowed, bool restartDelay = false)
        {
            bool becameAllowed = allowed && !regenerationAllowed;
            regenerationAllowed = allowed;

            if (restartDelay || becameAllowed)
            {
                RestartDelay();
            }
        }

        private void HandleDamaged(Health _, int __)
        {
            RestartDelay();
        }

        private void RestartDelay()
        {
            nextHealTime = Time.time + recoveryDelay;
        }
    }
}
