using ProjectGenesis.Core;
using ProjectGenesis.Gameplay;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace ProjectGenesis.UI
{
    public sealed class WorldMapView : MonoBehaviour
    {
        [SerializeField] private RectTransform miniMapRoot;
        [SerializeField] private RectTransform largeMapRoot;
        [SerializeField] private RectTransform miniPlayerMarker;
        [SerializeField] private RectTransform largePlayerMarker;
        [SerializeField] private Text miniZoneText;
        [SerializeField] private Text largeZoneText;
        [SerializeField] private Button closeLargeMapButton;
        [SerializeField] private Transform player;
        [SerializeField] private PlayerZoneController zoneController;
        [SerializeField] private Vector2 worldMinimum = new(-8.5f, -8.5f);
        [SerializeField] private Vector2 worldMaximum = new(18.5f, 18.5f);

        public RectTransform MiniMapRoot => miniMapRoot;
        public RectTransform LargeMapRoot => largeMapRoot;
        public RectTransform MiniPlayerMarker => miniPlayerMarker;
        public RectTransform LargePlayerMarker => largePlayerMarker;
        public Text MiniZoneText => miniZoneText;
        public Text LargeZoneText => largeZoneText;
        public Vector2 WorldMinimum => worldMinimum;
        public Vector2 WorldMaximum => worldMaximum;
        public bool IsLargeMapOpen => largeMapRoot != null && largeMapRoot.gameObject.activeSelf;

        public void Initialize(
            RectTransform miniRoot,
            RectTransform largeRoot,
            RectTransform miniMarker,
            RectTransform largeMarker,
            Text miniZoneLabel,
            Text largeZoneLabel,
            Button closeButton,
            Transform playerTransform,
            PlayerZoneController playerZoneController,
            Vector2 minimum,
            Vector2 maximum)
        {
            miniMapRoot = miniRoot;
            largeMapRoot = largeRoot;
            miniPlayerMarker = miniMarker;
            largePlayerMarker = largeMarker;
            miniZoneText = miniZoneLabel;
            largeZoneText = largeZoneLabel;
            closeLargeMapButton = closeButton;
            player = playerTransform;
            zoneController = playerZoneController;
            worldMinimum = minimum;
            worldMaximum = maximum;
        }

        private void Awake()
        {
            closeLargeMapButton?.onClick.AddListener(CloseLargeMap);
            CloseLargeMap();
            Refresh();
        }

        private void OnDestroy()
        {
            closeLargeMapButton?.onClick.RemoveListener(CloseLargeMap);
        }

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (!GameplayInputGate.IsTextEntryFocused && keyboard != null &&
                keyboard.mKey.wasPressedThisFrame)
            {
                ToggleLargeMap();
            }

            Refresh();
        }

        public void ToggleLargeMap()
        {
            if (largeMapRoot == null)
            {
                return;
            }

            largeMapRoot.gameObject.SetActive(!largeMapRoot.gameObject.activeSelf);
            Refresh();
        }

        public void CloseLargeMap()
        {
            if (largeMapRoot != null)
            {
                largeMapRoot.gameObject.SetActive(false);
            }
        }

        public Vector2 WorldToMapPosition(Vector3 worldPosition, Vector2 mapSize)
        {
            float width = Mathf.Max(0.01f, worldMaximum.x - worldMinimum.x);
            float height = Mathf.Max(0.01f, worldMaximum.y - worldMinimum.y);
            float normalizedX = Mathf.Clamp01((worldPosition.x - worldMinimum.x) / width);
            float normalizedY = Mathf.Clamp01((worldPosition.z - worldMinimum.y) / height);
            return new Vector2(
                (normalizedX - 0.5f) * mapSize.x,
                (normalizedY - 0.5f) * mapSize.y);
        }

        private void Refresh()
        {
            if (player == null)
            {
                return;
            }

            UpdateMarker(miniMapRoot, miniPlayerMarker);
            UpdateMarker(largeMapRoot, largePlayerMarker);
            string zoneName = zoneController != null && zoneController.CurrentZone != null
                ? zoneController.CurrentZone.DisplayName
                : "Неизвестная область";

            if (miniZoneText != null)
            {
                miniZoneText.text = zoneName;
            }

            if (largeZoneText != null)
            {
                largeZoneText.text = zoneName;
            }
        }

        private void UpdateMarker(RectTransform mapRoot, RectTransform marker)
        {
            if (mapRoot == null || marker == null)
            {
                return;
            }

            marker.anchoredPosition = WorldToMapPosition(player.position, mapRoot.rect.size);
            marker.localRotation = Quaternion.Euler(0f, 0f, -player.eulerAngles.y);
        }
    }
}
