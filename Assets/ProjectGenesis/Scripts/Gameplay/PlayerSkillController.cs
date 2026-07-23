using System;
using ProjectGenesis.Data;
using UnityEngine;
using UnityEngine.AI;

namespace ProjectGenesis.Gameplay
{
    [DefaultExecutionOrder(130)]
    [RequireComponent(typeof(PlayerCombatController), typeof(CombatStats), typeof(Health))]
    public sealed class PlayerSkillController : MonoBehaviour
    {
        [SerializeField] private SkillDefinition[] quickSlots = Array.Empty<SkillDefinition>();
        [SerializeField, Min(0.1f)] private float approachPadding = 0.35f;
        [SerializeField, Min(1f)] private float maximumTargetDistance = 30f;

        private float[] nextReadyTimes = Array.Empty<float>();
        private NavMeshAgent agent;
        private Health health;
        private CombatStats stats;
        private PlayerCombatController combatController;
        private Collider playerCollider;
        private LocalMessageStream messageStream;
        private PlayerZoneController zoneController;
        private int pendingSlotIndex = -1;

        public event Action<int, SkillDefinition> SlotChanged;
        public event Action<int, SkillDefinition, float> CooldownChanged;
        public event Action<string> Feedback;

        public int SlotCount => quickSlots != null ? quickSlots.Length : 0;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
            stats = GetComponent<CombatStats>();
            combatController = GetComponent<PlayerCombatController>();
            playerCollider = GetComponent<Collider>();
            messageStream = GetComponent<LocalMessageStream>();
            zoneController = GetComponent<PlayerZoneController>();
            EnsureCooldownStorage();
        }

        private void Update()
        {
            if (pendingSlotIndex < 0)
            {
                return;
            }

            SkillDefinition skill = GetSkill(pendingSlotIndex);
            EnemyBrain target = combatController != null ? combatController.Target : null;

            if (!CanKeepPendingSkill(skill, target))
            {
                pendingSlotIndex = -1;
                return;
            }

            if (!IsInRange(skill, target))
            {
                MoveTowardTarget(skill, target);
                return;
            }

            StopAgent();
            FaceTarget(target.transform.position);
            ApplySkill(pendingSlotIndex, skill, target);
            pendingSlotIndex = -1;
        }

        public void ConfigureQuickSlots(SkillDefinition[] skills)
        {
            quickSlots = skills ?? Array.Empty<SkillDefinition>();
            EnsureCooldownStorage();

            for (int index = 0; index < quickSlots.Length; index++)
            {
                SlotChanged?.Invoke(index, quickSlots[index]);
            }
        }

        public SkillDefinition GetSkill(int slotIndex)
        {
            if (quickSlots == null || slotIndex < 0 || slotIndex >= quickSlots.Length)
            {
                return null;
            }

            return quickSlots[slotIndex];
        }

        public float GetRemainingCooldown(int slotIndex)
        {
            if (nextReadyTimes == null || slotIndex < 0 || slotIndex >= nextReadyTimes.Length)
            {
                return 0f;
            }

            return Mathf.Max(0f, nextReadyTimes[slotIndex] - Time.time);
        }

        public bool IsReady(int slotIndex)
        {
            return GetRemainingCooldown(slotIndex) <= 0f;
        }

        public void TryUseQuickSlot(int slotIndex)
        {
            SkillDefinition skill = GetSkill(slotIndex);
            if (skill == null)
            {
                PublishFeedback("В этой ячейке нет навыка.");
                return;
            }

            if (!skill.IsValid)
            {
                PublishFeedback("Данные навыка заполнены неверно.");
                return;
            }

            if (health == null || health.IsDead || (combatController != null && combatController.IsInputLocked))
            {
                PublishFeedback("Нельзя применить навык сейчас.");
                return;
            }

            if (zoneController != null && !zoneController.TryAuthorizeCombat())
            {
                pendingSlotIndex = -1;
                return;
            }

            float remainingCooldown = GetRemainingCooldown(slotIndex);
            if (remainingCooldown > 0f)
            {
                PublishFeedback($"Навык перезаряжается: {Mathf.CeilToInt(remainingCooldown)} с.");
                return;
            }

            EnemyBrain target = combatController != null ? combatController.Target : null;
            if (target == null || target.IsDead)
            {
                PublishFeedback("Выберите врага для навыка.");
                return;
            }

            if (GetDistanceToTarget(target) > maximumTargetDistance)
            {
                PublishFeedback("Цель слишком далеко.");
                return;
            }

            pendingSlotIndex = slotIndex;
            combatController.StopCombatAction();

            if (IsInRange(skill, target))
            {
                ApplySkill(slotIndex, skill, target);
                pendingSlotIndex = -1;
                return;
            }

            MoveTowardTarget(skill, target);
            PublishFeedback($"Подхожу для {skill.DisplayName}.");
        }

