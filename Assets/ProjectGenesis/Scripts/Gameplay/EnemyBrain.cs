using System;
using UnityEngine;
using UnityEngine.AI;

namespace ProjectGenesis.Gameplay
{
    public enum EnemyState
    {
        Idle,
        Roam,
        Chase,
        Attack,
        Return,
        Dead
    }

    [RequireComponent(typeof(NavMeshAgent), typeof(Health), typeof(CombatStats))]
    [RequireComponent(typeof(HealthRegeneration))]
    public sealed class EnemyBrain : MonoBehaviour
    {
        [Header("Behavior")]
        [SerializeField] private string displayName = "Молодой волк";
        [SerializeField, Min(1)] private int enemyLevel = 1;
        [SerializeField, Min(0.5f)] private float detectionRadius = 4f;
        [SerializeField, Min(1f)] private float leashRadius = 6f;
        [SerializeField, Min(1)] private int experienceReward = 20;
        [SerializeField, Min(0f)] private float corpseLifetime = 6f;
        [SerializeField] private string questTargetId = "wolf";

        [Header("Roaming")]
        [SerializeField, Min(0f)] private float roamingRadius = 1.4f;
        [SerializeField, Min(0f)] private float minimumIdleDelay = 1.5f;
        [SerializeField, Min(0f)] private float maximumIdleDelay = 4f;

        [Header("References")]
        [SerializeField] private Transform player;
        [SerializeField] private GameObject visualRoot;
        [SerializeField] private GameObject selectionRing;

        private NavMeshAgent agent;
        private Health health;
        private HealthRegeneration regeneration;
        private CombatStats stats;
        private Health playerHealth;
        private CombatStats playerStats;
        private Collider targetCollider;
        private Collider playerCollider;
        private EnemyTerritory territory;
        private NavMeshPath roamingPath;
        private Vector3 homePosition;
        private Quaternion livingVisualRotation;
        private float nextAttackTime;
        private float nextRoamTime;
        private bool hasAggro;

        public event Action<EnemyBrain> Died;

        public EnemyState State { get; private set; } = EnemyState.Idle;
        public bool IsDead => health == null || health.IsDead;
        public string DisplayName => displayName;
        public int EnemyLevel => enemyLevel;
        public int ExperienceReward => experienceReward;
        public string QuestTargetId => questTargetId;
        public Health Health => health;
        public CombatStats CombatStats => stats;
        public Collider TargetCollider => targetCollider;
        public float DetectionRadius => detectionRadius;
        public float LeashRadius => leashRadius;
        public float CorpseLifetime => corpseLifetime;
        public float RoamingRadius => roamingRadius;
        public float MinimumIdleDelay => minimumIdleDelay;
        public float MaximumIdleDelay => maximumIdleDelay;
        public Vector3 HomePosition => homePosition;
        public EnemyTerritory Territory => territory;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
            regeneration = GetComponent<HealthRegeneration>();
            stats = GetComponent<CombatStats>();
            targetCollider = GetComponent<Collider>();
            roamingPath = new NavMeshPath();
            homePosition = transform.position;
            livingVisualRotation = visualRoot != null ? visualRoot.transform.localRotation : Quaternion.identity;
            health.Died += HandleDeath;
            regeneration.SetRegenerationAllowed(false);
            ScheduleNextRoam();

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
                if (State == EnemyState.Return)
                {
                    UpdateReturn();
                }
                else
                {
                    hasAggro = false;
                    UpdateRoaming();
                }

                return;
            }

            float playerFromHome = GetPlanarDistance(homePosition, player.position);
            float distanceFromHome = GetPlanarDistance(homePosition, transform.position);
            float playerDistance = GetDistanceToPlayer();
            bool playerInsideTerritory = territory == null || territory.Contains(player.position);
            bool enemyInsideTerritory = territory == null || territory.Contains(transform.position);

            if (playerHealth.IsDead)
            {
                if (State == EnemyState.Return)
                {
                    UpdateReturn();
                }
                else if (hasAggro || distanceFromHome > GetReturnArrivalDistance())
                {
                    BeginReturn();
                }
                else
                {
                    hasAggro = false;
                    UpdateRoaming();
                }

                return;
            }

            if (State == EnemyState.Return)
            {
                if (playerInsideTerritory &&
                    playerFromHome <= leashRadius &&
                    playerDistance <= detectionRadius)
                {
                    hasAggro = true;
                    regeneration.SetRegenerationAllowed(true);
                }
                else
                {
                    UpdateReturn();
                    return;
                }
            }

            if (hasAggro &&
                (!playerInsideTerritory || !enemyInsideTerritory || distanceFromHome > leashRadius))
            {
                BeginReturn();
                return;
            }

            if (!hasAggro && (!playerInsideTerritory || playerDistance > detectionRadius))
            {
                UpdateRoaming();
                return;
            }

