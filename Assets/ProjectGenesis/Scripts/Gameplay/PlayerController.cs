using UnityEngine;
using UnityEngine.InputSystem;

namespace ProjectGenesis.Gameplay
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float rotationSpeed = 12f;
        [SerializeField] private float gravity = -20f;
        [SerializeField] private float stoppingDistance = 0.15f;

        [Header("References")]
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private GameObject destinationMarker;

        private CharacterController characterController;
        private Vector3 verticalVelocity;
        private Vector3 destination;
        private bool hasDestination;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();

            if (cameraTransform == null && Camera.main != null)
            {
                cameraTransform = Camera.main.transform;
            }
        }

        private void Update()
        {
            TrySetClickDestination();

            Vector2 moveInput = ReadMoveInput();
            Vector3 moveDirection;

            if (moveInput.sqrMagnitude > 0.001f)
            {
                CancelClickMovement();
                moveDirection = GetCameraRelativeMoveDirection(moveInput);
            }
            else
            {
                moveDirection = GetClickMoveDirection();
            }

            if (moveDirection.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            if (characterController.isGrounded && verticalVelocity.y < 0f)
            {
                verticalVelocity.y = -2f;
            }

            verticalVelocity.y += gravity * Time.deltaTime;

            Vector3 velocity = moveDirection * moveSpeed + verticalVelocity;
            characterController.Move(velocity * Time.deltaTime);
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

            destination = closestHit.point;
            hasDestination = true;

            if (destinationMarker != null)
            {
                destinationMarker.transform.position = closestHit.point + Vector3.up * 0.03f;
                destinationMarker.SetActive(true);
            }
        }

        private Vector3 GetClickMoveDirection()
        {
            if (!hasDestination)
            {
                return Vector3.zero;
            }

            Vector3 toDestination = destination - transform.position;
            toDestination.y = 0f;

            if (toDestination.magnitude <= stoppingDistance)
            {
                CancelClickMovement();
                return Vector3.zero;
            }

            return toDestination.normalized;
        }

        private void CancelClickMovement()
        {
            hasDestination = false;

            if (destinationMarker != null)
            {
                destinationMarker.SetActive(false);
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
