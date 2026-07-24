using ProjectGenesis.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectGenesis.UI
{
    public sealed class CombatHudView : MonoBehaviour
    {
        [SerializeField] private Text playerHealthText;
        [SerializeField] private Image playerHealthFill;
        [SerializeField] private Text targetNameText;
        [SerializeField] private Text targetHealthText;
        [SerializeField] private Image targetHealthFill;
        [SerializeField] private Text targetStatusText;
        [SerializeField] private Text experienceText;
        [SerializeField] private GameObject targetPanel;
        [SerializeField] private Button clearTargetButton;

        [SerializeField] private Health playerHealth;
        [SerializeField] private PlayerProgression progression;
        [SerializeField] private PlayerCombatController combatController;
        [SerializeField] private PlayerInteractionController interactionController;
        private EnemyBrain observedTarget;
        private InteractableNpc observedNpc;

        public void Initialize(
            Text healthLabel,
            Image healthFill,
            Text targetLabel,
            Text targetHealthLabel,
            Image selectedTargetHealthFill,
            Text selectedTargetStatusLabel,
            Text experienceLabel,
            GameObject selectedTargetPanel,
            Button selectedTargetClearButton,
            Health health,
            PlayerProgression playerProgression,
            PlayerCombatController combat,
            PlayerInteractionController interaction)
        {
            playerHealthText = healthLabel;
            playerHealthFill = healthFill;
            targetNameText = targetLabel;
            targetHealthText = targetHealthLabel;
            targetHealthFill = selectedTargetHealthFill;
            targetStatusText = selectedTargetStatusLabel;
            experienceText = experienceLabel;
            targetPanel = selectedTargetPanel;
            clearTargetButton = selectedTargetClearButton;
            playerHealth = health;
            progression = playerProgression;
            combatController = combat;
            interactionController = interaction;
        }

        public void SetTargetStatusLabel(Text statusLabel)
        {
            targetStatusText = statusLabel;
            RefreshTarget();
        }

        private void Awake()
        {
            Subscribe();
            RefreshPlayer();
            RefreshExperience();
            ObserveTarget(combatController != null ? combatController.Target : null);
            ObserveNpc(interactionController != null ? interactionController.SelectedNpc : null);

            if (clearTargetButton != null)
            {
                clearTargetButton.onClick.AddListener(ClearSelectedTarget);
            }
        }

        private void OnDestroy()
        {
            if (playerHealth != null)
            {
                playerHealth.Changed -= HandlePlayerHealthChanged;
            }

            if (progression != null)
            {
                progression.ExperienceChanged -= HandleExperienceChanged;
            }

            if (combatController != null)
            {
                combatController.TargetChanged -= ObserveTarget;
            }

            if (interactionController != null)
            {
                interactionController.SelectionChanged -= ObserveNpc;
            }

            if (clearTargetButton != null)
            {
                clearTargetButton.onClick.RemoveListener(ClearSelectedTarget);
            }

            StopObservingTarget();
        }

        private void Update()
        {
            if (observedTarget != null && !observedTarget.IsDead)
            {
                RefreshTargetStatus();
            }
        }

        private void Subscribe()
        {
            if (playerHealth != null)
            {
                playerHealth.Changed += HandlePlayerHealthChanged;
            }

            if (progression != null)
            {
                progression.ExperienceChanged += HandleExperienceChanged;
            }

            if (combatController != null)
            {
                combatController.TargetChanged += ObserveTarget;
            }

            if (interactionController != null)
            {
                interactionController.SelectionChanged += ObserveNpc;
            }
        }

        private void ObserveTarget(EnemyBrain enemy)
        {
            StopObservingTarget();
            observedTarget = enemy;

            if (observedTarget != null && observedTarget.Health != null)
            {
                observedTarget.Health.Changed += HandleTargetHealthChanged;
            }

            RefreshTarget();
        }

        private void ObserveNpc(InteractableNpc npc)
        {
            observedNpc = npc;
            RefreshTarget();
        }

        private void StopObservingTarget()
        {
            if (observedTarget != null && observedTarget.Health != null)
            {
                observedTarget.Health.Changed -= HandleTargetHealthChanged;
            }

            observedTarget = null;
        }

        private void HandlePlayerHealthChanged(Health _)
        {
            RefreshPlayer();
        }

        private void HandleTargetHealthChanged(Health _)
        {
            RefreshTarget();
        }

        private void HandleExperienceChanged(PlayerProgression _)
        {
            RefreshExperience();
        }

        private void ClearSelectedTarget()
        {
            if (observedTarget != null)
            {
                combatController?.ClearTarget();
            }
            else
            {
                interactionController?.ClearSelection();
            }
        }

        private void RefreshPlayer()
        {
            if (playerHealthText != null && playerHealth != null)
            {
                playerHealthText.text = $"Здоровье: {playerHealth.CurrentHealth} / {playerHealth.MaximumHealth}";
            }

            if (playerHealthFill != null && playerHealth != null)
            {
                SetHealthFill(
                    playerHealthFill,
                    (float)playerHealth.CurrentHealth / playerHealth.MaximumHealth);
            }
        }

        private void RefreshExperience()
        {
            if (experienceText != null && progression != null)
            {
                experienceText.text = progression.Level >= progression.MaximumLevel
                    ? $"Уровень {progression.Level}   Опыт: MAX"
                    : $"Уровень {progression.Level}   Опыт: {progression.CurrentExperience} / {progression.ExperienceToNextLevel}";
            }
        }

        private void RefreshTarget()
        {
            bool hasEnemyTarget = observedTarget != null && !observedTarget.IsDead;
            bool hasNpcTarget = observedNpc != null && !IsNpcInteractionHubShowing(observedNpc);
            bool hasTarget = hasEnemyTarget || hasNpcTarget;

            if (targetPanel != null)
            {
                targetPanel.SetActive(hasTarget);
            }

            if (!hasTarget)
            {
                return;
            }

            if (targetNameText != null)
            {
                targetNameText.text = hasEnemyTarget
                    ? observedTarget.Rank == EnemyRank.Common
                        ? $"{observedTarget.DisplayName} [Ур. {observedTarget.EnemyLevel}]"
                        : $"{observedTarget.DisplayName} [{GetRankLabel(observedTarget.Rank)} · Ур. {observedTarget.EnemyLevel}]"
                    : observedNpc.DisplayName;
            }

            if (targetHealthText != null)
            {
                if (hasEnemyTarget)
                {
                    Health targetHealth = observedTarget.Health;
                    targetHealthText.text = $"Здоровье: {targetHealth.CurrentHealth} / {targetHealth.MaximumHealth}";
                }
                else
                {
                    targetHealthText.text = "Мирный NPC";
                }
            }

            if (targetHealthFill != null)
            {
                targetHealthFill.transform.parent.gameObject.SetActive(hasEnemyTarget);

                if (hasEnemyTarget)
                {
                    Health targetHealth = observedTarget.Health;
                    SetHealthFill(
                        targetHealthFill,
                        (float)targetHealth.CurrentHealth / targetHealth.MaximumHealth);
                }
            }

            RefreshTargetStatus();
        }

        private static string GetRankLabel(EnemyRank rank)
        {
            return rank switch
            {
                EnemyRank.Elite => "Элита",
                EnemyRank.Boss => "Босс",
                _ => "Противник"
            };
        }

        private void RefreshTargetStatus()
        {
            if (targetStatusText == null)
            {
                return;
            }

            if (observedTarget != null && !observedTarget.IsDead)
            {
                string warning = BuildEnemyWarning(observedTarget);
                bool hasWarning = !string.IsNullOrWhiteSpace(warning);
                targetStatusText.gameObject.SetActive(hasWarning);
                targetStatusText.text = warning;
                targetStatusText.color = new Color(1f, 0.48f, 0.2f);
                return;
            }

            if (observedNpc != null)
            {
                targetStatusText.gameObject.SetActive(false);
                return;
            }

            targetStatusText.gameObject.SetActive(false);
        }

        private bool IsNpcInteractionHubShowing(InteractableNpc npc)
        {
            NpcInteractionView hub = interactionController != null
                ? interactionController.NpcInteractionView
                : null;
            return hub != null && hub.IsVisible && hub.CurrentNpc == npc;
        }

        private static string BuildEnemyWarning(EnemyBrain enemy)
        {
            TelegraphedEnemyAttack special = enemy.SpecialAttack != null
                ? enemy.SpecialAttack
                : enemy.GetComponent<TelegraphedEnemyAttack>();
            if (special != null && special.IsWindingUp)
            {
                float remaining = special.GetRemainingWindup(Time.time);
                return $"Опасно: {special.AttackName} {remaining:0.0} с. Отойдите.";
            }

            return string.Empty;
        }

        private static void SetHealthFill(Image fill, float normalizedHealth)
        {
            float amount = Mathf.Clamp01(normalizedHealth);
            RectTransform rect = fill.rectTransform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = new Vector2(amount, 1f);
            rect.offsetMin = new Vector2(2f, 2f);
            rect.offsetMax = new Vector2(amount > 0f ? -2f : 0f, -2f);
            fill.enabled = amount > 0f;
        }
    }
}
