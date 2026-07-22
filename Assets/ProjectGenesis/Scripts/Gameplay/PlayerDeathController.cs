using ProjectGenesis.Core;
using ProjectGenesis.UI;
using UnityEngine;
using UnityEngine.AI;

namespace ProjectGenesis.Gameplay
{
    [DefaultExecutionOrder(140)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Health), typeof(PlayerProgression), typeof(HealthRegeneration))]
    [RequireComponent(typeof(NavMeshAgent))]
    public sealed class PlayerDeathController : MonoBehaviour
    {
        [SerializeField] private Transform respawnPoint;
        [SerializeField] private DeathRespawnView deathView;

        private Health health;
        private PlayerProgression progression;
        private HealthRegeneration regeneration;
        private NavMeshAgent agent;
        private PlayerCombatController combatController;
        private PlayerSkillController skillController;
        private PlayerLootController lootController;
        private PlayerInteractionController interactionController;
        private LocalMessageStream messageStream;
        private bool isDeathStateActive;
        private int lastExperienceLoss;
        private bool isSubscribed;

        public bool IsDeathStateActive => isDeathStateActive;
        public int LastExperienceLoss => lastExperienceLoss;
        public Transform RespawnPoint => respawnPoint;

        private void Awake()
        {
            EnsureDependencies();
        }

        private void OnDestroy()
        {
            UnsubscribeFromHealth();
        }

        public void SetRespawnPoint(Transform spawnPoint)
        {
            EnsureDependencies();
            respawnPoint = spawnPoint;
        }

        public void SetDeathView(DeathRespawnView view)
        {
            EnsureDependencies();
            deathView = view;
        }

        public void RespawnAtVillage()
        {
            EnsureDependencies();
            if (!isDeathStateActive)
            {
                return;
            }

            Vector3 destination = respawnPoint != null ? respawnPoint.position : transform.position;
            if (NavMesh.SamplePosition(destination, out NavMeshHit hit, 3f, NavMesh.AllAreas))
            {
                destination = hit.position;
            }

            if (agent != null && agent.enabled && agent.isOnNavMesh)
            {
                agent.Warp(destination);
                agent.ResetPath();
            }
            else
            {
                transform.position = destination;
            }

            health.RestoreFull();
            regeneration.SetRegenerationAllowed(true, true);
            isDeathStateActive = false;
            deathView?.Hide();
            messageStream?.Publish(
                LocalMessageCategory.System,
                "Персонаж воскрешён в деревне.");
        }

        private void HandlePlayerDied(Health _)
        {
            EnsureDependencies();
            if (isDeathStateActive)
            {
                return;
            }

            isDeathStateActive = true;
            lastExperienceLoss = progression.ApplyDeathPenalty();
            StopGameplayActions();
            regeneration.SetRegenerationAllowed(false);
            deathView?.Show(lastExperienceLoss, RespawnAtVillage);
            messageStream?.Publish(
                LocalMessageCategory.Combat,
                $"Персонаж погиб. Потеря опыта: {lastExperienceLoss}.");
        }

        private void StopGameplayActions()
        {
            combatController?.ClearTarget();
            combatController?.StopCombatAction();
            skillController?.CancelSkillAction();
            lootController?.CancelLootAction();
            interactionController?.ClearSelection();

            if (agent != null && agent.enabled && agent.isOnNavMesh)
            {
                agent.ResetPath();
            }
        }

        private void EnsureDependencies()
        {
            health ??= GetComponent<Health>();
            progression ??= GetComponent<PlayerProgression>();
            regeneration ??= GetComponent<HealthRegeneration>();
            agent ??= GetComponent<NavMeshAgent>();
            combatController ??= GetComponent<PlayerCombatController>();
            skillController ??= GetComponent<PlayerSkillController>();
            lootController ??= GetComponent<PlayerLootController>();
            interactionController ??= GetComponent<PlayerInteractionController>();
            messageStream ??= GetComponent<LocalMessageStream>();

            if (!isSubscribed && health != null)
            {
                health.Died += HandlePlayerDied;
                isSubscribed = true;
            }

            if (respawnPoint == null)
            {
                PlayerSpawnPoint spawn = FindFirstObjectByType<PlayerSpawnPoint>();
                respawnPoint = spawn != null ? spawn.transform : null;
            }
        }

        private void UnsubscribeFromHealth()
        {
            if (isSubscribed && health != null)
            {
                health.Died -= HandlePlayerDied;
            }

            isSubscribed = false;
        }
    }
}
