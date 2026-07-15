using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace ProjectGenesis.Gameplay
{
    [RequireComponent(typeof(NavMeshAgent))]
    public sealed class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float rotationSpeed = 12f;
        [SerializeField] private float stoppingDistance = 0.25f;
        [SerializeField] private float manualMoveSampleDistance = 1.25f;

        [Header("References")]
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private GameObject destinationMarker;

        private NavMeshAgent agent;
        private NavMeshPath clickPath;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            if (agent == null)
            {
                agent = gameObject.AddComponent<NavMeshAgent>();
            }

            agent.speed = moveSpeed;
            agent.angularSpeed = 720f;
            agent.acceleration = 24f;
            agent.stoppingDistance = stoppingDistance;
            agent.updateRotation = false;
            clickPath = new NavMeshPath();

            if (cameraTransform == null && Camera.main != null)
            {
                cameraTransform = Camera.main.transform;
            }
        }

        private void Update()
        {
            TrySetClickDestination();

            Vector2 moveInput = ReadMoveInput();

            if (moveInput.sqrMagnitude > 0.001f)
            {
                CancelClickMovement();
                MoveManually(GetCameraRelativeMoveDirection(moveInput));
            }
            else if (agent.isOnNavMesh && agent.hasPath && agent.remainingDistance <= agent.stoppingDistance)
            {
                CancelClickMovement();
            }

            Vector3 planarVelocity = agent.velocity;
            planarVelocity.y = 0f;

            if (planarVelocity.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(planarVelocity.normalized, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        public void SetCameraTransform(Transform followCamera)
        {
            cameraTransform = followCamera;
        }

        public void SetDestinationMarker(GameObject marker)
        {
            destinationMarker = marker;

            if (destinationMarker != null)
            {
                destinationMarker.SetActive(false);
            }
        }

        private void TrySetClickDestination()
        {
            Mouse mouse = Mouse.current;
            if (mouse == null || !mouse.leftButton.wasPressedThisFrame || cameraTransform == null)
            {
                return;
            }

            Camera gameplayCamera = cameraTransform.GetComponent<Camera>();
            if (gameplayCamera == null)
            {
                return;
            }

            Ray ray = gameplayCamera.ScreenPointToRay(mouse.position.ReadValue());
            RaycastHit[] hits = Physics.RaycastAll(ray, gameplayCamera.farClipPlane, ~0, QueryTriggerInteraction.Ignore);

            bool foundWalkableSurface = false;
            RaycastHit closestHit = default;

            foreach (RaycastHit hit in hits)
            {
                bool hitPlayer = hit.transform == transform || hit.transform.IsChildOf(transform);
                bool isWalkable = Vector3.Dot(hit.normal, Vector3.up) >= 0.6f;

                if (hitPlayer || !isWalkable)
                {
                    continue;
                }

                if (!foundWalkableSurface || hit.distance < closestHit.distance)
                {
                    foundWalkableSurface = true;
                    closestHit = hit;
                }
            }

            if (!foundWalkableSurface)
            {
                return;
            }

            if (!TrySetReachableDestination(closestHit.point))
            {
                CancelClickMovement();
                return;
            }

            if (destinationMarker != null)
            {
                destinationMarker.transform.position = agent.destination + Vector3.up * 0.03f;
                destinationMarker.SetActive(true);
            }
        }

        private bool TrySetReachableDestination(Vector3 requestedPoint)
        {
            if (!NavMesh.SamplePosition(requestedPoint, out NavMeshHit navMeshHit, 1.5f, NavMesh.AllAreas))
            {
                return false;
            }

            if (!agent.isOnNavMesh)
            {
                return false;
            }

            if (!agent.CalculatePath(navMeshHit.position, clickPath) || clickPath.status != NavMeshPathStatus.PathComplete)
            {
                return false;
            }

            agent.SetPath(clickPath);
            return true;
        }

        private void CancelClickMovement()
        {
            if (agent != null && agent.enabled && agent.isOnNavMesh)
            {
                agent.ResetPath();
            }

            if (destinationMarker != null)
            {
                destinationMarker.SetActive(false);
            }
        }

        private void MoveManually(Vector3 moveDirection)
        {
            if (moveDirection.sqrMagnitude <= 0.001f || !agent.isOnNavMesh)
            {
                return;
            }

            Vector3 desiredPosition = transform.position + moveDirection * moveSpeed * Time.deltaTime;
            if (NavMesh.SamplePosition(desiredPosition, out NavMeshHit navMeshHit, manualMoveSampleDistance, NavMesh.AllAreas))
            {
                agent.Move(navMeshHit.position - transform.position);
            }
        }

        private static Vector2 ReadMoveInput()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return Vector2.zero;
            }

            Vector2 input = Vector2.zero;

            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
            {
                input.y += 1f;
            }

            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
            {
                input.y -= 1f;
            }

            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
            {
                input.x += 1f;
            }

            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
            {
                input.x -= 1f;
            }

            return Vector2.ClampMagnitude(input, 1f);
        }

        private Vector3 GetCameraRelativeMoveDirection(Vector2 moveInput)
        {
            if (moveInput.sqrMagnitude <= 0.001f)
            {
                return Vector3.zero;
            }

            Transform referenceTransform = cameraTransform != null ? cameraTransform : transform;
            Vector3 forward = referenceTransform.forward;
            Vector3 right = referenceTransform.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            return (forward * moveInput.y + right * moveInput.x).normalized;
        }
    }
}
