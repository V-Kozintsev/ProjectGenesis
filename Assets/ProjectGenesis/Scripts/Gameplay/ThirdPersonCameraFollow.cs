using UnityEngine;
using UnityEngine.InputSystem;

namespace ProjectGenesis.Gameplay
{
    public sealed class ThirdPersonCameraFollow : MonoBehaviour
    {
        [Header("Follow")]
        [SerializeField] private Transform target;
        [SerializeField] private float followSharpness = 10f;
        [SerializeField] private float lookHeight = 1.3f;

        [Header("Orbit")]
        [SerializeField] private float rotationSensitivity = 0.18f;
        [SerializeField] private float minimumPitch = 8f;
        [SerializeField] private float maximumPitch = 70f;
        [SerializeField] private float defaultPitch = 28f;
        [SerializeField] private float rightClickDragThreshold = 6f;
        [SerializeField] private bool invertVertical;

        [Header("Zoom")]
        [SerializeField] private float distance = 6.8f;
        [SerializeField] private float minimumDistance = 3f;
        [SerializeField] private float maximumDistance = 10f;
        [SerializeField, Range(0.25f, 3f)] private float zoomStep = 1.25f;

        [Header("Collision")]
        [SerializeField] private float collisionRadius = 0.25f;
        [SerializeField] private float collisionOffset = 0.2f;
        [SerializeField] private LayerMask collisionMask = ~0;

        private float yaw;
        private float pitch;
        private bool isOrbitInitialized;
        private float rightDragDistance;
        private bool isRightDragging;
        private bool isFrontView;

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            if (!isOrbitInitialized)
            {
                InitializeOrbitFromCurrentPosition();
            }

            ReadCameraInput();

            Vector3 focusPoint = target.position + Vector3.up * lookHeight;
            Quaternion orbitRotation = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 desiredPosition = focusPoint - orbitRotation * Vector3.forward * distance;
            desiredPosition = ResolveCameraCollision(focusPoint, desiredPosition);
            float followBlend = 1f - Mathf.Exp(-followSharpness * Time.deltaTime);

            transform.position = Vector3.Lerp(transform.position, desiredPosition, followBlend);
            transform.rotation = Quaternion.LookRotation(focusPoint - transform.position, Vector3.up);
        }

        public void SetTarget(Transform followTarget)
        {
            target = followTarget;
            isOrbitInitialized = false;
        }

        private void ReadCameraInput()
        {
            Mouse mouse = Mouse.current;
            if (mouse == null)
            {
                return;
            }

            if (mouse.middleButton.wasPressedThisFrame)
            {
                ToggleFrontView();
            }

            if (mouse.rightButton.wasPressedThisFrame)
            {
                rightDragDistance = 0f;
                isRightDragging = false;
            }

            if (mouse.rightButton.isPressed && !mouse.rightButton.wasPressedThisFrame)
            {
                Vector2 lookDelta = mouse.delta.ReadValue();
                rightDragDistance += lookDelta.magnitude;
                isRightDragging |= rightDragDistance >= rightClickDragThreshold;

                yaw += lookDelta.x * rotationSensitivity;

                float verticalDirection = invertVertical ? 1f : -1f;
                pitch += lookDelta.y * rotationSensitivity * verticalDirection;
                pitch = Mathf.Clamp(pitch, minimumPitch, maximumPitch);
                isFrontView = false;
            }

            if (mouse.rightButton.wasReleasedThisFrame && !isRightDragging)
            {
                ResetBehindTarget();
            }

            float scrollDelta = mouse.scroll.ReadValue().y;
            if (Mathf.Abs(scrollDelta) > 0.01f)
            {
                float scrollDirection = Mathf.Sign(scrollDelta);
                distance = Mathf.Clamp(
                    distance - scrollDirection * zoomStep,
                    minimumDistance,
                    maximumDistance);
            }
        }

        private void InitializeOrbitFromCurrentPosition()
        {
            Vector3 focusPoint = target.position + Vector3.up * lookHeight;
            Vector3 cameraOffset = transform.position - focusPoint;

            if (cameraOffset.sqrMagnitude < 0.01f)
            {
                cameraOffset = new Vector3(0f, 3.2f, -6f);
            }

            distance = Mathf.Clamp(cameraOffset.magnitude, minimumDistance, maximumDistance);

            Quaternion currentLookRotation = Quaternion.LookRotation(-cameraOffset.normalized, Vector3.up);
            yaw = currentLookRotation.eulerAngles.y;
            pitch = Mathf.Clamp(NormalizeAngle(currentLookRotation.eulerAngles.x), minimumPitch, maximumPitch);
            isOrbitInitialized = true;
        }

        private void ToggleFrontView()
        {
            isFrontView = !isFrontView;
            yaw = target.eulerAngles.y + (isFrontView ? 180f : 0f);
            pitch = Mathf.Clamp(defaultPitch, minimumPitch, maximumPitch);
        }

        private void ResetBehindTarget()
        {
            isFrontView = false;
            yaw = target.eulerAngles.y;
            pitch = Mathf.Clamp(defaultPitch, minimumPitch, maximumPitch);
        }

        private Vector3 ResolveCameraCollision(Vector3 focusPoint, Vector3 desiredPosition)
        {
            Vector3 focusToCamera = desiredPosition - focusPoint;
            float targetDistance = focusToCamera.magnitude;

            if (targetDistance <= 0.01f)
            {
                return desiredPosition;
            }

            Vector3 direction = focusToCamera / targetDistance;
            if (Physics.SphereCast(
                    focusPoint,
                    collisionRadius,
                    direction,
                    out RaycastHit hit,
                    targetDistance,
                    collisionMask,
                    QueryTriggerInteraction.Ignore))
            {
                float safeDistance = Mathf.Max(hit.distance - collisionOffset, minimumDistance * 0.35f);
                return focusPoint + direction * safeDistance;
            }

            return desiredPosition;
        }

        private static float NormalizeAngle(float angle)
        {
            return angle > 180f ? angle - 360f : angle;
        }
    }
}