            hasAggro = true;
            regeneration.SetRegenerationAllowed(true);

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
            string objectiveTargetId = "wolf",
            float roamDistance = 1.4f,
            float minimumIdleDelay = 1.5f,
            float maximumIdleDelay = 4f,
            int level = 1,
            string enemyName = "Молодой волк")
        {
            displayName = string.IsNullOrWhiteSpace(enemyName) ? "Противник" : enemyName;
            enemyLevel = Mathf.Max(1, level);
            detectionRadius = Mathf.Max(0.5f, detection);
            leashRadius = Mathf.Max(1f, leash);
            experienceReward = Mathf.Max(1, reward);
            corpseLifetime = Mathf.Max(0f, despawnDelay);
            questTargetId = string.IsNullOrWhiteSpace(objectiveTargetId) ? "wolf" : objectiveTargetId;
            roamingRadius = Mathf.Clamp(roamDistance, 0f, leashRadius);
            this.minimumIdleDelay = Mathf.Max(0f, minimumIdleDelay);
            this.maximumIdleDelay = Mathf.Max(this.minimumIdleDelay, maximumIdleDelay);
        }

        public void SetPlayer(Transform playerTransform)
        {
            player = playerTransform;
            CachePlayerReferences();
        }

        public void SetHomePosition(Vector3 position)
        {
            homePosition = position;
            ScheduleNextRoam();
        }

        public void SetTerritory(EnemyTerritory enemyTerritory)
        {
            territory = enemyTerritory;
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
            StopAgent();
            regeneration.SetRegenerationAllowed(true);
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
            regeneration.SetRegenerationAllowed(false);
            SetState(EnemyState.Return);

            if (agent.enabled && agent.isOnNavMesh)
            {
                agent.SetDestination(homePosition);
            }
        }

        private void UpdateReturn()
        {
            bool reachedHome =
                GetPlanarDistance(transform.position, homePosition) <= GetReturnArrivalDistance();

            if (!reachedHome)
            {
                agent.SetDestination(homePosition);
                return;
            }

            StopAgent();
            SetState(EnemyState.Idle);
            regeneration.SetRegenerationAllowed(true, true);
            ScheduleNextRoam();
        }

        private void UpdateRoaming()
        {
            if (State == EnemyState.Return || State == EnemyState.Dead)
            {
                return;
            }

            if (territory != null && !territory.Contains(transform.position))
            {
                BeginReturn();
                return;
            }

            if (State == EnemyState.Roam)
            {
                float arrivalDistance = Mathf.Max(0.25f, agent.stoppingDistance + 0.1f);
                if (agent.pathPending ||
                    (agent.hasPath && agent.remainingDistance > arrivalDistance))
                {
                    return;
                }

                StopAgent();
                SetState(EnemyState.Idle);
                ScheduleNextRoam();
                return;
            }

            SetState(EnemyState.Idle);

            if (roamingRadius <= 0f || Time.time < nextRoamTime)
            {
                return;
            }

            if (!TryStartRoaming())
            {
                ScheduleNextRoam();
            }
        }

        private bool TryStartRoaming()
        {
            const int maximumAttempts = 8;
            float minimumTravelDistance = Mathf.Max(0.35f, agent.stoppingDistance + 0.2f);

            for (int attempt = 0; attempt < maximumAttempts; attempt++)
            {
                Vector2 offset = UnityEngine.Random.insideUnitCircle * roamingRadius;
                Vector3 candidate = homePosition + new Vector3(offset.x, 0f, offset.y);

                if (GetPlanarDistance(transform.position, candidate) < minimumTravelDistance ||
                    (territory != null && !territory.Contains(candidate)))
                {
                    continue;
                }

                if (!NavMesh.SamplePosition(candidate, out NavMeshHit hit, 0.8f, agent.areaMask) ||
                    GetPlanarDistance(homePosition, hit.position) > roamingRadius + 0.1f ||
                    (territory != null && !territory.Contains(hit.position)))
                {
                    continue;
                }

                if (!agent.CalculatePath(hit.position, roamingPath) ||
                    roamingPath.status != NavMeshPathStatus.PathComplete ||
                    !IsPathInsideTerritory(roamingPath))
                {
                    continue;
                }

                agent.SetDestination(hit.position);
                SetState(EnemyState.Roam);
                return true;
            }

            return false;
        }

        private bool IsPathInsideTerritory(NavMeshPath path)
        {
            if (territory == null)
            {
                return true;
            }

            foreach (Vector3 corner in path.corners)
            {
                if (!territory.Contains(corner))
                {
                    return false;
                }
            }

            return true;
        }

        private float GetReturnArrivalDistance()
        {
            return Mathf.Max(0.35f, agent.stoppingDistance + 0.1f);
        }

        private void ScheduleNextRoam()
        {
            float minimum = Mathf.Max(0f, minimumIdleDelay);
            float maximum = Mathf.Max(minimum, maximumIdleDelay);
            nextRoamTime = Time.time + UnityEngine.Random.Range(minimum, maximum);
        }

        private void HandleDeath(Health _)
        {
            hasAggro = false;
            regeneration.SetRegenerationAllowed(false);
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
