using System;
using UnityEngine;
using UnityEngine.AI;

namespace ProjectGenesis.Gameplay
{
    [DefaultExecutionOrder(120)]
    [RequireComponent(typeof(Health), typeof(CombatStats), typeof(PlayerProgression))]
    [RequireComponent(typeof(HealthRegeneration))]
    public sealed class PlayerCombatController : MonoBehaviour
    {
        [SerializeField, Min(0.1f)] private float approachPadding = 0.35f;
        [SerializeField, Min(1f)] private float maximumTargetDistance = 30f;

        private NavMeshAgent agent;
        private Health health;
        private HealthRegeneration regeneration;
        private CombatStats stats;
        private PlayerProgression progression;
        private QuestLog questLog;
        private Collider playerCollider;
        private EnemyBrain target;
        private float nextAttackTime;
        private bool isCombatActive;

        public event Action<EnemyBrain> TargetChanged;

        public EnemyBrain Target => target;
        public bool IsInputLocked => health == null || health.IsDead;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
            regeneration = GetComponent<HealthRegeneration>();
            stats = GetComponent<CombatStats>();
            progression = GetComponent<PlayerProgression>();
            questLog = GetComponent<QuestLog>();
            playerCollider = GetComponent<Collider>();
            health.Died += HandlePlayerDied;
        }

        private void OnDestroy()
        {
            if (health != null)
            {
                health.Died -= HandlePlayerDied;
            }

            UnsubscribeFromTarget();
        }

        private void Update()
        {
            if (IsInputLocked || target == null)
            {
                return;
            }

            if (target.IsDead)
            {
                ClearTargetInternal(false);
                return;
            }

            float distance = CombatDistance.GetPlanarSurfaceDistance(
                transform,
                playerCollider,
                target.transform,
                target.TargetCollider);

            if (distance > maximumTargetDistance)
            {
                ClearTargetInternal(false);
                return;
            }

            if (!isCombatActive)
            {
                return;
            }

            if (distance > stats.AttackRange)
            {
                MoveTowardTarget();
                return;
            }

            StopAgent();
            FaceTarget(target.transform.position);

            if (Time.time >= nextAttackTime)
            {
                nextAttackTime = Time.time + stats.AttackInterval;
                target.ReceiveAttack(this, stats.CalculateDamageAgainst(target.CombatStats));
            }
        }

        public void HandleEnemyClick(EnemyBrain enemy)
        {
            if (enemy == null || enemy.IsDead || IsInputLocked)
            {
                return;
            }

            if (target != enemy)
            {
                SetTarget(enemy);
                return;
            }

            isCombatActive = true;
            regeneration.SetRegenerationAllowed(false);
            MoveTowardTarget();
        }

        public void StopCombatAction()
        {
            bool wasCombatActive = isCombatActive;
            isCombatActive = false;
            regeneration ??= GetComponent<HealthRegeneration>();
            regeneration?.SetRegenerationAllowed(true, wasCombatActive);
            StopAgent();
        }

        public void ResumeCombatAfterSkill()
        {
            if (target == null || target.IsDead || IsInputLocked)
            {
                return;
            }

            isCombatActive = true;
            nextAttackTime = Time.time + stats.AttackInterval;
            regeneration.SetRegenerationAllowed(false);
            MoveTowardTarget();
        }

        public void ClearTarget()
        {
            ClearTargetInternal(true);
        }

        private void SetTarget(EnemyBrain enemy)
        {
            bool wasCombatActive = isCombatActive;

            if (target != null)
            {
                target.SetSelected(false);
            }

            UnsubscribeFromTarget();
            StopAgent();
            target = enemy;
            target.Died += HandleTargetDied;
            target.SetSelected(true);
            nextAttackTime = 0f;
            isCombatActive = false;
            regeneration ??= GetComponent<HealthRegeneration>();
            regeneration?.SetRegenerationAllowed(true, wasCombatActive);
            TargetChanged?.Invoke(target);
        }

        private void HandleTargetDied(EnemyBrain defeatedEnemy)
        {
            if (defeatedEnemy != target)
            {
                return;
            }

            progression.AddEnemyExperience(
                defeatedEnemy.EnemyLevel,
                defeatedEnemy.ExperienceReward);
            questLog?.ReportEnemyDefeated(defeatedEnemy.QuestTargetId);
            ClearTargetInternal(false);
        }

        private void ClearTargetInternal(bool stopMovement)
        {
            bool hadTarget = target != null;
            bool wasCombatActive = isCombatActive;

            if (target != null)
            {
                target.SetSelected(false);
            }

            UnsubscribeFromTarget();
            target = null;
            isCombatActive = false;
            regeneration ??= GetComponent<HealthRegeneration>();
            regeneration?.SetRegenerationAllowed(true, wasCombatActive);

            if (stopMovement)
            {
                StopAgent();
            }

            if (hadTarget)
            {
                TargetChanged?.Invoke(null);
            }
        }

        private void UnsubscribeFromTarget()
        {
            if (target != null)
            {
                target.Died -= HandleTargetDied;
            }
        }

        private void MoveTowardTarget()
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

            float desiredOffset = stats.AttackRange + agent.radius + approachPadding;
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

        private void HandlePlayerDied(Health _)
        {
            ClearTargetInternal(true);
            regeneration ??= GetComponent<HealthRegeneration>();
            regeneration?.SetRegenerationAllowed(false);
        }

        private void StopAgent()
        {
            if (agent != null && agent.enabled && agent.isOnNavMesh)
            {
                agent.ResetPath();
            }
        }
    }
}
