using ProjectGenesis.Data;
using ProjectGenesis.Gameplay;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace ProjectGenesis.UI
{
    public sealed class CharacterStatsView : MonoBehaviour
    {
        [SerializeField] private GameObject windowRoot;
        [SerializeField] private Button openButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private Text characterNameText;
        [SerializeField] private Text identityText;
        [SerializeField] private Text experienceText;
        [SerializeField] private Text healthText;
        [SerializeField] private Text healthBreakdownText;
        [SerializeField] private Text attackText;
        [SerializeField] private Text attackBreakdownText;
        [SerializeField] private Text defenseText;
        [SerializeField] private Text attackTimingText;
        [SerializeField] private Text skillPowerText;
        [SerializeField] private PlayerIdentity identity;
        [SerializeField] private PlayerProgression progression;
        [SerializeField] private Health health;
        [SerializeField] private CombatStats combatStats;
        [SerializeField] private PlayerCombatController combatController;
        [SerializeField] private PlayerSkillController skillController;

        public GameObject WindowRoot => windowRoot;
        public CombatStats CombatStats => combatStats;
        public PlayerProgression Progression => progression;

        public void Initialize(
            GameObject statsWindow,
            Button statsOpenButton,
            Button statsCloseButton,
            Text nameLabel,
            Text identityLabel,
            Text experienceLabel,
            Text healthLabel,
            Text healthBreakdownLabel,
            Text attackLabel,
            Text attackBreakdownLabel,
            Text defenseLabel,
            Text attackTimingLabel,
            Text skillPowerLabel,
            PlayerIdentity playerIdentity,
            PlayerProgression playerProgression,
            Health playerHealth,
            CombatStats playerCombatStats,
            PlayerCombatController playerCombatController,
            PlayerSkillController playerSkillController)
        {
            windowRoot = statsWindow;
            openButton = statsOpenButton;
            closeButton = statsCloseButton;
            characterNameText = nameLabel;
            identityText = identityLabel;
            experienceText = experienceLabel;
            healthText = healthLabel;
            healthBreakdownText = healthBreakdownLabel;
            attackText = attackLabel;
            attackBreakdownText = attackBreakdownLabel;
            defenseText = defenseLabel;
            attackTimingText = attackTimingLabel;
            skillPowerText = skillPowerLabel;
            identity = playerIdentity;
            progression = playerProgression;
            health = playerHealth;
            combatStats = playerCombatStats;
            combatController = playerCombatController;
            skillController = playerSkillController;
        }

        private void Awake()
        {
            openButton?.onClick.AddListener(ToggleWindow);
            closeButton?.onClick.AddListener(CloseWindow);

            if (identity != null)
            {
                identity.Changed += HandleIdentityChanged;
            }

            if (progression != null)
            {
                progression.ExperienceChanged += HandleProgressionChanged;
                progression.LevelChanged += HandleProgressionChanged;
            }

            if (health != null)
            {
                health.Changed += HandleHealthChanged;
            }

            if (combatStats != null)
            {
                combatStats.Changed += HandleCombatStatsChanged;
            }

            if (combatController != null)
            {
                combatController.TargetChanged += HandleTargetChanged;
            }

            CloseWindow();
            Refresh();
        }

        private void OnDestroy()
        {
            openButton?.onClick.RemoveListener(ToggleWindow);
            closeButton?.onClick.RemoveListener(CloseWindow);

            if (identity != null)
            {
                identity.Changed -= HandleIdentityChanged;
            }

            if (progression != null)
            {
                progression.ExperienceChanged -= HandleProgressionChanged;
                progression.LevelChanged -= HandleProgressionChanged;
            }

            if (health != null)
            {
                health.Changed -= HandleHealthChanged;
            }

            if (combatStats != null)
            {
                combatStats.Changed -= HandleCombatStatsChanged;
            }

            if (combatController != null)
            {
                combatController.TargetChanged -= HandleTargetChanged;
            }
        }

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard != null && keyboard.cKey.wasPressedThisFrame)
            {
                ToggleWindow();
            }
        }

        private void ToggleWindow()
        {
            if (windowRoot == null)
            {
                return;
            }

            windowRoot.SetActive(!windowRoot.activeSelf);
            Refresh();
        }

        private void CloseWindow()
        {
            windowRoot?.SetActive(false);
        }

        private void HandleIdentityChanged(PlayerIdentity _)
        {
            Refresh();
        }

        private void HandleProgressionChanged(PlayerProgression _)
        {
            Refresh();
        }

        private void HandleHealthChanged(Health _)
        {
            Refresh();
        }

        private void HandleCombatStatsChanged(CombatStats _)
        {
            Refresh();
        }

        private void HandleTargetChanged(EnemyBrain _)
        {
            Refresh();
        }

        private void Refresh()
        {
            if (identity == null || progression == null || health == null || combatStats == null)
            {
                return;
            }

            if (characterNameText != null)
            {
                characterNameText.text = identity.CharacterName;
            }

            if (identityText != null)
            {
                string raceName = identity.Race != null ? identity.Race.DisplayName : "Раса не задана";
                string className = identity.CharacterClass != null
                    ? identity.CharacterClass.DisplayName
                    : "Класс не задан";
                identityText.text = $"Уровень {progression.Level} · {raceName} · {className}";
            }

            if (experienceText != null)
            {
                experienceText.text = progression.ExperienceToNextLevel > 0
                    ? $"Опыт: {progression.CurrentExperience} / {progression.ExperienceToNextLevel}"
                    : "Опыт: максимальный уровень";
            }

            if (healthText != null)
            {
                healthText.text = $"Здоровье: {health.CurrentHealth} / {health.MaximumHealth}";
            }

            if (healthBreakdownText != null)
            {
                healthBreakdownText.text =
                    $"Бонусы: класс +{progression.ClassHealthBonus} · " +
                    $"уровень +{progression.ProgressionHealthBonus}";
            }

            if (attackText != null)
            {
                attackText.text = $"Сила атаки: {combatStats.AttackPower}";
            }

            if (attackBreakdownText != null)
            {
                attackBreakdownText.text =
                    $"Бонусы: класс +{combatStats.ClassAttackBonus} · " +
                    $"уровень +{combatStats.ProgressionAttackBonus} · " +
                    $"оружие +{combatStats.EquipmentAttackBonus}";
            }

            if (defenseText != null)
            {
                defenseText.text = $"Защита: {combatStats.Defense}";
            }

            if (attackTimingText != null)
            {
                attackTimingText.text =
                    $"Скорость атаки: 1 удар каждые {combatStats.AttackInterval:0.##} с";
            }

            if (skillPowerText != null)
            {
                SkillDefinition skill = skillController != null ? skillController.GetSkill(0) : null;
                if (skill != null)
                {
                    EnemyBrain target = combatController != null ? combatController.Target : null;
                    if (target != null && !target.IsDead)
                    {
                        int targetDamage = combatStats.CalculateScaledDamageAgainst(
                            target.CombatStats,
                            skill.AttackPowerMultiplier);
                        skillPowerText.text =
                            $"{skill.DisplayName}: {targetDamage} урона по выбранной цели";
                    }
                    else
                    {
                        int powerPercent = Mathf.RoundToInt(skill.AttackPowerMultiplier * 100f);
                        skillPowerText.text =
                            $"{skill.DisplayName}: усиленный удар ({powerPercent}% силы атаки)";
                    }
                }
                else
                {
                    skillPowerText.text = "Активный навык не назначен";
                }
            }
        }
    }
}
