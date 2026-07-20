using ProjectGenesis.Gameplay;
using ProjectGenesis.Saving;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectGenesis.UI
{
    public sealed class CharacterEntryView : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private GameObject creationModeRoot;
        [SerializeField] private GameObject selectionModeRoot;
        [SerializeField] private InputField nameInput;
        [SerializeField] private Button createButton;
        [SerializeField] private Button playButton;
        [SerializeField] private Text creationFeedbackText;
        [SerializeField] private Text selectedNameText;
        [SerializeField] private Text selectedIdentityText;
        [SerializeField] private Text selectedLevelText;
        [SerializeField] private PlayerPersistenceController persistence;
        [SerializeField] private PlayerIdentity identity;
        [SerializeField] private PlayerProgression progression;
        [SerializeField] private MonoBehaviour[] gameplayBehaviours;

        public GameObject Root => root;
        public PlayerPersistenceController Persistence => persistence;
        public int GameplayBehaviourCount => gameplayBehaviours != null
            ? gameplayBehaviours.Length
            : 0;

        public void Initialize(
            GameObject overlayRoot,
            GameObject creationRoot,
            GameObject selectionRoot,
            InputField characterNameInput,
            Button characterCreateButton,
            Button characterPlayButton,
            Text creationFeedback,
            Text selectedName,
            Text selectedIdentity,
            Text selectedLevel,
            PlayerPersistenceController playerPersistence,
            PlayerIdentity playerIdentity,
            PlayerProgression playerProgression,
            MonoBehaviour[] gatedBehaviours)
        {
            root = overlayRoot;
            creationModeRoot = creationRoot;
            selectionModeRoot = selectionRoot;
            nameInput = characterNameInput;
            createButton = characterCreateButton;
            playButton = characterPlayButton;
            creationFeedbackText = creationFeedback;
            selectedNameText = selectedName;
            selectedIdentityText = selectedIdentity;
            selectedLevelText = selectedLevel;
            persistence = playerPersistence;
            identity = playerIdentity;
            progression = playerProgression;
            gameplayBehaviours = gatedBehaviours;
        }

        private void Awake()
        {
            createButton?.onClick.AddListener(HandleCreateCharacter);
            playButton?.onClick.AddListener(HandleEnterGameplay);
            nameInput?.onValueChanged.AddListener(HandleNameChanged);

            if (persistence != null)
            {
                persistence.Initialized += HandlePersistenceInitialized;
            }

            if (identity != null)
            {
                identity.Changed += HandleIdentityChanged;
            }

            SetGameplayAvailable(false);
            root?.SetActive(true);

            if (persistence != null && persistence.IsInitialized)
            {
                RefreshMode();
            }
        }

        private void OnDestroy()
        {
            createButton?.onClick.RemoveListener(HandleCreateCharacter);
            playButton?.onClick.RemoveListener(HandleEnterGameplay);
            nameInput?.onValueChanged.RemoveListener(HandleNameChanged);

            if (persistence != null)
            {
                persistence.Initialized -= HandlePersistenceInitialized;
            }

            if (identity != null)
            {
                identity.Changed -= HandleIdentityChanged;
            }
        }

        private void HandlePersistenceInitialized(PlayerPersistenceController _)
        {
            RefreshMode();
        }

        private void HandleIdentityChanged(PlayerIdentity _)
        {
            RefreshSelectionDetails();
        }

        private void HandleNameChanged(string value)
        {
            if (createButton != null)
            {
                createButton.interactable = PlayerIdentity.IsAcceptableName(value);
            }

            if (creationFeedbackText != null)
            {
                creationFeedbackText.text = string.Empty;
            }
        }

        private void HandleCreateCharacter()
        {
            string requestedName = nameInput != null ? nameInput.text : string.Empty;
            if (persistence == null || !persistence.TryCreateCharacter(requestedName))
            {
                if (creationFeedbackText != null)
                {
                    creationFeedbackText.text = "Введите имя персонажа.";
                }

                return;
            }

            ShowSelectionMode();
        }

        private void HandleEnterGameplay()
        {
            if (persistence == null || !persistence.HasCreatedCharacter)
            {
                return;
            }

            root?.SetActive(false);
            SetGameplayAvailable(true);
        }

        private void RefreshMode()
        {
            if (persistence == null || !persistence.IsInitialized)
            {
                return;
            }

            if (persistence.HasCreatedCharacter)
            {
                ShowSelectionMode();
            }
            else
            {
                ShowCreationMode();
            }
        }

        private void ShowCreationMode()
        {
            creationModeRoot?.SetActive(true);
            selectionModeRoot?.SetActive(false);

            if (nameInput != null)
            {
                nameInput.text = string.Empty;
            }

            if (createButton != null)
            {
                createButton.interactable = false;
            }

            if (creationFeedbackText != null)
            {
                creationFeedbackText.text = string.Empty;
            }
        }

        private void ShowSelectionMode()
        {
            creationModeRoot?.SetActive(false);
            selectionModeRoot?.SetActive(true);
            RefreshSelectionDetails();
        }

        private void RefreshSelectionDetails()
        {
            if (identity == null)
            {
                return;
            }

            if (selectedNameText != null)
            {
                selectedNameText.text = identity.CharacterName;
            }

            if (selectedIdentityText != null)
            {
                string raceName = identity.Race != null ? identity.Race.DisplayName : "Раса не задана";
                string className = identity.CharacterClass != null
                    ? identity.CharacterClass.DisplayName
                    : "Класс не задан";
                selectedIdentityText.text = $"{raceName} · {className}";
            }

            if (selectedLevelText != null)
            {
                int level = progression != null ? progression.Level : 1;
                selectedLevelText.text = $"Уровень {level}";
            }
        }

        private void SetGameplayAvailable(bool available)
        {
            if (gameplayBehaviours == null)
            {
                return;
            }

            foreach (MonoBehaviour behaviour in gameplayBehaviours)
            {
                if (behaviour != null)
                {
                    behaviour.enabled = available;
                }
            }
        }
    }
}
