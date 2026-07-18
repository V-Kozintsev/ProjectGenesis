using System;
using UnityEngine;
using UnityEngine.AI;

namespace ProjectGenesis.Gameplay
{
    public enum EnemyState
    {
        Idle,
        Chase,
        Attack,
        Return,
        Dead
    }

    [RequireComponent(typeof(NavMeshAgent), typeof(Health), typeof(CombatStats))]
    public sealed class EnemyBrain : MonoBehaviour
    {
        [Header("Behavior")]
        [SerializeField, Min(0.5f)] private float detectionRadius = 4f;
        [SerializeField, Min(1f)] private float leashRadius = 6f;
        [SerializeField, Min(1)] private int experienceReward = 20;
        [SerializeField, Min(0f)] private float corpseLifetime = 6f;
        [SerializeField] private string questTargetId = "wolf";

        [Header("References")]
        [SerializeField] private Transform player;
        [SerializeField] private GameObject visualRoot;
        [SerializeField] private GameObject selectionRing;

        private NavMeshAgent agent;
        private Health health;
        private CombatStats stats;
        private Health playerHealth;
        private CombatStats playerStats;
        private Collider targetCollider;
        private Collider playerCollider;
        private Vector3 homePosition;
        private Quaternion livingVisualRotation;
        private float nextAttackTime;
        private bool hasAggro;

        public event Action<EnemyBrain> Died;

        public EnemyState State { get; private set; } = EnemyState.Idle;
        public bool IsDead => health == null || health.IsDead;
        public int ExperienceReward => experienceReward;
        public string QuestTargetId => questTargetId;
        public Health Health => health;
        public CombatStats CombatStats => stats;
        public Collider TargetCollider => targetCollider;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
            stats = GetComponent<CombatStats>();
            targetCollider = GetComponent<Collider>();
            homePosition = transform.position;
            livingVisualRotation = visualRoot != null ? visualRoot.transform.localRotation : Quaternion.identity;
            health.Died += HandleDeath;

            if (player != null)
            {
                CachePlayerReferences();
            }

            SetSelected(false);
        }

        private void OnDestroy()
        {
            if (health != null)
            {
                health.Died -= HandleDeath;
            }
        }

        private void Update()
        {
            if (IsDead || agent == null || !agent.enabled || !agent.isOnNavMesh)
            {
                return;
            }

            if (player == null || playerHealth == null)
            {
                SetState(EnemyState.Idle);
                return;
            }

            if (playerHealth.IsDead)
            {
                BeginReturn();
                UpdateReturn();
                return;
            }

            float playerFromHome = GetPlanarDistance(homePosition, player.position);
            float playerDistance = GetDistanceToPlayer();

            if (State == EnemyState.Return)
            {
                UpdateReturn();
                return;
            }

            if (playerFromHome > leashRadius)
            {
                BeginReturn();
                return;
            }

            if (!hasAggro && playerDistance > detectionRadius)
            {
                SetState(EnemyState.Idle);
                StopAgent();
                return;
            }

            hasAggro = true;

            if (playerDistance > stats.AttackRange)
            {
                SetState(EnemyState.Chase);
                agent.SetDestination(player.position);
                return;
            }

            SetState(EnemyState.Attack);
            StopAgent();
            FacePlayer();

            if (Time.time >= nextAttackTime)
            {
                nextAttackTime = Time.time + stats.AttackInterval;
                playerHealth.TakeDamage(stats.CalculateDamageAgainst(playerStats));
            }
        }

        public void Configure(
            float detection,
            float leash,
            int reward,
            float despawnDelay = 6f,
            string objectiveTargetId = "wolf")
        {
            detectionRadius = Mathf.Max(0.5f, detection);
            leashRadius = Mathf.Max(1f, leash);
            experienceReward = Mathf.Max(1, reward);
            corpseLifetime = Mathf.Max(0f, despawnDelay);
            questTargetId = string.IsNullOrWhiteSpace(objectiveTargetId) ? "wolf" : objectiveTargetId;
        }

        public void SetPlayer(Transform playerTransform)
        {
            player = playerTransform;
            CachePlayerReferences();
        }

        public void SetVisuals(GameObject bodyVisual, GameObject ring)
        {
            visualRoot = bodyVisual;
            selectionRing = ring;
            livingVisualRotation = visualRoot != null ? visualRoot.transform.localRotation : Quaternion.identity;
            SetSelected(false);
        }

        public void SetSelected(bool selected)
        {
            if (selectionRing != null)
            {
                selectionRing.SetActive(selected && !IsDead);
            }
        }

        public void ReceiveAttack(PlayerCombatController attacker, int damage)
        {
            if (IsDead)
            {
                return;
            }

            if (attacker != null)
            {
                SetPlayer(attacker.transform);
            }

            hasAggro = true;
            health.TakeDamage(damage);
        }

        private void CachePlayerReferences()
        {
            playerHealth = player != null ? player.GetComponent<Health>() : null;
            playerStats = player != null ? player.GetComponent<CombatStats>() : null;
            playerCollider = player != null ? player.GetComponent<Collider>() : null;
        }

        private float GetDistanceToPlayer()
        {
            return CombatDistance.GetPlanarSurfaceDistance(
                transform,
                targetCollider,
                player,
                playerCollider);
        }

        private void BeginReturn()
        {
            hasAggro = false;
            SetState(EnemyState.Return);

            if (agent.enabled && agent.isOnNavMesh)
            {
                agent.SetDestination(homePosition);
            }
        }

        private void UpdateReturn()
        {
            if (GetPlanarDistance(transform.position, homePosition) > 0.25f)
            {
                agent.SetDestination(homePosition);
                return;
            }

            StopAgent();
            health.RestoreFull();
            SetState(EnemyState.Idle);
        }

        private void HandleDeath(Health _)
        {
            hasAggro = false;
            SetState(EnemyState.Dead);
            StopAgent();

            if (agent != null)
            {
                agent.enabled = false;
            }

            if (targetCollider != null)
            {
                targetCollider.enabled = false;
            }

            SetSelected(false);

            if (visualRoot != null)
            {
                visualRoot.transform.localRotation = livingVisualRotation * Quaternion.Euler(0f, 0f, 90f);
                visualRoot.transform.localPosition += Vector3.down * 0.2f;
            }

            Died?.Invoke(this);

            if (corpseLifetime > 0f)
            {
                Destroy(gameObject, corpseLifetime);
            }
        }

        private void FacePlayer()
        {
            Vector3 direction = player.position - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(direction.normalized, Vector3.up),
                    12f * Time.deltaTime);
            }
        }

        private void SetState(EnemyState nextState)
        {
            State = nextState;
        }

        private void StopAgent()
        {
            if (agent != null && agent.enabled && agent.isOnNavMesh)
            {
                agent.ResetPath();
            }
        }

        private static float GetPlanarDistance(Vector3 first, Vector3 second)
        {
            Vector3 offset = second - first;
            offset.y = 0f;
            return offset.magnitude;
        }
    }
}
