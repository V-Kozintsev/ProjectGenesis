using ProjectGenesis.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectGenesis.UI
{
    public sealed class CharacterIdentityView : MonoBehaviour
    {
        [SerializeField] private Text characterNameText;
        [SerializeField] private Text identityDetailsText;
        [SerializeField] private PlayerIdentity identity;

        public PlayerIdentity Identity => identity;

        public void Initialize(
            Text nameLabel,
            Text detailsLabel,
            PlayerIdentity playerIdentity)
        {
            characterNameText = nameLabel;
            identityDetailsText = detailsLabel;
            identity = playerIdentity;
        }

        private void Awake()
        {
            if (identity != null)
            {
                identity.Changed += HandleIdentityChanged;
            }

            Refresh();
        }

        private void OnDestroy()
        {
            if (identity != null)
            {
                identity.Changed -= HandleIdentityChanged;
            }
        }

        private void HandleIdentityChanged(PlayerIdentity _)
        {
            Refresh();
        }

        private void Refresh()
        {
            if (identity == null)
            {
                return;
            }

            if (characterNameText != null)
            {
                characterNameText.text = identity.CharacterName;
            }

            if (identityDetailsText != null)
            {
                string raceName = identity.Race != null ? identity.Race.DisplayName : "Раса не задана";
                string className = identity.CharacterClass != null
                    ? identity.CharacterClass.DisplayName
                    : "Класс не задан";
                identityDetailsText.text = $"{raceName} · {className}";
            }
        }
    }
}
