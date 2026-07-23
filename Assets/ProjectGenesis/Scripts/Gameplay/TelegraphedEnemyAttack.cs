using UnityEngine;

namespace ProjectGenesis.Gameplay
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CombatStats))]
    public sealed class TelegraphedEnemyAttack : MonoBehaviour
    {
        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        private static readonly int ColorId = Shader.PropertyToID("_Color");

        [Header("Attack")]
        [SerializeField] private string attackName = "Powerful Attack";
        [SerializeField, Min(0f)] private float initialDelay = 2.5f;
        [SerializeField, Min(0.5f)] private float cooldown = 6f;
        [SerializeField, Min(0.2f)] private float windupDuration = 1.25f;
        [SerializeField, Min(0.5f)] private float hitRange = 1.55f;
        [SerializeField, Min(1f)] private float attackPowerMultiplier = 2f;

        [Header("Warning")]
        [SerializeField] private Color warningColor = new(1f, 0.28f, 0.08f, 1f);
        [SerializeField] private Renderer[] warningRenderers = System.Array.Empty<Renderer>();

        private CombatStats sourceStats;
        private MaterialPropertyBlock propertyBlock;
        private float nextReadyTime;
        private float resolveTime;
        private bool isWindingUp;

        public string AttackName => attackName;
        public float InitialDelay => initialDelay;
        public float Cooldown => cooldown;
        public float WindupDuration => windupDuration;
        public float HitRange => hitRange;
        public float AttackPowerMultiplier => attackPowerMultiplier;
        public bool IsWindingUp => isWindingUp;
        public bool IsValid => TryValidate(out _);
        public float GetRemainingWindup(float currentTime)
        {
            return isWindingUp ? Mathf.Max(0f, resolveTime - currentTime) : 0f;
        }

        private void Awake()
        {
            EnsureDependencies();
            ResetCycle(Time.time);
        }

        private void OnDisable()
        {
            ClearWarningVisual();
        }

        public void Configure(
            string authoredAttackName,
            float authoredInitialDelay,
            float authoredCooldown,
            float authoredWindupDuration,
            float authoredHitRange,
            float authoredAttackPowerMultiplier,
            Color authoredWarningColor,
            Renderer[] authoredWarningRenderers)
        {
            attackName = authoredAttackName?.Trim() ?? string.Empty;
            initialDelay = Mathf.Max(0f, authoredInitialDelay);
            cooldown = Mathf.Max(0.5f, authoredCooldown);
            windupDuration = Mathf.Max(0.2f, authoredWindupDuration);
            hitRange = Mathf.Max(0.5f, authoredHitRange);
            attackPowerMultiplier = Mathf.Max(1f, authoredAttackPowerMultiplier);
            warningColor = authoredWarningColor;
            warningRenderers = authoredWarningRenderers ?? System.Array.Empty<Renderer>();
            EnsureDependencies();
        }

        public bool UpdateAttack(
            float currentTime,
            float targetDistance,
            Health targetHealth,
            CombatStats targetStats,
            LocalMessageStream messageStream,
            string attackerName)
        {
            EnsureDependencies();

            if (targetHealth == null || targetHealth.IsDead)
            {
                CancelAndReset(currentTime);
                return false;
            }

            if (isWindingUp)
            {
                if (currentTime < resolveTime)
                {
                    return true;
                }

                ResolveAttack(
                    currentTime,
                    targetDistance,
                    targetHealth,
                    targetStats,
                    messageStream,
                    attackerName);
                return true;
            }

            if (currentTime < nextReadyTime || targetDistance > hitRange)
            {
                return false;
            }

            isWindingUp = true;
            resolveTime = currentTime + windupDuration;
            SetWarningVisual();
            messageStream?.Publish(
                LocalMessageCategory.Combat,
                $"{attackerName} готовит «{attackName}». Отойдите!");
            return true;
        }

        public int CalculateDamageAgainst(CombatStats targetStats)
        {
            EnsureDependencies();
            return sourceStats != null
                ? sourceStats.CalculateScaledDamageAgainst(
                    targetStats,
                    attackPowerMultiplier)
                : 0;
        }

        public void CancelAndReset(float currentTime)
        {
            isWindingUp = false;
            resolveTime = 0f;
            nextReadyTime = currentTime + initialDelay;
            ClearWarningVisual();
        }

        public void ResetCycle(float currentTime)
        {
            CancelAndReset(currentTime);
        }

        public bool TryValidate(out string error)
        {
            if (string.IsNullOrWhiteSpace(attackName))
            {
                error = "Telegraphed attack name is empty.";
                return false;
            }

            if (cooldown < 0.5f || windupDuration < 0.2f ||
                hitRange < 0.5f || attackPowerMultiplier < 1f)
            {
                error = "Telegraphed attack timing, range, or power is invalid.";
                return false;
            }

            if (warningRenderers == null || warningRenderers.Length == 0)
            {
                error = "Telegraphed attack has no warning renderers.";
                return false;
            }

            error = string.Empty;
            return true;
        }

        private void ResolveAttack(
            float currentTime,
            float targetDistance,
            Health targetHealth,
            CombatStats targetStats,
            LocalMessageStream messageStream,
            string attackerName)
        {
            isWindingUp = false;
            resolveTime = 0f;
            nextReadyTime = currentTime + cooldown;
            ClearWarningVisual();

            if (targetDistance > hitRange)
            {
                messageStream?.Publish(
                    LocalMessageCategory.Combat,
                    $"Вы избежали атаки «{attackName}»: {attackerName}.");
                return;
            }

            int healthBefore = targetHealth.CurrentHealth;
            int damage = CalculateDamageAgainst(targetStats);
            if (!targetHealth.TakeDamage(damage))
            {
                return;
            }

            int appliedDamage = healthBefore - targetHealth.CurrentHealth;
            messageStream?.Publish(
                LocalMessageCategory.Combat,
                $"{attackerName} применяет «{attackName}» и наносит вам " +
                $"{appliedDamage} урона.");
        }

        private void EnsureDependencies()
        {
            sourceStats ??= GetComponent<CombatStats>();
            propertyBlock ??= new MaterialPropertyBlock();
            warningRenderers ??= System.Array.Empty<Renderer>();
        }

        private void SetWarningVisual()
        {
            foreach (Renderer targetRenderer in warningRenderers)
            {
                if (targetRenderer == null)
                {
                    continue;
                }

                propertyBlock.Clear();
                propertyBlock.SetColor(BaseColorId, warningColor);
                propertyBlock.SetColor(ColorId, warningColor);
                targetRenderer.SetPropertyBlock(propertyBlock);
            }
        }

        private void ClearWarningVisual()
        {
            if (warningRenderers == null)
            {
                return;
            }

            foreach (Renderer targetRenderer in warningRenderers)
            {
                if (targetRenderer != null)
                {
                    targetRenderer.SetPropertyBlock(null);
                }
            }
        }
    }
}