        public void CancelSkillAction()
        {
            if (pendingSlotIndex < 0)
            {
                return;
            }

            pendingSlotIndex = -1;
            StopAgent();
        }

        private void EnsureCooldownStorage()
        {
            if (quickSlots == null)
            {
                quickSlots = Array.Empty<SkillDefinition>();
            }

            if (nextReadyTimes.Length == quickSlots.Length)
            {
                return;
            }

            Array.Resize(ref nextReadyTimes, quickSlots.Length);
        }

        private bool CanKeepPendingSkill(SkillDefinition skill, EnemyBrain target)
        {
            if (skill == null || target == null || target.IsDead)
            {
                return false;
            }

            if (health == null || health.IsDead || (combatController != null && combatController.IsInputLocked))
            {
                return false;
            }

            if (zoneController != null && !zoneController.IsCombatAllowed)
            {
                return false;
            }

            if (GetRemainingCooldown(pendingSlotIndex) > 0f)
            {
                return false;
            }

            return GetDistanceToTarget(target) <= maximumTargetDistance;
        }

        private void ApplySkill(int slotIndex, SkillDefinition skill, EnemyBrain target)
        {
            if (target == null || target.IsDead)
            {
                PublishFeedback("Цель недоступна.");
                return;
            }

            if (zoneController != null && !zoneController.TryAuthorizeCombat())
            {
                return;
            }

            int calculatedDamage = stats.CalculateScaledDamageAgainst(
                target.CombatStats,
                skill.AttackPowerMultiplier);
            int appliedDamage = target.ReceiveAttack(combatController, calculatedDamage);
            nextReadyTimes[slotIndex] = Time.time + skill.Cooldown;
            CooldownChanged?.Invoke(slotIndex, skill, skill.Cooldown);
            PublishFeedback($"{skill.DisplayName}: {appliedDamage} урона.");
            combatController.ResumeCombatAfterSkill();
        }

        private bool IsInRange(SkillDefinition skill, EnemyBrain target)
        {
            return GetDistanceToTarget(target) <= skill.Range;
        }

        private float GetDistanceToTarget(EnemyBrain target)
        {
            if (target == null)
            {
                return float.MaxValue;
            }

            return CombatDistance.GetPlanarSurfaceDistance(
                transform,
                playerCollider,
                target.transform,
                target.TargetCollider);
        }

        private void MoveTowardTarget(SkillDefinition skill, EnemyBrain target)
        {
            if (target == null || agent == null || !agent.enabled || !agent.isOnNavMesh)
            {
                return;
            }

            Vector3 awayFromTarget = transform.position - target.transform.position;
            awayFromTarget.y = 0f;

            if (awayFromTarget.sqrMagnitude <= 0.001f)
            {
                awayFromTarget = -target.transform.forward;
            }

            float desiredOffset = skill.Range + agent.radius + approachPadding;
            Vector3 desiredPoint = target.transform.position + awayFromTarget.normalized * desiredOffset;

            if (NavMesh.SamplePosition(desiredPoint, out NavMeshHit hit, 2.5f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
        }

        private void FaceTarget(Vector3 targetPosition)
        {
            Vector3 direction = targetPosition - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(direction.normalized, Vector3.up),
                    14f * Time.deltaTime);
            }
        }

        private void StopAgent()
        {
            if (agent != null && agent.enabled && agent.isOnNavMesh)
            {
                agent.ResetPath();
            }
        }

        private void PublishFeedback(string message)
        {
            Feedback?.Invoke(message);
            messageStream ??= GetComponent<LocalMessageStream>();
            messageStream?.Publish(LocalMessageCategory.Combat, message);
        }
    }
}
