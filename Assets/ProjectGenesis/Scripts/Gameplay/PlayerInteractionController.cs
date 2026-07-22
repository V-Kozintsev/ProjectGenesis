using System;
using ProjectGenesis.Core;
using ProjectGenesis.UI;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace ProjectGenesis.Gameplay
{
    [DefaultExecutionOrder(100)]
    public sealed class PlayerInteractionController : MonoBehaviour
    {
        [Header("Interaction")]
        [Tooltip("Maximum gap between the player and NPC colliders for interaction.")]
        [SerializeField, Min(0.1f)] private float interactionRadius = 1.25f;
        [SerializeField, Min(0.25f)] private float approachDistance = 0.5f;
        [SerializeField, Min(1f)] private float clickApproachMaxDistance = 20f;

        [Header("References")]
        [SerializeField] private DialogueWindow dialogueWindow;
        [SerializeField] private InteractionPromptView promptView;
        [SerializeField] private Camera gameplayCamera;

        private QuestLog questLog;
        private Health health;
        private NavMeshAgent agent;
        private Collider playerCollider;
        private NavMeshPath approachPath;
        private InteractableNpc nearestNpc;
        private InteractableNpc selectedNpc;
        private InteractableNpc pendingInteractionNpc;
        private bool shouldOpenDialogueAfterApproach;

        public event Action<InteractableNpc> SelectionChanged;

        public InteractableNpc SelectedNpc => selectedNpc;

        private void Awake()
        {
            questLog = GetComponent<QuestLog>();
            health = GetComponent<Health>();
            if (questLog == null)
            {
                questLog = gameObject.AddComponent<QuestLog>();
            }

            if (gameplayCamera == null)
            {
                gameplayCamera = Camera.main;
            }

            agent = GetComponent<NavMeshAgent>();
            playerCollider = GetComponent<Collider>();
            approachPath = new NavMeshPath();
        }

        private void Update()
        {
            if (health != null && health.IsDead)
            {
                CancelPendingInteraction(true);
                promptView?.Hide();
                return;
            }

            CancelPendingInteractionOnManualMove();
            nearestNpc = FindNearestNpc();
            ClearSelectionWhenTooFar();
            CloseDialogueWhenTooFar();
            TryOpenPendingInteraction();

            if (nearestNpc != null)
            {
                promptView?.Show(nearestNpc.DisplayName);
            }
            else
            {
                promptView?.Hide();
            }

            Keyboard keyboard = Keyboard.current;
            if (!GameplayInputGate.IsTextEntryFocused && nearestNpc != null &&
                keyboard != null && keyboard.eKey.wasPressedThisFrame)
            {
                nearestNpc.Interact(questLog, dialogueWindow, CanInteractWith);
            }

        }

        public void SetDialogueWindow(DialogueWindow window)
        {
            dialogueWindow = window;
        }

        public void SetPromptView(InteractionPromptView view)
        {
            promptView = view;
        }

        public void SetGameplayCamera(Camera camera)
        {
            gameplayCamera = camera;
        }

        public void CancelForMovement()
        {
            CancelPendingInteraction(false);
        }

        public void CancelForCombat()
        {
            CancelPendingInteraction(true);
            ClearNpcSelection();
            dialogueWindow?.Hide();
        }

        public void ClearSelection()
        {
            CancelPendingInteraction(true);
            ClearNpcSelection();
            dialogueWindow?.Hide();
        }

        public void HandleNpcClick(InteractableNpc clickedNpc)
        {
            if (clickedNpc == null || (health != null && health.IsDead))
            {
                return;
            }

            if (clickedNpc != selectedNpc)
            {
                CancelPendingInteraction(true);
                SetSelectedNpc(clickedNpc);
                dialogueWindow?.Hide();
                promptView?.ShowMessage($"Выбран NPC: {clickedNpc.DisplayName}");
                return;
            }

            float distance = GetPlanarDistance(clickedNpc);
            if (distance <= interactionRadius)
            {
                CancelPendingInteraction(true);
                clickedNpc.Interact(questLog, dialogueWindow, CanInteractWith);
            }
            else if (distance <= clickApproachMaxDistance)
            {
                StartApproachInteraction(clickedNpc, true);
            }
            else
            {
                promptView?.ShowMessage($"NPC слишком далеко: {clickedNpc.DisplayName}");
            }
        }

        private void StartApproachInteraction(InteractableNpc npc, bool openDialogueWhenClose)
        {
            if (agent == null || !agent.enabled || !agent.isOnNavMesh)
            {
                promptView?.ShowMessage($"Подойди ближе к NPC: {npc.DisplayName}");
                return;
            }

            Vector3 fromNpcToPlayer = transform.position - npc.transform.position;
            fromNpcToPlayer.y = 0f;

            if (fromNpcToPlayer.sqrMagnitude <= 0.001f)
            {
                fromNpcToPlayer = -npc.transform.forward;
            }

            Vector3 approachPoint = npc.transform.position + fromNpcToPlayer.normalized * approachDistance;
            if (!NavMesh.SamplePosition(approachPoint, out NavMeshHit navMeshHit, 2f, NavMesh.AllAreas))
            {
                promptView?.ShowMessage($"Не могу подойти к NPC: {npc.DisplayName}");
                return;
            }

            if (!agent.CalculatePath(navMeshHit.position, approachPath) || approachPath.status != NavMeshPathStatus.PathComplete)
            {
                promptView?.ShowMessage($"Путь к NPC недоступен: {npc.DisplayName}");
                return;
            }

            pendingInteractionNpc = npc;
            shouldOpenDialogueAfterApproach = openDialogueWhenClose;
            agent.SetPath(approachPath);
            promptView?.ShowMessage(openDialogueWhenClose ? $"Иду говорить с NPC: {npc.DisplayName}" : $"Иду к NPC: {npc.DisplayName}");
        }

        private void TryOpenPendingInteraction()
        {
            if (pendingInteractionNpc == null)
            {
                return;
            }

            if (shouldOpenDialogueAfterApproach && CanInteractWith(pendingInteractionNpc))
            {
                InteractableNpc npc = pendingInteractionNpc;
                CancelPendingInteraction(true);
                npc.Interact(questLog, dialogueWindow, CanInteractWith);
            }
        }

        private void CancelPendingInteraction(bool resetAgentPath)
        {
            pendingInteractionNpc = null;
            shouldOpenDialogueAfterApproach = false;

            if (resetAgentPath && agent != null && agent.enabled && agent.isOnNavMesh)
            {
                agent.ResetPath();
            }
        }

        private void SetSelectedNpc(InteractableNpc npc)
        {
            if (selectedNpc != null)
            {
                selectedNpc.SetSelected(false);
            }

            selectedNpc = npc;
            selectedNpc?.SetSelected(true);
            SelectionChanged?.Invoke(selectedNpc);
        }

        private void ClearNpcSelection()
        {
            bool hadSelection = selectedNpc != null;

            if (selectedNpc != null)
            {
                selectedNpc.SetSelected(false);
            }

            selectedNpc = null;

            if (hadSelection)
            {
                SelectionChanged?.Invoke(null);
            }
        }

        private void CancelPendingInteractionOnManualMove()
        {
            if (GameplayInputGate.IsTextEntryFocused)
            {
                return;
            }

            Keyboard keyboard = Keyboard.current;
            if (keyboard == null || pendingInteractionNpc == null)
            {
                return;
            }

            bool hasManualMove =
                keyboard.wKey.isPressed ||
                keyboard.aKey.isPressed ||
                keyboard.sKey.isPressed ||
                keyboard.dKey.isPressed ||
                keyboard.upArrowKey.isPressed ||
                keyboard.leftArrowKey.isPressed ||
                keyboard.downArrowKey.isPressed ||
                keyboard.rightArrowKey.isPressed;

            if (hasManualMove)
            {
                CancelPendingInteraction(false);
            }
        }

        private void CloseDialogueWhenTooFar()
        {
            if (dialogueWindow == null || !dialogueWindow.IsVisible || dialogueWindow.CurrentNpc == null)
            {
                return;
            }

            if (!CanInteractWith(dialogueWindow.CurrentNpc))
            {
                dialogueWindow.Hide();
            }
        }

        private void ClearSelectionWhenTooFar()
        {
            if (selectedNpc == null || GetPlanarDistance(selectedNpc) <= clickApproachMaxDistance)
            {
                return;
            }

            CancelPendingInteraction(true);
            ClearNpcSelection();
            dialogueWindow?.Hide();
        }

        private bool CanInteractWith(InteractableNpc npc)
        {
            return npc != null && GetPlanarDistance(npc) <= interactionRadius;
        }

        private InteractableNpc FindNearestNpc()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, interactionRadius + 2f, ~0, QueryTriggerInteraction.Collide);
            InteractableNpc bestNpc = null;
            float bestDistance = float.MaxValue;

            foreach (Collider hit in hits)
            {
                InteractableNpc npc = hit.GetComponentInParent<InteractableNpc>();
                if (npc == null)
                {
                    continue;
                }

                float distance = GetPlanarDistance(npc);
                if (distance > interactionRadius)
                {
                    continue;
                }

                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestNpc = npc;
                }
            }

            return bestNpc;
        }

        private float GetPlanarDistance(InteractableNpc npc)
        {
            Collider npcCollider = npc.GetComponentInChildren<Collider>();
            if (playerCollider == null || npcCollider == null)
            {
                Vector3 fallbackDistance = npc.transform.position - transform.position;
                fallbackDistance.y = 0f;
                return fallbackDistance.magnitude;
            }

            Bounds playerBounds = playerCollider.bounds;
            Bounds npcBounds = npcCollider.bounds;
            Vector3 toNpc = npcBounds.center - playerBounds.center;
            toNpc.y = 0f;

            float playerRadius = Mathf.Max(playerBounds.extents.x, playerBounds.extents.z);
            float npcRadius = Mathf.Max(npcBounds.extents.x, npcBounds.extents.z);
            return Mathf.Max(0f, toNpc.magnitude - playerRadius - npcRadius);
        }
    }
}
