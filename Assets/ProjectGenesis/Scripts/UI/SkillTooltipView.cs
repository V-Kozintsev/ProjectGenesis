using ProjectGenesis.Data;
using ProjectGenesis.Gameplay;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ProjectGenesis.UI
{
    public sealed class SkillTooltipView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private GameObject tooltipRoot;
        [SerializeField] private Text nameText;
        [SerializeField] private Text descriptionText;
        [SerializeField] private Text damageText;
        [SerializeField] private Text detailsText;
        [SerializeField] private PlayerSkillController skillController;
        [SerializeField] private PlayerCombatController combatController;
        [SerializeField] private CombatStats combatStats;
        [SerializeField, Min(0)] private int slotIndex;

        public GameObject TooltipRoot => tooltipRoot;
        public int SlotIndex => slotIndex;

        public void Initialize(
            GameObject root,
            Text skillName,
            Text skillDescription,
            Text skillDamage,
            Text skillDetails,
            PlayerSkillController playerSkills,
            PlayerCombatController playerCombat,
            CombatStats playerCombatStats,
            int quickSlotIndex)
        {
            tooltipRoot = root;
            nameText = skillName;
            descriptionText = skillDescription;
            damageText = skillDamage;
            detailsText = skillDetails;
            skillController = playerSkills;
            combatController = playerCombat;
            combatStats = playerCombatStats;
            slotIndex = Mathf.Max(0, quickSlotIndex);
        }

        private void Awake()
        {
            Hide();
        }

        private void OnDisable()
        {
            Hide();
        }

        private void Update()
        {
            if (tooltipRoot != null && tooltipRoot.activeSelf)
            {
                Refresh();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Refresh();
            tooltipRoot?.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Hide();
        }

        private void Refresh()
        {
            SkillDefinition skill = skillController != null
                ? skillController.GetSkill(slotIndex)
                : null;
            if (skill == null)
            {
                Hide();
                return;
            }

            SetText(nameText, skill.DisplayName);
            SetText(descriptionText, skill.Description);

            int powerPercent = Mathf.RoundToInt(skill.AttackPowerMultiplier * 100f);
            EnemyBrain target = combatController != null ? combatController.Target : null;
            if (target != null && !target.IsDead && combatStats != null)
            {
                int targetDamage = combatStats.CalculateScaledDamageAgainst(
                    target.CombatStats,
                    skill.AttackPowerMultiplier);
                SetText(damageText, $"Урон по выбранной цели: {targetDamage}");
            }
            else
            {
                SetText(damageText, $"Урон: {powerPercent}% силы атаки");
            }

            SetText(
                detailsText,
                $"Дальность: {skill.Range:0.##} · Перезарядка: {skill.Cooldown:0.#} с");
        }

        private void Hide()
        {
            tooltipRoot?.SetActive(false);
        }

        private static void SetText(Text label, string value)
        {
            if (label != null)
            {
                label.text = value;
            }
        }
    }
}
